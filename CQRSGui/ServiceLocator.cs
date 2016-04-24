using System;
using dddlib.Projections.Memory;
using SimpleCQRS;

namespace CQRSGui
{
    internal static class ServiceLocator
    {
        public static Microbus Bus { get; set; }

        public static MemoryRepository<Guid, InventoryItemDetailsDto> DetailsRepository { get; set; }

        public static MemoryRepository<Guid, InventoryItemListDto> ListRepository { get; set; }
    }
}