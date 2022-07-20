using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard;

internal class QueryFishService
{
    List<FishItem> _allFish = new List<FishItem>{
        new FishItem(){
            Name = "Anchovy",
            Level = RecommendLevel.Best,
        },
        new FishItem(){
            Name = "Atlantic croaker",
            Level = RecommendLevel.Best
        },
        new FishItem(){
            Name="Atlantic mackerel",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Black sea bass",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Butterfish",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Catfish",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Clam",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Cod",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Crab",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Crawfish",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Flounder",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Haddock",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Hake",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Herring",
            Level = RecommendLevel.Best
        },
         new FishItem(){
            Name = "Lobster",
            Level = RecommendLevel.Best,
        },
        new FishItem(){
            Name = "American and spiny",
            Level = RecommendLevel.Best
        },
        new FishItem(){
            Name="Atlantic mackerel",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Mullet",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Oyster",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Pacific chub",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="mackerel",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Perch, freshwater and ocean",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Pickerel",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Plaice",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Pollock",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Salmon",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Sardine",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Scallop",
            Level = RecommendLevel.Best
        },
         new FishItem(){
            Name = "Shad",
            Level = RecommendLevel.Best,
        },
        new FishItem(){
            Name = "Shrimp",
            Level = RecommendLevel.Best
        },
        new FishItem(){
            Name="Skate",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Smelt",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Sole",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Squid",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Tilapia",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Trout, freshwater",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Tuna, canned light(includes skipjack)",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Crawfish",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Whitefish",
            Level = RecommendLevel.Best
        },
                new FishItem(){
            Name="Whiting",
            Level = RecommendLevel.Best
        },
        new FishItem(){
            Name="Bluefish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Buffalofish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Carp",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Chilean sea bass/Patagonian toothfish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Grouper",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Halibut",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Mahi mahi/dolphinfish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Buffalofish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Monkfish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Rockfish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Sablefish",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Sheepshead",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Snapper",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Spanish mackerel",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Striped bass (ocean)",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Tilefish (Atlantic Ocean)",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Tuna, albacore/ white tuna, canned and fresh/frozen",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Tuna, yellowfin",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="Weakfish/seatrout",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="White croaker/Pacific croaker",
            Level = RecommendLevel.Good
        },
        new FishItem(){
            Name="King mackerel",
            Level = RecommendLevel.Avoid
        },
        new FishItem(){
            Name="Marlin",
            Level = RecommendLevel.Avoid
        },
        new FishItem(){
            Name="Orange roughy",
            Level = RecommendLevel.Avoid
        },
        new FishItem(){
            Name="Shark",
            Level = RecommendLevel.Avoid
        },
        new FishItem(){
            Name="Swordfish",
            Level = RecommendLevel.Avoid
        },
        new FishItem(){
            Name="Tilefish (Gulf of Mexico)",
            Level = RecommendLevel.Avoid
        },
        new FishItem(){
            Name="Tuna, bigeye",
            Level = RecommendLevel.Avoid
        },
    };

    public Task<IEnumerable<FishItem>> GetFishesByAsync(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Task.FromResult(Enumerable.Empty<FishItem>());
        }

        keyword = keyword.Trim();
        return Task.FromResult(_allFish.Where(f => f.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
    }
}