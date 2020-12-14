using Petrol_Station.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class ContainersViewModel
    {
        public IEnumerable<Containers> Containers { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public Containers Container { get; set; }
        public DeleteViewModels DeleteViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public ContainersFilterViewModel ContainersFilterViewModel { get; set; }
    }
}
