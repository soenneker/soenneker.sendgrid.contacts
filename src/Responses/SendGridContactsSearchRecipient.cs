using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

public class SendGridContactsSearchRecipient
{
    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("last_emailed")]
    public int LastEmailed { get; set; }

    [JsonPropertyName("last_clicked")]
    public int LastClicked { get; set; }

    [JsonPropertyName("last_opened")]
    public int LastOpened { get; set; }

    [JsonPropertyName("custom_fields")]
    public List<object> CustomFields { get; set; }

    [JsonPropertyName("updated_at")]
    public int UpdatedAt { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
}