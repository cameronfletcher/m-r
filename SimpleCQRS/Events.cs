using System;
namespace SimpleCQRS
{
    public class InventoryItemDeactivated
    {
        public Guid Id { get; set; }
    }

    public class InventoryItemCreated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class InventoryItemRenamed
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }

    public class ItemsCheckedInToInventory
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
    }

    public class ItemsRemovedFromInventory
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
    }
}

