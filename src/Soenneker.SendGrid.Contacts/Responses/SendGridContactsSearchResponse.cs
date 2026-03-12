using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

public class SendGridContactsSearchResponse
{
    [JsonPropertyName("result")]
    public List<SendGridContactGetResponse>? Result { get; set; }

    [JsonPropertyName("contact_count")]
    public int ContactCount { get; set; }
}