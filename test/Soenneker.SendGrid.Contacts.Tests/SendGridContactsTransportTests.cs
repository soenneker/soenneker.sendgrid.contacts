using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.SendGrid.OpenApiClient;
using Soenneker.SendGrid.Contacts.Requests;

namespace Soenneker.SendGrid.Contacts.Tests;

public sealed class SendGridContactsTransportTests
{
    [Test]
    public async Task Contact_operations_use_openapi_routes_and_preserve_payloads()
    {
        using var handler = new ContactsHttpMessageHandler();
        using var http = new HttpClient(handler);
        using var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: http)
        {
            BaseUrl = "https://sendgrid.test"
        };
        var client = new SendGridOpenApiClient(adapter);
        var util = new SendGridContactsUtil(new ContactsOpenApiClientUtil(client), NullLogger<SendGridContactsUtil>.Instance,
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["SendGrid:ClientBaseUrl"] = "https://sendgrid.test/" }).Build());
        const string contact = "{\"id\":\"contact-id\",\"email\":\"contact@example.com\",\"custom_fields\":{\"w1\":\"value\"}}";
        string expectedMethod = "PUT";
        string expectedPath = "/v3/marketing/contacts";
        string response = "{\"job_id\":\"11111111-1111-1111-1111-111111111111\"}";
        string? requestBody = null;
        handler.Respond = async (request, token) =>
        {
            if (request.Method.Method != expectedMethod || request.RequestUri!.PathAndQuery != expectedPath)
                throw new InvalidOperationException($"Unexpected request: {request.Method} {request.RequestUri}");
            requestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(token);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(response, Encoding.UTF8, "application/json") };
        };
        var job = await util.AddOrUpdate(new SendGridContactsRequest
        {
            Contacts = [new SendGridContactRequest { Email = "contact@example.com", CustomFields = new Dictionary<string, string> { ["w1"] = "value" } }]
        });
        using (JsonDocument body = JsonDocument.Parse(requestBody!))
        {
            if (body.RootElement.GetProperty("contacts")[0].GetProperty("custom_fields").GetProperty("w1").GetString() != "value" || job.JobId != "11111111-1111-1111-1111-111111111111")
                throw new InvalidOperationException("Upsert payload or response changed.");
        }
        expectedMethod = "DELETE";
        expectedPath += "?ids=one%2Ctwo";
        await util.Delete(["one", "two"]);
        expectedPath = "/v3/marketing/contacts?delete_all_contacts=true";
        await util.DeleteAll();
        expectedMethod = "GET";
        expectedPath = "/v3/marketing/contacts/contact-id";
        response = contact;
        var found = await util.Get("contact-id");
        if (found.CustomFields?["w1"] != "value")
            throw new InvalidOperationException("Contact fields were lost.");
        expectedMethod = "POST";
        expectedPath = "/v3/marketing/contacts/search";
        response = "{\"result\":[" + contact + "],\"contact_count\":1}";
        var searched = await util.Search("contact@example.com", "list-id");
        using (JsonDocument body = JsonDocument.Parse(requestBody!))
        {
            if (body.RootElement.GetProperty("query").GetString() != "email = 'contact@example.com' AND CONTAINS(list_ids, 'list-id')" || searched.Result?.Count != 1)
                throw new InvalidOperationException("Search payload or response changed.");
        }
        expectedPath = "/v3/marketing/contacts/search/emails";
        response = "{\"result\":{\"contact@example.com\":{\"contact\":" + contact + "}}}";
        var byEmail = await util.Get(new List<string> { "contact@example.com" });
        if (byEmail.ContactCount != 1 || byEmail.Result?[0].Email != "contact@example.com")
            throw new InvalidOperationException("Email lookup did not map its results.");
    }
}
