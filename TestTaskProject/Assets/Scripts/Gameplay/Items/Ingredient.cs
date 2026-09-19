using Gameplay.Minigame;
using UnityEngine;

namespace Gameplay.Items
{
    public class Ingredient : Item
    {
        public IngredientEffect Effect1 { get; }
        public IngredientEffect Effect2 { get; }
        public ArrowSequence QtePath1 { get; }
        public ArrowSequence QtePath2 { get; }

        public IngredientKind Kind { get; }

        public Ingredient(
            string id,
            string name,
            string description,
            IngredientEffect effect1,
            IngredientEffect effect2,
            ArrowSequence qtePath1,
            ArrowSequence qtePath2,
            Sprite icon = null,
            IngredientKind kind = default)
            : base(id, name, description, icon)
        {
            Effect1 = effect1;
            Effect2 = effect2;
            QtePath1 = qtePath1;
            QtePath2 = qtePath2;
            Kind = kind;
        }
    }
}