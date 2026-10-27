using Soenneker.SendGrid.OpenApiClientUtil.Abstract;
using Soenneker.SendGrid.OpenApiClient;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;
using Soenneker.SendGrid.OpenApiClient.Models;
using Soenneker.SendGrid.OpenApiClient.Tsg_mc_contacts_v3.V3.Marketing.Contacts;
using System.Text.Json;
using System.Text;
using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.SendGrid.Contacts.Requests;
using System.Threading.Tasks;
using Soenneker.Utils.Json;
using System.Collections.Generic;
using Soenneker.Extensions.Enumerable.String;
using Soenneker.SendGrid.Contacts.Responses;
using Kevlar;
using System;
using System.Linq;
using System.IO;
using System.Threading;
using Soenneker.Utils.Random;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Soenneker.Extensions.Enumerable;
using Soenneker.Extensions.ValueTask;
using Soenneker.Extensions.Task;

namespace Soenneker.SendGrid.Contacts;

public sealed class SendGridContactsUtil : ISendGridContactsUtil
{
    private readonly ISendGridOpenApiClientUtil _sendGridOpenApiClientUtil;
    private readonly ILogger<SendGridContactsUtil> _logger;
    private readonly string _contactsUrl;

    public SendGridContactsUtil(ISendGridOpenApiClientUtil sendGridOpenApiClientUtil, ILogger<SendGridContactsUtil> logger, IConfiguration configuration)
    {
        _sendGridOpenApiClientUtil = sendGridOpenApiClientUtil;
        _logger = logger;
        _contactsUrl = $"{(configuration["SendGrid:ClientBaseUrl"] ?? "https://api.sendgrid.com").TrimEnd('/')}/v3/marketing/contacts";
    }

    public async ValueTask<SendGridContactsJobResponse> AddOrUpdate(SendGridContactsRequest request, CancellationToken cancellationToken = default)
    {
        string? json = JsonUtil.Serialize(request, LibraryJsonContext.Get<SendGridContactsRequest>());

        ContactsRequestBuilder contacts = await GetContacts(cancellationToken).NoSync();
        using var body = new MemoryStream(Encoding.UTF8.GetBytes(json!));
        IParseNode root = await new JsonParseNodeFactory().GetRootParseNodeAsync("application/json", body, cancellationToken).NoSync();
        UpdateContactRequest payload = root.GetObjectValue(UpdateContactRequest.CreateFromDiscriminatorValue)!;
        UpdateContact202Response? response = await contacts.PutAsync(payload, cancellationToken: cancellationToken).NoSync();
        return await DeserializeBody<SendGridContactsJobResponse>(response, cancellationToken).NoSync();
    }

    public async ValueTask<SendGridContactGetResponse?> AddAndWait(SendGridContactsRequest request, CancellationToken cancellationToken = default)
    {
        SendGridContactsJobResponse _ = await AddOrUpdate(request, cancellationToken).NoSync();

        string? listId = request.ListIds?.FirstOrDefault();

        SendGridContactGetResponse? waitResponse = await WaitForSendGridContact(request.Contacts.First().Email, listId, cancellationToken).NoSync();
        return waitResponse;
    }

    private async ValueTask<SendGridContactGetResponse?> WaitForSendGridContact(string email, string? listId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("*** WaitForSendGridContact *** Verifying email ({email}) for list ({listId})", email, listId);

        SendGridContactGetResponse? contact = null;

        await Task.Delay(15000, cancellationToken).NoSync();

        try
        {
            Shield retryShield = Shield.When<Exception>()
                .Retry(options =>
                {
                    options.MaxRetries = 5;
                    options.Backoff = Backoff.Custom(static attempt => TimeSpan.FromSeconds(5 + Math.Pow(2, attempt))
                        + TimeSpan.FromMilliseconds(RandomUtil.Next(0, 3000)));
                    options.OnRetry = retry =>
                    {
                        _logger.LogDebug(retry.Exception,
                            "*** InternalWaitOnSendGridContact *** Failed to find SendGrid contact ({email}). Trying again in {delay}s ... count: {retryCount}",
                            email, retry.Delay.TotalSeconds, retry.AttemptNumber + 1);
                        return default;
                    };
                });

            await retryShield.ExecuteAsync(async token => { contact = await InternalWaitOnSendGridContact(email, listId, token).NoSync(); }, cancellationToken)
                             .NoSync();

            return contact;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "*** WaitForSendGridContact *** Failed to wait on lead contact creation ({email}), there may be an issue", email);
        }

