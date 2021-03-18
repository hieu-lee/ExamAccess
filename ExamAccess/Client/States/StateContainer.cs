using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamAccess.Client.States
{
    public class StateContainer
    {
        public bool admin { get; set; } = false;
        public string user { get; set; } = string.Empty;
        public string account { get; set; } = string.Empty;
        public bool logged { get; set; } = false;
    }
}
