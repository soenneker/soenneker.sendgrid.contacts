using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

/// <summary>
/// Represents the send grid contacts search response.
/// </summary>
public class SendGridContactsSearchResponse
{
    /// <summary>
    /// Gets or sets result.
    /// </summary>
    [JsonPropertyName("result")]
    public List<SendGridContactGetResponse>? Result { get; set; }

    /// <summary>
    /// Gets or sets contact count.
    /// </summary>
    [JsonPropertyName("contact_count")]
    public int ContactCount { get; set; }
}