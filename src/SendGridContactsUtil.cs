using Soenneker.SendGrid.Client.Abstract;
using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.SendGrid.Contacts.Requests;
using System.Threading.Tasks;
using SendGrid;
using Soenneker.Utils.Json;
using System.Collections.Generic;
using Soenneker.Extensions.Enumerable.String;
using Soenneker.SendGrid.Contacts.Responses;

namespace Soenneker.SendGrid.Contacts;

/// <inheritdoc cref="ISendGridContactsUtil"/>
public class SendGridContactsUtil : ISendGridContactsUtil
{
    private readonly ISendGridClientUtil _sendGridClientUtil;

    public SendGridContactsUtil(ISendGridClientUtil sendGridClientUtil)
    {
        _sendGridClientUtil = sendGridClientUtil;
    }

    public async ValueTask<SendGridContactsJobResponse> AddOrUpdateMultiple(SendGridContactsRequest request)
    {
        string? json = JsonUtil.Serialize(request);

        SendGridClient client = await _sendGridClientUtil.Get();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.PUT,
            urlPath: "marketing/contacts",
            requestBody: json
        );

        string body = await response.Body.ReadAsStringAsync();
        var result = JsonUtil.Deserialize<SendGridContactsJobResponse>(body)!;

        return result;
    }

    public ValueTask<SendGridContactsJobResponse> AddOrUpdate(SendGridContactRequest request)
    {
        var multipleRequest = new SendGridContactsRequest
        {
            Contacts = [request]
        };

        return AddOrUpdateMultiple(multipleRequest);
    }

    public async ValueTask<SendGridContactsJobResponse> Delete(List<string> ids)
    {
        string commaSeparatedIds = ids.ToCommaSeparatedString();

        SendGridClient client = await _sendGridClientUtil.Get();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.DELETE,
            urlPath: $"marketing/contacts?ids={commaSeparatedIds}"
        );

        string body = await response.Body.ReadAsStringAsync();
        var result = JsonUtil.Deserialize<SendGridContactsJobResponse>(body)!;

        return result;
    }

    public async ValueTask<SendGridContactsJobResponse> DeleteAll()
    {
        SendGridClient client = await _sendGridClientUtil.Get();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.DELETE,
            urlPath: $"marketing/contacts?delete_all_contacts=true"
        );

        string body = await response.Body.ReadAsStringAsync();
        var result = JsonUtil.Deserialize<SendGridContactsJobResponse>(body)!;

        return result;
    }

    public async ValueTask<SendGridContactResponse> Get(string id)
    {
        SendGridClient client = await _sendGridClientUtil.Get();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.GET,
            urlPath: $"marketing/contacts/{id}"
        );

        string body = await response.Body.ReadAsStringAsync();
        var result = JsonUtil.Deserialize<SendGridContactResponse>(body)!;

        return result;
    }

    public async ValueTask<SendGridContactsGetByEmailResponse> Get(List<string> emails)
    {
        var request = new SendGridContactsGetByEmailRequest
        {
            Emails = emails
        };

        string? json = JsonUtil.Serialize(request);

        SendGridClient client = await _sendGridClientUtil.Get();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.POST,
            urlPath: "marketing/contacts",
            requestBody: json
        );

        string body = await response.Body.ReadAsStringAsync();
        var result = JsonUtil.Deserialize<SendGridContactsGetByEmailResponse>(body)!;

        return result;
    }
}