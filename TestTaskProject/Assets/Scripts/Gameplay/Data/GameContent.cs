using System;
using System.Collections.Generic;
using Gameplay.Brewing;
using Gameplay.Inventory;
using Gameplay.Items;
using Gameplay.Presentation;

namespace Gameplay.Data
{
    public class GameContent
    {
        public IReadOnlyList<Ingredient> Ingredients { get; }
        public BrewingRecipe CurrentRecipe { get; private set; }
        public IReadOnlyList<BrewingRecipe> Recipes => _recipes;

        private readonly Random _random = new();
        private BrewingRecipe[] _recipes = Array.Empty<BrewingRecipe>();

        public GameContent()
        {
            var list = new List<Ingredient>(IngredientLibrary.All.Count);
            foreach (var definition in IngredientLibrary.All)
                list.Add(IngredientLibrary.Create(definition, _random));

            Ingredients = list;
            RerollRecipe();
        }

        public BrewingRecipe RerollRecipe()
        {
            var count = _random.Next(1, 5);
            var steps = new List<ProcessedIngredient>(count);

            for (var i = 0; i < count; i++)
            {
                var ingredient = Ingredients[_random.Next(Ingredients.Count)];
                var effect = _random.Next(2) == 0 ? ingredient.Effect1 : ingredient.Effect2;
                steps.Add(new ProcessedIngredient(ingredient, effect));
            }

            CurrentRecipe = new BrewingRecipe("brew", "Готовое зелье", steps);
            _recipes = new[] { CurrentRecipe };
            return CurrentRecipe;
        }

        public string FormatCurrentRecipe()
        {
            var parts = new string[CurrentRecipe.Steps.Count];
            for (var i = 0; i < CurrentRecipe.Steps.Count; i++)
            {
                var step = CurrentRecipe.Steps[i];
                parts[i] = $"{step.Ingredient.Name}+{step.Effect}";
            }

            var stacked = Potion.StackEffects(CurrentRecipe.Steps);
            var result = new List<string>();
            foreach (var pair in stacked)
                result.Add($"{pair.Key} {pair.Value}");

            return "Рецепт:\n" + string.Join("\n", parts) + "\nРезультат: " + string.Join(", ", result);
        }

        public Item[] CreateCabinetStock()
        {
            var items = new Item[Cabinet.SlotCount];
            for (var i = 0; i < items.Length; i++)
            {
                if (_random.NextDouble() < 0.25)
                    continue;

                items[i] = Ingredients[_random.Next(Ingredients.Count)];
            }

            return items;
        }
    }
}