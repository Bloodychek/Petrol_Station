using System;
using System.Collections.Generic;

namespace Petrol_Station.Models
{
    public partial class IncomeAndExpensesOfGsm
    {
        public int IncomeAndExpenseOfGsmid { get; set; }
        public int? NumberOfCapacity { get; set; }
        public int? ContainerId { get; set; }
        public int? IncomeOrExpensePerliter { get; set; }
        public DateTime DateAndTimeOfTheOperationIncomeOrExpense { get; set; }
        public int? StaffId { get; set; }
        public string ResponsibleForTheOperation { get; set; }

        public virtual Containers Container { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
