using Petrol_Station.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class IncomeAndExpensesOfGsmViewModel
    {
        public IEnumerable<IncomeAndExpensesOfGsm> IncomeAndExpensesOfGsms { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public IncomeAndExpensesOfGsm IncomeAndExpensesOfGsm { get; set; }
        public DeleteViewModels DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public IncomeAndExpensesOfGsmFilterViewModel IncomeAndExpensesOfGsmFilterViewModel { get; set; }
        public IEnumerable<Staff> StaffList { get; set; }
        public IEnumerable<Containers> ContainerList { get; set; }
        public string StaffName { get; set; }
        public int ContainerNumber { get; set; }
        
    }
}
