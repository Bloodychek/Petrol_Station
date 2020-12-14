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
    public class CostsController : Controller
    {
        Petrol_StationContext context;
        private readonly CacheProvider cache;
        private const string filterKey = "pricePerLiter";
        public CostsController(Petrol_StationContext context, CacheProvider cacheProvider)
        {
            this.context = context;
            cache = cacheProvider;

        }

        public IActionResult Index(SortState sortState = SortState.TypeOfGsmAsc, int page = 1)
        {
            CostsFilterViewModel filter = HttpContext.Session.Get<CostsFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new CostsFilterViewModel { PricePerLiter = 0, DateOfCostGsm = default };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Costs).Name}-{page}-{sortState}-{filter.PricePerLiter}-{filter.DateOfCostGsm}";
            if (!cache.TryGetValue(modelKey, out CostsViewModel model))
            {
                model = new CostsViewModel();

                IQueryable<Costs> costs = GetSortedEntities(sortState, filter.PricePerLiter, filter.DateOfCostGsm);

                int count = costs.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(count, page, pageSize);

                model.Costs = count == 0 ? new List<Costs>() : costs.Skip((model.PageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.CostsFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CostsFilterViewModel filterModel, int page)
        {
            CostsFilterViewModel filter = HttpContext.Session.Get<CostsFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.PricePerLiter = filterModel.PricePerLiter;
                filter.DateOfCostGsm = filterModel.DateOfCostGsm;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        {
            CostsViewModel costsViewModel = new CostsViewModel();
            costsViewModel.PageViewModel = new PageViewModel { PageNumber = page };

            return View(costsViewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CostsViewModel costsViewModel)
        {
            if (ModelState.IsValid)
            {
                await context.Costs.AddAsync(costsViewModel.Cost);
                await context.SaveChangesAsync();
                cache.Clean();
                return RedirectToAction("Index", "Costs");
            }

            return View(costsViewModel);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Costs costs = await context.Costs.FindAsync(id);
            if (costs != null)
            {
                CostsViewModel costsViewModel = new CostsViewModel();
                costsViewModel.PageViewModel = new PageViewModel { PageNumber = page };
                costsViewModel.Cost = costs;

                return View(costsViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CostsViewModel costsViewModel)
        {
            if (ModelState.IsValid)
            {
                Costs costs = context.Costs.Find(costsViewModel.Cost.CostId);
                if (costs != null)
                {
                    costs.PricePerLiter = costsViewModel.Cost.PricePerLiter;
                    costs.DateOfCostGsm = costsViewModel.Cost.DateOfCostGsm;

                    context.Costs.Update(costs);
                    await context.SaveChangesAsync();
                    cache.Clean();
                    return RedirectToAction("Index", "Costs", new { page = costsViewModel.PageViewModel.PageNumber });
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
            Costs costs = await context.Costs.FindAsync(id);
            if (costs == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (context.Containers.Any(s => s.TypeOfGsmid == costs.CostId) && context.Costs.Any(s => s.TypeOfGsmid == costs.CostId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            CostsViewModel costsViewModel = new CostsViewModel();
            costsViewModel.Cost = costs;
            costsViewModel.PageViewModel = new PageViewModel { PageNumber = page };
            costsViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };

            return View(costsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CostsViewModel costsViewModel)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";
            Costs costs = await context.Costs.FindAsync(costsViewModel.Cost.CostId);
            costsViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };
            if (costs == null)
                return NotFound();

            context.Costs.Remove(costs);
            await context.SaveChangesAsync();
            cache.Clean();
            return View(costsViewModel);
        }

        private IQueryable<Costs> GetSortedEntities(SortState sortState, double pricePerLiter, DateTime dateOfCostGsm)
        {
            IQueryable<Costs> costs = context.Costs.AsQueryable();

            switch (sortState)
            {
                case SortState.PricePerLiterAsc:
                    costs = costs.OrderBy(g => g.PricePerLiter);
                    break;
                case SortState.PricePerLiterDesc:
                    costs = costs.OrderByDescending(g => g.PricePerLiter);
                    break;
                case SortState.DateOfCostGsmAsc:
                    costs = costs.OrderBy(g => g.DateOfCostGsm);
                    break;
                case SortState.DateOfCostGsmDesc:
                    costs = costs.OrderByDescending(g => g.DateOfCostGsm);
                    break;
            }

            if (pricePerLiter != 0)
                costs = costs.Where(g => g.PricePerLiter == pricePerLiter).AsQueryable();
            if (dateOfCostGsm != default)
                costs = costs.Where(g => g.DateOfCostGsm.Date == dateOfCostGsm.Date).AsQueryable();

            return costs;
        }
    }
}
