using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

public class SendGridContactMetadata
{
    [JsonPropertyName("self")]
    public string? Self { get; set; }
}