using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

public class SendGridContactsGetByEmailResponse
{
    [JsonPropertyName("result")]
    public Dictionary<string, SendGridContactResponse>? Result { get; set; }
}