using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System.Linq;

namespace SportsStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IStoreRepository Repository;
        public NavigationMenuViewComponent(IStoreRepository repo)
        {
            Repository = repo;
        }
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData.Values["category"];
            return View(Repository.Products.Select(c => c.Category).Distinct().OrderBy(c => c));
        }
    }
}