using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Responses;

/// <summary>
/// Represents the send grid contact get response.
/// </summary>
public class SendGridContactGetResponse
{
    /// <summary>
    /// Gets or sets address line1.
    /// </summary>
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets address line2.
    /// </summary>
    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets alternate emails.
    /// </summary>
    [JsonPropertyName("alternate_emails")]
    public List<string>? AlternateEmails { get; set; }

    /// <summary>
    /// Gets or sets city.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets country.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets email.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets list ids.
    /// </summary>
    [JsonPropertyName("list_ids")]
    public List<string>? ListIds { get; set; }

    /// <summary>
    /// Gets or sets segment ids.
    /// </summary>
    [JsonPropertyName("segment_ids")]
    public List<string>? SegmentIds { get; set; }

    /// <summary>
    /// Gets or sets postal code.
    /// </summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets state province region.
    /// </summary>
    [JsonPropertyName("state_province_region")]
    public string? StateProvinceRegion { get; set; }

    /// <summary>
    /// Gets or sets phone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whatsapp.
    /// </summary>
    [JsonPropertyName("whatsapp")]
    public string? Whatsapp { get; set; }

    /// <summary>
    /// Gets or sets line.
    /// </summary>
    [JsonPropertyName("line")]
    public string? Line { get; set; }

    /// <summary>
    /// Gets or sets facebook.
    /// </summary>
    [JsonPropertyName("facebook")]
    public string? Facebook { get; set; }

    /// <summary>
    /// Gets or sets unique name.
    /// </summary>
    [JsonPropertyName("unique_name")]
    public string? UniqueName { get; set; }

    /// <summary>
    /// Gets or sets custom fields.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public Dictionary<string, string>? CustomFields { get; set; }

    /// <summary>
    /// Gets or sets created at.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets updated at.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets metadata.
    /// </summary>
    [JsonPropertyName("_metadata")]
    public SendGridContactMetadata? Metadata { get; set; }
}