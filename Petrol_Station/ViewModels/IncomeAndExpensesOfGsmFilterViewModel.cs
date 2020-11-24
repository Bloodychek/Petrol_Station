using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class IncomeAndExpensesOfGsmFilterViewModel
    {
        [Display(Name = "NumberOfCapacity")]
        public int NumberOfCapacity { get; set; }

        [Display(Name = "IncomeOrExpensePerliter")]
        public int IncomeOrExpensePerliter { get; set; }

        [Display(Name = "DateAndTimeOfTheOperationIncomeOrExpense")]
        public DateTime DateAndTimeOfTheOperationIncomeOrExpense { get; set; }

        [Display(Name = "ResponsibleForTheOperation")]
        public string ResponsibleForTheOperation { get; set; }
        public string StaffName { get; set; }
    }
}
