using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

/// <summary>
/// Represents the send grid contact metadata.
/// </summary>
public class SendGridContactMetadata
{
    /// <summary>
    /// Gets or sets self.
    /// </summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }
}