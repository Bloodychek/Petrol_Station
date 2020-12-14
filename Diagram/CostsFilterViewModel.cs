using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class CostsFilterViewModel
    {
        [Display(Name = "PricePerLiter")]
        public double PricePerLiter { get; set; }

        [Display(Name = "DateOfCostGsm")]
        public DateTime DateOfCostGsm { get; set; }
    }
}
