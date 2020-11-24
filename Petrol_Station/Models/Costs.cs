using System;
using System.Collections.Generic;

namespace Petrol_Station.Models
{
    public partial class Costs
    {
        public int CostId { get; set; }
        public int? TypeOfGsmid { get; set; }
        public double PricePerLiter { get; set; }
        public DateTime DateOfCostGsm { get; set; }

        public virtual Gsm TypeOfGsm { get; set; }
    }
}
