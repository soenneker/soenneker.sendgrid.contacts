using Soenneker.SendGrid.Client.Abstract;
using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.SendGrid.Contacts.Requests;
using System.Threading.Tasks;
using SendGrid;
using Soenneker.Utils.Json;
using System.Collections.Generic;
using Soenneker.Extensions.Enumerable.String;
using Soenneker.SendGrid.Contacts.Responses;
using Polly.Retry;
using Polly;
using System;
using System.Linq;
using System.Threading;
using Soenneker.Utils.Random;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Enumerable;
using Soenneker.Extensions.ValueTask;
using Soenneker.Extensions.Task;

namespace Soenneker.SendGrid.Contacts;

/// <inheritdoc cref="ISendGridContactsUtil"/>
public class SendGridContactsUtil : ISendGridContactsUtil
{
    private readonly ISendGridClientUtil _sendGridClientUtil;
    private readonly ILogger<SendGridContactsUtil> _logger;

    public SendGridContactsUtil(ISendGridClientUtil sendGridClientUtil, ILogger<SendGridContactsUtil> logger)
    {
        _sendGridClientUtil = sendGridClientUtil;
        _logger = logger;
    }

    public async ValueTask<SendGridContactsJobResponse> AddOrUpdate(SendGridContactsRequest request, CancellationToken cancellationToken = default)
    {
        string? json = JsonUtil.Serialize(request);

        SendGridClient client = await _sendGridClientUtil.Get(cancellationToken).NoSync();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.PUT,
            urlPath: "marketing/contacts",
            requestBody: json, cancellationToken: cancellationToken).NoSync();

        string body = await response.Body.ReadAsStringAsync(cancellationToken).NoSync();
        var result = JsonUtil.Deserialize<SendGridContactsJobResponse>(body)!;

        return result;
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
            AsyncRetryPolicy? retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5 + Math.Pow(2, retryAttempt)) // exponential back-off with jitter
                                                      + TimeSpan.FromMilliseconds(RandomUtil.Next(0, 3000)),
                    (exception, timespan, retryCount) =>
                    {
                        _logger.LogDebug(exception, "*** InternalWaitOnSendGridContact *** Failed to find SendGrid contact ({email}). Trying again in {delay}s ... count: {retryCount}", email,
                            timespan.Seconds, retryCount);
                    });

            await retryPolicy.ExecuteAsync(async () => { contact = await InternalWaitOnSendGridContact(email, listId, cancellationToken).NoSync(); }).NoSync();

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

        SendGridClient client = await _sendGridClientUtil.Get(cancellationToken).NoSync();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.DELETE,
            urlPath: $"marketing/contacts?ids={commaSeparatedIds}", cancellationToken: cancellationToken);

        string body = await response.Body.ReadAsStringAsync(cancellationToken).NoSync();
        var result = JsonUtil.Deserialize<SendGridContactsJobResponse>(body)!;

        return result;
    }

    public async ValueTask<SendGridContactsJobResponse> DeleteAll(CancellationToken cancellationToken = default)
    {
        SendGridClient client = await _sendGridClientUtil.Get(cancellationToken).NoSync();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.DELETE,
            urlPath: $"marketing/contacts?delete_all_contacts=true", cancellationToken: cancellationToken).NoSync();

        string body = await response.Body.ReadAsStringAsync(cancellationToken).NoSync();
        var result = JsonUtil.Deserialize<SendGridContactsJobResponse>(body)!;

        return result;
    }

    public async ValueTask<SendGridContactGetResponse> Get(string id, CancellationToken cancellationToken = default)
    {
        SendGridClient client = await _sendGridClientUtil.Get(cancellationToken).NoSync();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.GET,
            urlPath: $"marketing/contacts/{id}", cancellationToken: cancellationToken).NoSync();

        string body = await response.Body.ReadAsStringAsync(cancellationToken).NoSync();
        var result = JsonUtil.Deserialize<SendGridContactGetResponse>(body)!;

        return result;
    }

    public async ValueTask<SendGridContactsSearchResponse> Search(string email, string? listId = null, CancellationToken cancellationToken = default)
    {
        SendGridClient client = await _sendGridClientUtil.Get(cancellationToken).NoSync();

        string query = $"email = '{email}'";

        if (listId != null)
            query += $" AND CONTAINS(list_ids, '{listId}')";

        Dictionary<string, string> body = new Dictionary<string, string> {{"query", query}};

        string? json = JsonUtil.Serialize(body);

        Response response = await client.RequestAsync(
            method: BaseClient.Method.POST,
            urlPath: "marketing/contacts/search",
            requestBody: json, cancellationToken: cancellationToken).NoSync();

        string responseBody = await response.Body.ReadAsStringAsync(cancellationToken).NoSync();
        var result = JsonUtil.Deserialize<SendGridContactsSearchResponse>(responseBody)!;

        return result;
    }

    public async ValueTask<SendGridContactsSearchResponse> Get(List<string> emails, CancellationToken cancellationToken = default)
    {
        var request = new SendGridContactsGetByEmailRequest
        {
            Emails = emails
        };

        string? json = JsonUtil.Serialize(request);

        SendGridClient client = await _sendGridClientUtil.Get(cancellationToken).NoSync();

        Response response = await client.RequestAsync(
            method: BaseClient.Method.POST,
            urlPath: "marketing/contacts",
            requestBody: json, cancellationToken: cancellationToken).NoSync();

        string body = await response.Body.ReadAsStringAsync(cancellationToken).NoSync();
        var result = JsonUtil.Deserialize<SendGridContactsSearchResponse>(body)!;

        return result;
    }
}