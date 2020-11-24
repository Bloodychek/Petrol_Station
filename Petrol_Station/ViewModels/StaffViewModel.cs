using Petrol_Station.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class StaffViewModel
    {
        public IEnumerable<Staff> Staffs { get; set; }
        public Staff Staff { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModels DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public StaffFilterViewModel StaffFilterViewModel { get; set; }
    }
}
