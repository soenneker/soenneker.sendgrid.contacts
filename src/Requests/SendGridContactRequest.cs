using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.SendGrid.Contacts.Requests;

/// <summary>
/// Represents a contact.
/// </summary>
public class SendGridContactRequest
{
    /// <summary>
    /// The first line of the address.
    /// </summary>
    [JsonPropertyName("address_line_1")]
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// An optional second line for the address.
    /// </summary>
    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Additional emails associated with the contact.
    /// </summary>
    [JsonPropertyName("alternate_emails")]
    public List<string>? AlternateEmails { get; set; }

    /// <summary>
    /// The contact's city.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// The contact's country. Can be a full name or an abbreviation.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// The contact's primary email. This is required to be a valid email.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// The contact's personal name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The contact's family name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// The contact's ZIP code or other postal code.
    /// </summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// The contact's state, province, or region.
    /// </summary>
    [JsonPropertyName("state_province_region")]
    public string? StateProvinceRegion { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// An object of custom field IDs and the values you want to associate with those custom fields.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public Dictionary<string, string>? CustomFields { get; set; }
}