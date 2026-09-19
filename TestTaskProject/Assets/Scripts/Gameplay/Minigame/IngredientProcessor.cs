using System;
using Gameplay.Items;

namespace Gameplay.Minigame
{
    public class IngredientProcessor
    {
        private readonly Random _random = new();

        public ProcessedIngredient Process(Ingredient ingredient, QteResult result)
        {
            IngredientEffect effect;

            switch (result)
            {
                case QteResult.Effect1:
                    effect = ingredient.Effect1;
                    break;
                case QteResult.Effect2:
                    effect = ingredient.Effect2;
                    break;
                case QteResult.Failed:
                    effect = GetRandomEffect();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }

            return new ProcessedIngredient(ingredient, effect);
        }

        private IngredientEffect GetRandomEffect()
        {
            var effects = (IngredientEffect[])Enum.GetValues(typeof(IngredientEffect));
            return effects[_random.Next(effects.Length)];
        }
    }
}