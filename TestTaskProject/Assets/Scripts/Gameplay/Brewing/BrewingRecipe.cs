using System.Collections.Generic;
using Gameplay.Items;

namespace Gameplay.Brewing
{
    public class BrewingRecipe
    {
        public string PotionId { get; }
        public string PotionName { get; }
        public IReadOnlyList<ProcessedIngredient> Steps { get; }

        public BrewingRecipe(
            string potionId,
            string potionName,
            IReadOnlyList<ProcessedIngredient> steps)
        {
            PotionId = potionId;
            PotionName = potionName;
            Steps = steps;
        }
    }
}