using Petrol_Station.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class CostsViewModel
    {
        public IEnumerable<Costs> Costs { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public Costs Cost { get; set; }
        public DeleteViewModels DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public CostsFilterViewModel CostsFilterViewModel { get; set; }
    }
}
