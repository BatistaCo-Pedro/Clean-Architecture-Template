namespace Application.Models.Dtos;

/// <summary>
/// 
/// </summary>
/// <param name="Value"></param>
/// <param name="Alt"></param>
/// <param name="Name"></param>
[Serializable]
[method: JsonConstructor]
public record AttachmentDto(
    [property: JsonPropertyName("value")] NonEmptyString Value,
    [property: JsonPropertyName("alt")] NonEmptyString Alt,
    [property: JsonPropertyName("name")] NonEmptyString Name
);