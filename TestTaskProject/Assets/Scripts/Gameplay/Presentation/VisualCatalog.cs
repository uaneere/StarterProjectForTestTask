using UnityEngine;

namespace Gameplay.Presentation
{
    public static class VisualCatalog
    {
        public const string MageName = "Маг";
        public const string CabinetName = "Сундук";
        public const string CauldronName = "Котёл";
        public const string BedName = "Кровать";
        public const string TableName = "Стол";

        public static Sprite BackgroundRoom => SpriteLoader.Load("Background1", new Color(0.28f, 0.22f, 0.18f));
        public static Sprite BackgroundMenu => SpriteLoader.Load("BackgroundMenu", new Color(0.09f, 0.08f, 0.11f));
        public static Sprite BackgroundPause => SpriteLoader.Load("BackgroundPause", new Color(0.12f, 0.1f, 0.14f));
        public static Sprite BackgroundQte => SpriteLoader.Load("BackgroungQTE", new Color(0.16f, 0.14f, 0.2f));
        public static Sprite BackgroundChest => SpriteLoader.Load("BackgroundChestOpened", new Color(0.36f, 0.22f, 0.12f));

        public static Sprite Chest => SpriteLoader.Load("Chest", new Color(0.45f, 0.28f, 0.14f));
        public static Sprite Cauldron => SpriteLoader.Load("Cauldron", new Color(0.22f, 0.28f, 0.34f));
        public static Sprite Bed => SpriteLoader.Load("Bed", new Color(0.55f, 0.35f, 0.4f));
        public static Sprite Table => SpriteLoader.Load("Table", new Color(0.4f, 0.28f, 0.16f));
        public static Sprite StartButton => SpriteLoader.Load("Start", Color.white);
        public static Sprite ExitButton => SpriteLoader.Load("Exit", Color.white);
        public static Sprite MenuButton => SpriteLoader.Load("Menu", Color.white);
        public static Sprite CloseButton => SpriteLoader.Load("Close", Color.white);
        public static Sprite ArrowRight => SpriteLoader.Load("ArrowRight", Color.white);
        public static Sprite PotionIcon => SpriteLoader.Load("Plate", new Color(0.55f, 0.2f, 0.7f));

        public static Sprite[] CauldronBurnFrames => new[]
        {
            SpriteLoader.Load("CauldronBurn1"),
            SpriteLoader.Load("CauldronBurn2"),
            SpriteLoader.Load("CauldronBurn3")
        };
    }
}