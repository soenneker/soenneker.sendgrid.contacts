using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Requests;

public class SendGridContactsGetByEmailRequest
{
    [JsonPropertyName("emails")]
    public List<string> Emails { get; set; } = default!;
}