using Petrol_Station.Infrastructure;
using Petrol_Station.Models;
using Petrol_Station.Services;
using Petrol_Station.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.Controllers
{
    public class GsmController : Controller
    {
        Petrol_StationContext context;
        private readonly CacheProvider cache;
        private const string filterKey = "gsm";
        public GsmController(Petrol_StationContext context, CacheProvider cacheProvider)
        {
            this.context = context;
            cache = cacheProvider;

        }

        public IActionResult Index(SortState sortState = SortState.TypeOfGsmAsc, int page = 1)
        {
            GsmFilterViewModel filter = HttpContext.Session.Get<GsmFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new GsmFilterViewModel { TypeOfGsm = string.Empty, TTkofType = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(Gsm).Name}-{page}-{sortState}-{filter.TypeOfGsm}-{filter.TTkofType}";
            if (!cache.TryGetValue(modelKey, out GsmViewModel model))
            {
                model = new GsmViewModel();

                IQueryable<Gsm> gsm = GetSortedEntities(sortState, filter.TypeOfGsm, filter.TTkofType);

                int count = gsm.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(count, page, pageSize);

                model.Gsms = count == 0 ? new List<Gsm>() : gsm.Skip((model.PageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.GsmFilterViewModel = filter;

                cache.Set(modelKey, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(GsmFilterViewModel filterModel, int page)
        {
            GsmFilterViewModel filter = HttpContext.Session.Get<GsmFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.TypeOfGsm = filterModel.TypeOfGsm;
                filter.TTkofType = filterModel.TTkofType;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        [Authorize(Roles ="admin")]
        public IActionResult Create(int page)
        {
            GsmViewModel gsmViewModel = new GsmViewModel();
            gsmViewModel.PageViewModel = new PageViewModel { PageNumber = page };

            return View(gsmViewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(GsmViewModel gsmViewModel)
        {
            if (ModelState.IsValid)
            {
                await context.Gsm.AddAsync(gsmViewModel.Gsm);
                await context.SaveChangesAsync();
                cache.Clean();
                return RedirectToAction("Index", "Gsm");
            }

            return View(gsmViewModel);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Gsm gsm = await context.Gsm.FindAsync(id);
            if (gsm != null)
            {
                GsmViewModel gsmViewModel = new GsmViewModel();
                gsmViewModel.PageViewModel = new PageViewModel { PageNumber = page };
                gsmViewModel.Gsm = gsm;

                return View(gsmViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GsmViewModel gsmViewModel)
        {
            if (ModelState.IsValid)
            {
                Gsm gsm = context.Gsm.Find(gsmViewModel.Gsm.GSmid);
                if (gsm != null)
                {
                    gsm.TypeOfGsm = gsmViewModel.Gsm.TypeOfGsm;
                    gsm.TTkofType = gsmViewModel.Gsm.TTkofType;

                    context.Gsm.Update(gsm);
                    await context.SaveChangesAsync();
                    cache.Clean();
                    return RedirectToAction("Index", "Gsm", new { page = gsmViewModel.PageViewModel.PageNumber });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(gsmViewModel);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            Gsm gsm = await context.Gsm.FindAsync(id);
            if (gsm == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (context.Containers.Any(s => s.TypeOfGsmid == gsm.GSmid) && context.Costs.Any(s => s.TypeOfGsmid == gsm.GSmid))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            GsmViewModel gsmViewModel = new GsmViewModel();
            gsmViewModel.Gsm = gsm;
            gsmViewModel.PageViewModel = new PageViewModel { PageNumber = page };
            gsmViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };
            
            return View(gsmViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(GsmViewModel gsmViewModel)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";
            Gsm gsm = await context.Gsm.FindAsync(gsmViewModel.Gsm.GSmid);
            gsmViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };
            if (gsm == null)
                return NotFound();

            context.Gsm.Remove(gsm);
            await context.SaveChangesAsync();
            cache.Clean();
            return View(gsmViewModel);
        }

        private IQueryable<Gsm> GetSortedEntities(SortState sortState, string typeOfGsm, string tTkofType)
        {
            IQueryable<Gsm> gsm = context.Gsm.AsQueryable();

            switch (sortState)
            {
                case SortState.TypeOfGsmAsc:
                    gsm = gsm.OrderBy(g => g.TypeOfGsm);
                    break;
                case SortState.TypeOfGsmDesc:
                    gsm = gsm.OrderByDescending(g => g.TypeOfGsm);
                    break;
                case SortState.TTkofTypeAsc:
                    gsm = gsm.OrderBy(g => g.TTkofType);
                    break;
                case SortState.TTkofTypeDesc:
                    gsm = gsm.OrderByDescending(g => g.TTkofType);
                    break;
            }

            if (!string.IsNullOrEmpty(typeOfGsm))
                gsm = gsm.Where(g => g.TypeOfGsm.Contains(typeOfGsm)).AsQueryable();
            if (!string.IsNullOrEmpty(tTkofType))
                gsm = gsm.Where(g => g.TTkofType.Contains(tTkofType)).AsQueryable();

            return gsm;
        }
    }
}
