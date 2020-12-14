using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class GsmFilterViewModel
    {
        [Display(Name = "TypeOfGsm")]
        public string TypeOfGsm { get; set; }

        [Display(Name = "TTkofType")]
        public string TTkofType { get; set; }
    }
}