        return null;
    }

    private async ValueTask<SendGridContactGetResponse> InternalWaitOnSendGridContact(string email, string? listId = null, CancellationToken cancellationToken = default)
    {
        SendGridContactsSearchResponse? response = await Search(email, listId, cancellationToken).NoSync();

        if (response == null)
            throw new Exception("Failed to find SendGrid contact");

        if (response.Result.IsNullOrEmpty())
            throw new Exception("Failed to find SendGrid contact");

        _logger.LogDebug("Found SendGrid contact! Exiting wait loop");

        SendGridContactGetResponse? result = response.Result.FirstOrDefault();

        if (result == null)
            throw new Exception("Failed to find SendGrid contact");

        return result;
    }

    public async ValueTask<SendGridContactsJobResponse> Delete(List<string> ids, CancellationToken cancellationToken = default)
    {
        string commaSeparatedIds = ids.ToCommaSeparatedString();

        ContactsRequestBuilder contacts = await GetContacts(cancellationToken).NoSync();
        DeleteContact202Response? response = await contacts.WithUrl($"{contacts.ToGetRequestInformation().URI}?ids={Uri.EscapeDataString(commaSeparatedIds)}")
            .DeleteAsync(cancellationToken: cancellationToken).NoSync();
        return await DeserializeBody<SendGridContactsJobResponse>(response, cancellationToken).NoSync();
    }

    public async ValueTask<SendGridContactsJobResponse> DeleteAll(CancellationToken cancellationToken = default)
    {
        ContactsRequestBuilder contacts = await GetContacts(cancellationToken).NoSync();
        DeleteContact202Response? response = await contacts.WithUrl($"{contacts.ToGetRequestInformation().URI}?delete_all_contacts=true")
            .DeleteAsync(cancellationToken: cancellationToken).NoSync();
        return await DeserializeBody<SendGridContactsJobResponse>(response, cancellationToken).NoSync();
    }

    public async ValueTask<SendGridContactGetResponse> Get(string id, CancellationToken cancellationToken = default)
    {
        ContactsRequestBuilder contacts = await GetContacts(cancellationToken).NoSync();
        var response = await contacts[id].WithUrl($"{contacts.ToGetRequestInformation().URI}/{Uri.EscapeDataString(id)}")
            .GetAsync(cancellationToken: cancellationToken).NoSync();
        return await DeserializeBody<SendGridContactGetResponse>(response, cancellationToken).NoSync();
    }

    public async ValueTask<SendGridContactsSearchResponse> Search(string email, string? listId = null, CancellationToken cancellationToken = default)
    {
        string query = $"email = '{email}'";

        if (listId != null)
            query += $" AND CONTAINS(list_ids, '{listId}')";

        ContactsRequestBuilder contacts = await GetContacts(cancellationToken).NoSync();
        SearchContact200Response? response = await contacts.Search.WithUrl($"{contacts.ToGetRequestInformation().URI}/search")
            .PostAsync(new SearchContactRequest { Query = query }, cancellationToken: cancellationToken).NoSync();
        return await DeserializeBody<SendGridContactsSearchResponse>(response, cancellationToken).NoSync();
    }

    public async ValueTask<SendGridContactsSearchResponse> Get(List<string> emails, CancellationToken cancellationToken = default)
    {
        ContactsRequestBuilder contacts = await GetContacts(cancellationToken).NoSync();
        ListContactByEmail200Response? response = await contacts.Search.Emails.WithUrl($"{contacts.ToGetRequestInformation().URI}/search/emails")
            .PostAsync(new ListContactByEmailRequest { Emails = emails }, cancellationToken: cancellationToken).NoSync();

        using var writer = new JsonSerializationWriter();
        writer.WriteObjectValue(null, response ?? throw new InvalidOperationException("SendGrid returned an empty response body."));
        await using Stream stream = writer.GetSerializedContent();
        using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).NoSync();
        var result = new List<SendGridContactGetResponse>();

        if (document.RootElement.TryGetProperty("result", out JsonElement results))
        {
            foreach (JsonProperty entry in results.EnumerateObject())
            {
                if (entry.Value.TryGetProperty("contact", out JsonElement contact) && contact.ValueKind == JsonValueKind.Object)
                    result.Add(contact.Deserialize(LibraryJsonContext.Get<SendGridContactGetResponse>())!);
            }
        }

        return new SendGridContactsSearchResponse { Result = result, ContactCount = result.Count };
    }

    private async ValueTask<ContactsRequestBuilder> GetContacts(CancellationToken cancellationToken)
    {
        SendGridOpenApiClient client = await _sendGridOpenApiClientUtil.Get(cancellationToken).NoSync();
        // Override the merged specification's document prefix with the actual SendGrid API route.
        return client.Tsg_mc_contacts_v3.V3.Marketing.Contacts.WithUrl(_contactsUrl);
    }

    private async ValueTask<T> DeserializeBody<T>(IParsable? response, CancellationToken cancellationToken)
    {
        if (response is null)
            throw new InvalidOperationException("SendGrid returned an empty response body.");

        using var writer = new JsonSerializationWriter();
        writer.WriteObjectValue(null, response);
        await using Stream stream = writer.GetSerializedContent();
        return (await JsonUtil.Deserialize<T>(stream, LibraryJsonContext.Get<T>(), _logger, cancellationToken).NoSync())!;
    }
}
