using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petrol_Station.ViewModels
{
    public class GroupViewModel
    {
        public Dictionary<string, int> KeyValuePairs { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
    }
}
