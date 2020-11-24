using System;
using System.Collections.Generic;

namespace Petrol_Station.Models
{
    public partial class Gsm
    {
        public Gsm()
        {
            Containers = new HashSet<Containers>();
            Costs = new HashSet<Costs>();
        }

        public int GSmid { get; set; }
        public string TypeOfGsm { get; set; }
        public string TTkofType { get; set; }

        public virtual ICollection<Containers> Containers { get; set; }
        public virtual ICollection<Costs> Costs { get; set; }
    }
}
