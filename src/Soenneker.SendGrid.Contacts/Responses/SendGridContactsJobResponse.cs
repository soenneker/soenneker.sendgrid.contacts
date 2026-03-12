using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

public class SendGridContactsJobResponse
{
    [JsonPropertyName("job_id")]
    public string JobId { get; set; } = default!;
}