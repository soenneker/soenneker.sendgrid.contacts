using Kevlar;
using SendGrid;
using Soenneker.SendGrid.Contacts.Requests;
using Soenneker.SendGrid.Contacts.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Soenneker.SendGrid.Contacts;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, ReadCommentHandling = JsonCommentHandling.Skip, UseStringEnumConverter = true)]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(SendGridContactGetResponse))]
[JsonSerializable(typeof(SendGridContactsGetByEmailRequest))]
[JsonSerializable(typeof(SendGridContactsJobResponse))]
[JsonSerializable(typeof(SendGridContactsRequest))]
[JsonSerializable(typeof(SendGridContactsSearchResponse))]
internal partial class LibraryJsonContext : JsonSerializerContext
{
    internal static JsonTypeInfo<T> Get<T>() =>
        (JsonTypeInfo<T>)(Default.GetTypeInfo(typeof(T)) ?? throw new NotSupportedException($"No generated JSON metadata for {typeof(T)}."));
}
