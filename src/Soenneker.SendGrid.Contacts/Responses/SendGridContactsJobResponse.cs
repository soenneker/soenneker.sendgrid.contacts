using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

/// <summary>
/// Represents the send grid contacts job response.
/// </summary>
public class SendGridContactsJobResponse
{
    /// <summary>
    /// Gets or sets job id.
    /// </summary>
    [JsonPropertyName("job_id")]
    public string JobId { get; set; } = default!;
}