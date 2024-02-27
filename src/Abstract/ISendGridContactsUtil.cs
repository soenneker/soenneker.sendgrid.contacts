using System.Collections.Generic;
using System.Threading.Tasks;
using Soenneker.SendGrid.Contacts.Requests;
using Soenneker.SendGrid.Contacts.Responses;

namespace Soenneker.SendGrid.Contacts.Abstract;

/// <summary>
/// A .NET typesafe implementation of SendGrid's Contact API
/// </summary>
public interface ISendGridContactsUtil
{
    /// <summary>
    /// Adds or updates contacts in SendGrid.
    /// </summary>
    /// <param name="request">The request containing contacts information.</param>
    /// <returns>A response containing information about the operation.</returns>
    ValueTask<SendGridContactsJobResponse> AddOrUpdate(SendGridContactsRequest request);

    ValueTask<SendGridContactGetResponse?> AddAndWait(SendGridContactsRequest request);

    /// <summary>
    /// Deletes contacts from SendGrid.
    /// </summary>
    /// <param name="ids">The IDs of the contacts to delete.</param>
    /// <returns>A response containing information about the operation.</returns>
    ValueTask<SendGridContactsJobResponse> Delete(List<string> ids);

    /// <summary>
    /// Deletes all contacts from SendGrid.
    /// </summary>
    /// <returns>A response containing information about the operation.</returns>
    ValueTask<SendGridContactsJobResponse> DeleteAll();

    /// <summary>
    /// Gets information about a contact from SendGrid by ID.
    /// </summary>
    /// <param name="id">The ID of the contact.</param>
    /// <returns>A response containing information about the contact.</returns>
    ValueTask<SendGridContactGetResponse> Get(string id);

    /// <summary>
    /// Gets information about contacts from SendGrid by email addresses.
    /// </summary>
    /// <param name="emails">The email addresses of the contacts.</param>
    /// <returns>A response containing information about the contacts.</returns>
    ValueTask<SendGridContactsSearchResponse> Get(List<string> emails);

    ValueTask<SendGridContactsSearchResponse> Search(string email, string? listId = null);
}