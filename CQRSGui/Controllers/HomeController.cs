using System;
using System.Linq;
using System.Web.Mvc;
using dddlib.Projections;
using dddlib.Projections.Memory;
using SimpleCQRS;

namespace CQRSGui.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private Microbus _bus;

        private readonly MemoryRepository<Guid, InventoryItemDetailsDto> detailsRepository;
        private readonly MemoryRepository<Guid, InventoryItemListDto> listRepository;

        public HomeController()
        {
            _bus = ServiceLocator.Bus;
            detailsRepository = ServiceLocator.DetailsRepository;
            listRepository = ServiceLocator.ListRepository;
        }

        public ActionResult Index()
        {
            ViewData.Model = listRepository.GetAll().Select(kvp => kvp.Value);

            return View();
        }

        public ActionResult Details(Guid id)
        {
            ViewData.Model = detailsRepository.Get(id);
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            _bus.Send(new CreateInventoryItem(Guid.NewGuid(), name));

            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            ViewData.Model = detailsRepository.Get(id);
            return View();
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string name, int version)
        {
            var command = new RenameInventoryItem(id, name, version);
            _bus.Send(command);

            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id)
        {
            ViewData.Model = detailsRepository.Get(id);
            return View();
        }

        [HttpPost]
        public ActionResult Deactivate(Guid id, int version)
        {
            _bus.Send(new DeactivateInventoryItem(id, version));
            return RedirectToAction("Index");
        }

        public ActionResult CheckIn(Guid id)
        {
            ViewData.Model = detailsRepository.Get(id);
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(Guid id, int number, int version)
        {
            _bus.Send(new CheckInItemsToInventory(id, number, version));
            return RedirectToAction("Index");
        }

        public ActionResult Remove(Guid id)
        {
            ViewData.Model = detailsRepository.Get(id);
            return View();
        }

        [HttpPost]
        public ActionResult Remove(Guid id, int number, int version)
        {
            _bus.Send(new RemoveItemsFromInventory(id, number, version));
            return RedirectToAction("Index");
        }
    }
}
