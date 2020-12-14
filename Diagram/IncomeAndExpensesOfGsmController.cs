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
    public class IncomeAndExpensesOfGsmController : Controller
    {
        Petrol_StationContext context;
        private readonly CacheProvider cache;
        private const string filterKey = "numberOfCapacity";
        public IncomeAndExpensesOfGsmController(Petrol_StationContext context, CacheProvider cacheProvider)
        {
            this.context = context;
            cache = cacheProvider;

        }

        public IActionResult Index(SortState sortState = SortState.NumberOfCapacityAsc,  int page = 1)
        {
            IncomeAndExpensesOfGsmFilterViewModel filter = HttpContext.Session.Get<IncomeAndExpensesOfGsmFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new IncomeAndExpensesOfGsmFilterViewModel { NumberOfCapacity = 0, IncomeOrExpensePerliter = 0, DateAndTimeOfTheOperationIncomeOrExpense = default, ResponsibleForTheOperation = string.Empty, StaffName = string.Empty };
                HttpContext.Session.Set(filterKey, filter);
            }

            string modelKey = $"{typeof(IncomeAndExpensesOfGsm).Name}-{page}-{sortState}-{filter.NumberOfCapacity}-{filter.IncomeOrExpensePerliter}-{filter.DateAndTimeOfTheOperationIncomeOrExpense}-{filter.ResponsibleForTheOperation}-{filter.StaffName}";
            if (!cache.TryGetValue(modelKey, out IncomeAndExpensesOfGsmViewModel model))
            {
                model = new IncomeAndExpensesOfGsmViewModel();

                IQueryable<IncomeAndExpensesOfGsm> incomeAndExpensesOfGsm = GetSortedEntities(sortState, filter.NumberOfCapacity, filter.IncomeOrExpensePerliter, filter.DateAndTimeOfTheOperationIncomeOrExpense, filter.ResponsibleForTheOperation, filter.StaffName);

                int count = incomeAndExpensesOfGsm.Count();
                int pageSize = 10;
                model.PageViewModel = new PageViewModel(count, page, pageSize);

                model.IncomeAndExpensesOfGsms = count == 0 ? new List<IncomeAndExpensesOfGsm>() : incomeAndExpensesOfGsm.Skip((model.PageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.IncomeAndExpensesOfGsmFilterViewModel = filter;

                cache.Set(modelKey, model);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IncomeAndExpensesOfGsmFilterViewModel filterModel, int page)
        {
            IncomeAndExpensesOfGsmFilterViewModel filter = HttpContext.Session.Get<IncomeAndExpensesOfGsmFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.NumberOfCapacity = filterModel.NumberOfCapacity;
                filter.IncomeOrExpensePerliter = filterModel.IncomeOrExpensePerliter;
                filter.DateAndTimeOfTheOperationIncomeOrExpense = filterModel.DateAndTimeOfTheOperationIncomeOrExpense;
                filter.ResponsibleForTheOperation = filterModel.ResponsibleForTheOperation;
                filter.StaffName = filterModel.StaffName;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        public IActionResult SumIncomeAndExpence(DateTime d1, DateTime d2, int page = 1)
        {
           Dictionary<string, int> keyValuePairs = context.IncomeAndExpensesOfGsm.Include(a => a.Container).Include(r => r.Container.TypeOfGsm).
                Where(i => i.DateAndTimeOfTheOperationIncomeOrExpense > d1 && i.DateAndTimeOfTheOperationIncomeOrExpense < d2).ToList().GroupBy(g => g.Container.TypeOfGsm.TypeOfGsm).ToDictionary(h => h.Key, h => h.Sum(j => j.IncomeOrExpensePerliter.Value));

            GroupViewModel model = new GroupViewModel();
            int count = keyValuePairs.Count();
            int pageSize = 10;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.KeyValuePairs = count == 0 ? new Dictionary<string, int>() : keyValuePairs.Skip((model.PageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToDictionary(p => p.Key, p => p.Value);
            model.d1 = d1;
            model.d2 = d2;
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create(int page)
        {
            IncomeAndExpensesOfGsmViewModel IncomeAndExpensesOfGsmViewModel = new IncomeAndExpensesOfGsmViewModel();
            IncomeAndExpensesOfGsmViewModel.PageViewModel = new PageViewModel { PageNumber = page };
            IncomeAndExpensesOfGsmViewModel.StaffList = context.Staff.ToList();
            IncomeAndExpensesOfGsmViewModel.ContainerList = context.Containers.ToList();
            return View(IncomeAndExpensesOfGsmViewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(IncomeAndExpensesOfGsmViewModel incomeAndExpensesOfGsmViewModel)
        {
            incomeAndExpensesOfGsmViewModel.StaffList = context.Staff.ToList();
            incomeAndExpensesOfGsmViewModel.ContainerList = context.Containers.ToList();

            if (ModelState.IsValid)
            {
                Staff staff = context.Staff.FirstOrDefault(a => a.FullName == incomeAndExpensesOfGsmViewModel.StaffName);
                if(staff == null)
                {
                    return NotFound();
                }
                Models.Containers containers = context.Containers.FirstOrDefault(a => a.Number == incomeAndExpensesOfGsmViewModel.ContainerNumber);
                if(containers == null)
                {
                    return NotFound();
                }
                incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.StaffId = staff.StaffId;
                incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.ContainerId = containers.ContainerId;
                await context.IncomeAndExpensesOfGsm.AddAsync(incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm);
                await context.SaveChangesAsync();
                cache.Clean();
                return RedirectToAction("Index", "IncomeAndExpensesOfGsm");
            }

            return View(incomeAndExpensesOfGsmViewModel);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            IncomeAndExpensesOfGsm incomeAndExpensesOfGsm = await context.IncomeAndExpensesOfGsm.FindAsync(id);
            if (incomeAndExpensesOfGsm != null)
            {
                IncomeAndExpensesOfGsmViewModel IncomeAndExpensesOfGsmViewModel = new IncomeAndExpensesOfGsmViewModel();
                IncomeAndExpensesOfGsmViewModel.PageViewModel = new PageViewModel { PageNumber = page };
                IncomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm = incomeAndExpensesOfGsm;

                IncomeAndExpensesOfGsmViewModel.StaffList = context.Staff.ToList();
                IncomeAndExpensesOfGsmViewModel.ContainerList = context.Containers.ToList();
                return View(IncomeAndExpensesOfGsmViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IncomeAndExpensesOfGsmViewModel incomeAndExpensesOfGsmViewModel)
        {
            incomeAndExpensesOfGsmViewModel.StaffList = context.Staff.ToList();
            incomeAndExpensesOfGsmViewModel.ContainerList = context.Containers.ToList();

            if (ModelState.IsValid)
            {
                IncomeAndExpensesOfGsm incomeAndExpensesOfGsm = context.IncomeAndExpensesOfGsm.Find(incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.IncomeAndExpenseOfGsmid);
                if (incomeAndExpensesOfGsm != null)
                {
                    Staff staff = context.Staff.FirstOrDefault(a => a.FullName == incomeAndExpensesOfGsmViewModel.StaffName);
                    if (staff == null)
                    {
                        return NotFound();
                    }
                    Models.Containers containers = context.Containers.FirstOrDefault(a => a.Number == incomeAndExpensesOfGsmViewModel.ContainerNumber);
                    if (containers == null)
                    {
                        return NotFound();
                    }
                    incomeAndExpensesOfGsm.StaffId = staff.StaffId;
                    incomeAndExpensesOfGsm.ContainerId = containers.ContainerId;


                    incomeAndExpensesOfGsm.NumberOfCapacity = incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.NumberOfCapacity;
                    incomeAndExpensesOfGsm.IncomeOrExpensePerliter = incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.IncomeOrExpensePerliter;
                    incomeAndExpensesOfGsm.DateAndTimeOfTheOperationIncomeOrExpense = incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.DateAndTimeOfTheOperationIncomeOrExpense;
                    incomeAndExpensesOfGsm.ResponsibleForTheOperation = incomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.ResponsibleForTheOperation;

                    context.IncomeAndExpensesOfGsm.Update(incomeAndExpensesOfGsm);
                    await context.SaveChangesAsync();
                    cache.Clean();
                    return RedirectToAction("Index", "IncomeAndExpensesOfGsm", new { page = incomeAndExpensesOfGsmViewModel.PageViewModel.PageNumber });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(incomeAndExpensesOfGsmViewModel);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            IncomeAndExpensesOfGsm incomeAndExpensesOfGsm = await context.IncomeAndExpensesOfGsm.FindAsync(id);
            if (incomeAndExpensesOfGsm == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            IncomeAndExpensesOfGsmViewModel IncomeAndExpensesOfGsmViewModel = new IncomeAndExpensesOfGsmViewModel();
            IncomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm = incomeAndExpensesOfGsm;
            IncomeAndExpensesOfGsmViewModel.PageViewModel = new PageViewModel { PageNumber = page };
            IncomeAndExpensesOfGsmViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };

            return View(IncomeAndExpensesOfGsmViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(IncomeAndExpensesOfGsmViewModel IncomeAndExpensesOfGsmViewModel)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";
            IncomeAndExpensesOfGsm incomeAndExpensesOfGsm = await context.IncomeAndExpensesOfGsm.FindAsync(IncomeAndExpensesOfGsmViewModel.IncomeAndExpensesOfGsm.IncomeAndExpenseOfGsmid);
            IncomeAndExpensesOfGsmViewModel.DeleteViewModel = new DeleteViewModels { Message = message, IsDeleted = deleteFlag };
            if (incomeAndExpensesOfGsm == null)
                return NotFound();

            context.IncomeAndExpensesOfGsm.Remove(incomeAndExpensesOfGsm);
            await context.SaveChangesAsync();
            cache.Clean();
            return View(IncomeAndExpensesOfGsmViewModel);
        }

        private IQueryable<IncomeAndExpensesOfGsm> GetSortedEntities(SortState sortState, int numberOfCapacity, int incomeOrExpensePerliter, DateTime dateAndTimeOfTheOperationIncomeOrExpense, string responsibleForTheOperation, string staffName)
        {
            IQueryable<IncomeAndExpensesOfGsm> incomeAndExpensesOfGsm = context.IncomeAndExpensesOfGsm.Include(a => a.Container).Include(g => g.Staff).Include(r => r.Container.TypeOfGsm).AsQueryable();
            switch (sortState)
            {
                case SortState.NumberOfCapacityAsc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderBy(g => g.NumberOfCapacity);
                    break;
                case SortState.NumberOfCapacityDesc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderByDescending(g => g.NumberOfCapacity);
                    break;
                case SortState.IncomeOrExpensePerliterAsc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderBy(g => g.IncomeOrExpensePerliter);
                    break;
                case SortState.IncomeOrExpensePerliterDesc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderByDescending(g => g.IncomeOrExpensePerliter);
                    break;
                case SortState.DateAndTimeOfTheOperationIncomeOrExpenseAsc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderBy(g => g.DateAndTimeOfTheOperationIncomeOrExpense);
                    break;
                case SortState.DateAndTimeOfTheOperationIncomeOrExpenseDesc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderByDescending(g => g.DateAndTimeOfTheOperationIncomeOrExpense);
                    break;
                case SortState.ResponsibleForTheOperationAsc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderBy(g => g.ResponsibleForTheOperation);
                    break;
                case SortState.ResponsibleForTheOperationDesc:
                    incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.OrderByDescending(g => g.ResponsibleForTheOperation);
                    break;
            }

            if (numberOfCapacity != 0)
                incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.Where(g => g.NumberOfCapacity == numberOfCapacity).AsQueryable();
            if (incomeOrExpensePerliter != 0)
                incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.Where(g => g.IncomeOrExpensePerliter == incomeOrExpensePerliter).AsQueryable();
            if (dateAndTimeOfTheOperationIncomeOrExpense != default)
                incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.Where(g => g.DateAndTimeOfTheOperationIncomeOrExpense.Date == dateAndTimeOfTheOperationIncomeOrExpense.Date).AsQueryable();
            if (!string.IsNullOrEmpty(responsibleForTheOperation))
                incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.Where(g => g.ResponsibleForTheOperation.Contains(responsibleForTheOperation)).AsQueryable();
            if (!string.IsNullOrEmpty(staffName))
                incomeAndExpensesOfGsm = incomeAndExpensesOfGsm.Include(r => r.Staff).Where(g => g.Staff.FullName.Contains(staffName)).AsQueryable();


            return incomeAndExpensesOfGsm;
        }
    }
}
