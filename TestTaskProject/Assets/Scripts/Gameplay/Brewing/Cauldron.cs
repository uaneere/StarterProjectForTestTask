using System.Collections.Generic;
using Gameplay.Items;
using Gameplay.Presentation;

namespace Gameplay.Brewing
{
    public class Cauldron
    {
        private readonly List<ProcessedIngredient> _contents = new();

        public IReadOnlyList<ProcessedIngredient> Contents => _contents;

        public void AddIngredient(ProcessedIngredient ingredient)
        {
            _contents.Add(ingredient);
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public bool TryFinish(IReadOnlyList<BrewingRecipe> recipes, out Potion potion)
        {
            potion = null;

            if (_contents.Count == 0)
                return false;

            foreach (var recipe in recipes)
            {
                if (!Matches(recipe))
                    continue;

                var effects = Potion.StackEffects(_contents);
                potion = new Potion(
                    recipe.PotionId,
                    recipe.PotionName,
                    $"Эффекты: {FormatEffects(effects)}",
                    effects,
                    VisualCatalog.PotionIcon);
                _contents.Clear();
                return true;
            }

            _contents.Clear();
            return false;
        }

        public bool Matches(BrewingRecipe recipe)
        {
            if (_contents.Count != recipe.Steps.Count)
                return false;

            for (var i = 0; i < _contents.Count; i++)
            {
                if (_contents[i].Ingredient.Id != recipe.Steps[i].Ingredient.Id)
                    return false;

                if (_contents[i].Effect != recipe.Steps[i].Effect)
                    return false;
            }

            return true;
        }

        private static string FormatEffects(Dictionary<IngredientEffect, int> effects)
        {
            var parts = new List<string>();

            foreach (var pair in effects)
                parts.Add($"{pair.Key} {pair.Value}");

            return string.Join(", ", parts);
        }
    }
}