using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petrol_Station.Infrastructure;
using Petrol_Station.Models;
using Petrol_Station.Services;
using Petrol_Station.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.Controllers
{
    public class ContainersController : Controller
    {
        Petrol_StationContext context;
        private readonly CacheProvider cache;
        private const string filterKey = "pricePerLiter";
        public ContainersController(Petrol_StationContext context, CacheProvider cacheProvider)
        {
            this.context = context;
            cache = cacheProvider;

        }

        public IActionResult Index(SortState sortState = SortState.NumberAsc, int page = 1)
        {
            ContainersFilterViewModel filter = HttpContext.Session.Get<ContainersFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new ContainersFilterViewModel { Number = 0, TankCapacity = 0 };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Containers).Name}-{page}-{sortState}-{filter.Number}-{filter.TankCapacity}";
            if (!cache.TryGetValue(modelKey, out ContainersViewModel model))
            {
                model = new ContainersViewModel();

                IQueryable<Containers> containers = GetSortedEntities(sortState, filter.Number, filter.TankCapacity);

                int count = containers.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(count, page, pageSize);

                model.Containers = count == 0 ? new List<Containers>() : containers.Skip((model.PageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.ContainersFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ContainersFilterViewModel filterModel, int page)
        {
            ContainersFilterViewModel filter = HttpContext.Session.Get<ContainersFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.Number = filterModel.Number;
                filter.TankCapacity = filterModel.TankCapacity;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        {
            ContainersViewModel costsViewModel = new ContainersViewModel();
            costsViewModel.PageViewModel = new PageViewModel { PageNumber = page };

            return View(costsViewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ContainersViewModel costsViewModel)
        {
            if (ModelState.IsValid)
            {
                await context.Containers.AddAsync(costsViewModel.Container);
                await context.SaveChangesAsync();
                cache.Clean();
                return RedirectToAction("Index", "Containers");
            }

            return View(costsViewModel);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Containers containers = await context.Containers.FindAsync(id);
            if (containers != null)
            {
                ContainersViewModel costsViewModel = new ContainersViewModel();
                costsViewModel.PageViewModel = new PageViewModel { PageNumber = page };
                costsViewModel.Container = containers;

                return View(costsViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ContainersViewModel costsViewModel)
        {
            if (ModelState.IsValid)
            {
                Containers containers = context.Containers.Find(costsViewModel.Container.ContainerId);
                if (containers != null)
                {
                    containers.Number = costsViewModel.Container.Number;
                    containers.TankCapacity = costsViewModel.Container.TankCapacity;

                    context.Containers.Update(containers);
                    await context.SaveChangesAsync();
                    cache.Clean();
                    return RedirectToAction("Index", "Containers", new { page = costsViewModel.PageViewModel.PageNumber });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(costsViewModel);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            Containers containers = await context.Containers.FindAsync(id);
            if (containers == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (context.Containers.Any(s => s.TypeOfGsmid == containers.ContainerId) && context.Containers.Any(s => s.TypeOfGsmid == containers.ContainerId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            ContainersViewModel costsViewModel = new ContainersViewModel();
            costsViewModel.Container = containers;
            costsViewModel.PageViewModel = new PageViewModel { PageNumber = page };
            costsViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };

            return View(costsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ContainersViewModel costsViewModel)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";
            Containers containers = await context.Containers.FindAsync(costsViewModel.Container.ContainerId);
            costsViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };
            if (containers == null)
                return NotFound();

            context.Containers.Remove(containers);
            await context.SaveChangesAsync();
            cache.Clean();
            return View(costsViewModel);
        }

        private IQueryable<Containers> GetSortedEntities(SortState sortState, int number, double tankCapacity)
        {
            IQueryable<Containers> containers = context.Containers.AsQueryable();

            switch (sortState)
            {
                case SortState.NumberAsc:
                    containers = containers.OrderBy(g => g.Number);
                    break;
                case SortState.NumberDesc:
                    containers = containers.OrderByDescending(g => g.Number);
                    break;
                case SortState.TankCapacityAsc:
                    containers = containers.OrderBy(g => g.TankCapacity);
                    break;
                case SortState.TankCapacityDesc:
                    containers = containers.OrderByDescending(g => g.TankCapacity);
                    break;
            }

            if (number != 0)
                containers = containers.Where(g => g.Number == number).AsQueryable();
            if (tankCapacity != 0)
                containers = containers.Where(g => g.TankCapacity == tankCapacity).AsQueryable();

            return containers;
        }
    }
}
