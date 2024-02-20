using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

public class SendGridContactsSearchResponse
{
    [JsonPropertyName("recipients")]
    public List<SendGridContactsSearchRecipient> Recipients { get; set; }

    [JsonPropertyName("recipient_count")]
    public int RecipientCount { get; set; }
}