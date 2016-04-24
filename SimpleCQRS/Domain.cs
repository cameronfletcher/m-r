using System;
using dddlib;

namespace SimpleCQRS
{
    public class InventoryItem : AggregateRoot
    {
        private Guid _id;

        private void Handle(InventoryItemCreated e)
        {
            _id = e.Id;
        }

        private void Handle(InventoryItemDeactivated e)
        {
            EndLifecycle();
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) throw new ArgumentException("newName");
            Apply(new InventoryItemRenamed { Id = _id, NewName = newName });
        }

        public void Remove(int count)
        {
            if (count <= 0) throw new BusinessException("cant remove negative count from inventory");
            Apply(new ItemsRemovedFromInventory { Id = _id, Count = count });
        }


        public void CheckIn(int count)
        {
            if(count <= 0) throw new BusinessException("must have a count greater than 0 to add to inventory");
            Apply(new ItemsCheckedInToInventory { Id = _id, Count = count });
        }

        public void Deactivate()
        {
            Apply(new InventoryItemDeactivated { Id = _id });
        }

        [NaturalKey]
        public Guid Id
        {
            get { return _id; }
        }

        protected internal InventoryItem()
        {
            // used to create in repository ... many ways to avoid this, eg making private constructor
        }

        public InventoryItem(Guid id, string name)
        {
            Apply(new InventoryItemCreated { Id = id, Name = name });
        }
    }
}
