using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class StaffWeekdaysViewModel
    {
        public IEnumerable<StaffsWeekdays> staffsWeekdays { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
