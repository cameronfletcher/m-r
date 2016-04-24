using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using dddlib.Persistence.EventDispatcher.SqlServer;
using dddlib.Persistence.SqlServer;
using dddlib.Projections.Memory;
using SimpleCQRS;

namespace CQRSGui
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            var bus = new Microbus();

            var connectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=mr;Integrated Security=True;Connect Timeout=30;";
            var rep = new SqlServerEventStoreRepository(connectionString);
            var commands = new InventoryCommandHandlers(rep);

            var detailRepository = new MemoryRepository<Guid, InventoryItemDetailsDto>();
            var listRepository = new MemoryRepository<Guid, InventoryItemListDto>();

            var detail = new InventoryItemDetailView(detailRepository);
            var list = new InventoryListView(listRepository);

            bus.AutoRegister(commands, detail, list);

            var dispatcher = new SqlServerEventDispatcher(connectionString, (sequenceNumber, @event) => bus.Send(@event), Guid.NewGuid());

            ServiceLocator.Bus = bus;
            ServiceLocator.DetailsRepository = detailRepository;
            ServiceLocator.ListRepository = listRepository;
        }
    }
}