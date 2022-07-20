using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard;

internal class RecommendService
{
    public string GetRecommendation(FishItem fish)
    {
        return fish.Level switch
        {
            RecommendLevel.Best => "Eat 2 to 3 servings a week ",
            RecommendLevel.Good => "Eat 1 serving a week",
            RecommendLevel.Avoid => "Do not eat it.",
            _ => $"Unknown recommendation level: {fish.Level}",
        };
    }
}