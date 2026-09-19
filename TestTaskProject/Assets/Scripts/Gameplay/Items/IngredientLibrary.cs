using System;
using System.Collections.Generic;
using Gameplay.Minigame;
using Gameplay.Presentation;
using UnityEngine;

namespace Gameplay.Items
{
    public static class IngredientLibrary
    {
        public static IReadOnlyList<IngredientDefinition> All { get; } = new[]
        {
            Def(IngredientKind.Amanita, "Мухомор", IngredientEffect.Poison, IngredientEffect.Healing),
            Def(IngredientKind.Artemisia, "Полынь", IngredientEffect.Poison, IngredientEffect.Speed),
            Def(IngredientKind.BracketFungus, "Трутовик", IngredientEffect.Healing, IngredientEffect.Fire),
            Def(IngredientKind.Chamomile, "Ромашка", IngredientEffect.Healing, IngredientEffect.Speed),
            Def(IngredientKind.Chanterelles, "Лисички", IngredientEffect.Healing, IngredientEffect.Fire),
            Def(IngredientKind.DragonScales, "Чешуя дракона", IngredientEffect.Fire, IngredientEffect.Speed),
            Def(IngredientKind.ElderBerry, "Бузина", IngredientEffect.Healing, IngredientEffect.Poison),
            Def(IngredientKind.HollyBerry, "Остролист", IngredientEffect.Poison, IngredientEffect.Fire),
            Def(IngredientKind.PettyMorel, "Сморчок", IngredientEffect.Poison, IngredientEffect.Healing),
            Def(IngredientKind.RavenEye, "Вороний глаз", IngredientEffect.Poison, IngredientEffect.Speed),
            Def(IngredientKind.Roman, "Рябина", IngredientEffect.Healing, IngredientEffect.Fire),
            Def(IngredientKind.Sage, "Шалфей", IngredientEffect.Healing, IngredientEffect.Speed),
            Def(IngredientKind.StarMushroom, "Звёздный гриб", IngredientEffect.Fire, IngredientEffect.Speed),
            Def(IngredientKind.SwampMint, "Болотная мята", IngredientEffect.Speed, IngredientEffect.Healing),
            Def(IngredientKind.ToadCaviar, "Икра жабы", IngredientEffect.Poison, IngredientEffect.Fire)
        };

        public static Ingredient Create(IngredientDefinition definition, System.Random random)
        {
            return new Ingredient(
                definition.Kind.ToString().ToLowerInvariant(),
                definition.DisplayName,
                $"Путь 1: {definition.Effect1}. Путь 2: {definition.Effect2}.",
                definition.Effect1,
                definition.Effect2,
                RandomSequence(random),
                RandomSequence(random),
                SpriteLoader.Load(definition.Kind.ToString()),
                definition.Kind);
        }

        private static IngredientDefinition Def(
            IngredientKind kind,
            string name,
            IngredientEffect effect1,
            IngredientEffect effect2)
        {
            return new IngredientDefinition(kind, name, effect1, effect2);
        }

        private static ArrowSequence RandomSequence(System.Random random)
        {
            var length = 3 + random.Next(2);
            var arrows = new Arrow[length];
            var values = (Arrow[])Enum.GetValues(typeof(Arrow));
            for (var i = 0; i < length; i++)
                arrows[i] = values[random.Next(values.Length)];
            return new ArrowSequence(arrows);
        }
    }

    public readonly struct IngredientDefinition
    {
        public IngredientKind Kind { get; }
        public string DisplayName { get; }
        public IngredientEffect Effect1 { get; }
        public IngredientEffect Effect2 { get; }

        public IngredientDefinition(
            IngredientKind kind,
            string displayName,
            IngredientEffect effect1,
            IngredientEffect effect2)
        {
            Kind = kind;
            DisplayName = displayName;
            Effect1 = effect1;
            Effect2 = effect2;
        }
    }
}