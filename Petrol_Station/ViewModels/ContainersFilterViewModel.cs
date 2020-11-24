using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class ContainersFilterViewModel
    {
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Display(Name = "TankCapacity")]
        public double TankCapacity { get; set; }
    }
}
