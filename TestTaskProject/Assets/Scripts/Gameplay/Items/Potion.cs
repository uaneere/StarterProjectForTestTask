using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Items
{
    public class Potion : Item
    {
        public IReadOnlyDictionary<IngredientEffect, int> Effects { get; }

        public Potion(
            string id,
            string name,
            string description,
            Dictionary<IngredientEffect, int> effects,
            Sprite icon = null)
            : base(id, name, description, icon)
        {
            Effects = effects;
        }

        public static Dictionary<IngredientEffect, int> StackEffects(
            IEnumerable<ProcessedIngredient> ingredients)
        {
            var stacked = new Dictionary<IngredientEffect, int>();

            foreach (var ingredient in ingredients)
            {
                if (!stacked.ContainsKey(ingredient.Effect))
                    stacked[ingredient.Effect] = 0;

                stacked[ingredient.Effect]++;
            }

            return stacked;
        }

        public string FormatEffects()
        {
            var parts = new List<string>();

            foreach (var pair in Effects)
                parts.Add($"{pair.Key} {pair.Value}");

            return parts.Count == 0 ? "нет эффектов" : string.Join(", ", parts);
        }
    }
}