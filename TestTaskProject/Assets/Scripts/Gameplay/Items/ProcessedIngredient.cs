namespace Gameplay.Items
{
    public class ProcessedIngredient
    {
        public Ingredient Ingredient { get; private set; }
        public IngredientEffect Effect { get; private set; }

        public ProcessedIngredient(
            Ingredient ingredient,
            IngredientEffect effect)
        {
            Ingredient = ingredient;
            Effect = effect;
        }
    }
}