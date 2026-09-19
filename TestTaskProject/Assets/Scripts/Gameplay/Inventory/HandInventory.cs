using Gameplay.Items;

namespace Gameplay.Inventory
{
    public class HandInventory
    {
        public Item HeldItem { get; private set; }
        public bool IsEmpty => HeldItem == null;

        public bool TryTake(Item item)
        {
            if (item == null || HeldItem != null)
                return false;

            HeldItem = item;
            return true;
        }

        public Item Remove()
        {
            var item = HeldItem;
            HeldItem = null;
            return item;
        }

        public void Clear()
        {
            HeldItem = null;
        }
    }
}