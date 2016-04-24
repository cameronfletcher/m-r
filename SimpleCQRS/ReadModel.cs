using System;
using System.Collections.Generic;
using dddlib.Projections;

namespace SimpleCQRS
{
    public interface IReadModelFacade
    {
        IEnumerable<InventoryItemListDto> GetInventoryItems();
        InventoryItemDetailsDto GetInventoryItemDetails(Guid id);
    }

    public class InventoryItemDetailsDto
    {
        public Guid Id;
        public string Name;
        public int CurrentCount;
        public int Version;

        public InventoryItemDetailsDto(Guid id, string name, int currentCount, int version)
        {
            Id = id;
            Name = name;
            CurrentCount = currentCount;
            Version = version;
        }
    }

    public class InventoryItemListDto
    {
        public Guid Id;
        public string Name;

        public InventoryItemListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class InventoryListView
    {
        private readonly IRepository<Guid, InventoryItemListDto> repository;

        public InventoryListView(IRepository<Guid, InventoryItemListDto> repository)
        {
            this.repository = repository;
        }

        public void Handle(InventoryItemCreated message)
        {
            repository.AddOrUpdate(message.Id, new InventoryItemListDto(message.Id, message.Name));
        }

        public void Handle(InventoryItemRenamed message)
        {
            var item = repository.Get(message.Id);
            item.Name = message.NewName;
            repository.AddOrUpdate(message.Id, item);
        }

        public void Handle(InventoryItemDeactivated message)
        {
            repository.Remove(message.Id);
        }
    }

    public class InventoryItemDetailView
    {
        private readonly IRepository<Guid, InventoryItemDetailsDto> repository;

        public InventoryItemDetailView(IRepository<Guid, InventoryItemDetailsDto> repository)
        {
            this.repository = repository;
        }

        public void Handle(InventoryItemCreated message)
        {
            repository.AddOrUpdate(message.Id, new InventoryItemDetailsDto(message.Id, message.Name, 0,0));
        }

        public void Handle(InventoryItemRenamed message)
        {
            var d = repository.Get(message.Id);
            d.Name = message.NewName;
            repository.AddOrUpdate(message.Id, d);
        }

        public void Handle(ItemsRemovedFromInventory message)
        {
            var d = repository.Get(message.Id);
            d.CurrentCount -= message.Count;
            repository.AddOrUpdate(message.Id, d);
        }

        public void Handle(ItemsCheckedInToInventory message)
        {
            var d = repository.Get(message.Id);
            d.CurrentCount += message.Count;
            repository.AddOrUpdate(message.Id, d);
        }

        public void Handle(InventoryItemDeactivated message)
        {
            repository.Remove(message.Id);
        }
    }
}
