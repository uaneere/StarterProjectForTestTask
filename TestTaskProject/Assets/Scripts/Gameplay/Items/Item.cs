using UnityEngine;

namespace Gameplay.Items
{
    public abstract class Item
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public Sprite Icon { get; }

        protected Item(string id, string name, string description, Sprite icon = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
        }
    }
}