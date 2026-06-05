using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Requests;

/// <summary>
/// Represents the send grid contacts get by email request.
/// </summary>
public class SendGridContactsGetByEmailRequest
{
    /// <summary>
    /// Gets or sets emails.
    /// </summary>
    [JsonPropertyName("emails")]
    public List<string> Emails { get; set; } = default!;
}