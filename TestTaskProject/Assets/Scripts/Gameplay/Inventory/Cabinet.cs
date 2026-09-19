using Gameplay.Items;

namespace Gameplay.Inventory
{
    public class Cabinet
    {
        public const int Columns = 7;
        public const int Rows = 4;
        public const int SlotCount = Columns * Rows;

        private readonly Item[] _slots = new Item[SlotCount];

        public Item GetSlot(int index)
        {
            return IsValidIndex(index) ? _slots[index] : null;
        }

        public void SetSlot(int index, Item item)
        {
            if (!IsValidIndex(index))
                return;

            _slots[index] = item;
        }

        public bool TryTake(int index, HandInventory hands)
        {
            if (!IsValidIndex(index))
                return false;

            var item = _slots[index];
            if (item == null || !hands.TryTake(item))
                return false;

            _slots[index] = null;
            return true;
        }

        public bool TryStoreFromHands(int index, HandInventory hands)
        {
            if (!IsValidIndex(index) || _slots[index] != null || hands.IsEmpty)
                return false;

            _slots[index] = hands.Remove();
            return true;
        }

        public void Fill(Item[] items)
        {
            for (var i = 0; i < SlotCount; i++)
                _slots[i] = items != null && i < items.Length ? items[i] : null;
        }

        private static bool IsValidIndex(int index)
        {
            return index >= 0 && index < SlotCount;
        }
    }
}