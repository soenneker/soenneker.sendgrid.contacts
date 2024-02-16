using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Requests;

/// <summary>
/// Represents a list of contacts.
/// </summary>
public class SendGridContactsRequest
{
    /// <summary>
    /// An array of List ID strings that this contact will be added to.
    /// </summary>
    [JsonPropertyName("list_ids")]
    public List<string>? ListIds { get; set; }

    /// <summary>
    /// One or more contacts objects that you intend to upsert.
    /// </summary>
    [JsonPropertyName("contacts")]
    public List<SendGridContactRequest> Contacts { get; set; } = default!;
}