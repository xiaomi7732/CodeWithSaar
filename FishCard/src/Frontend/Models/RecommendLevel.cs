using System.Text.Json.Serialization;

namespace CodeWithSaar.FishCard.Models;

[JsonConverter(typeof(EnumStringConverter<RecommendLevel>))]
public enum RecommendLevel
{
    Best,
    Good,
    Avoid
}