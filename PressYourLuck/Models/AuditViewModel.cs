using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PressYourLuck.Models
{
    public class AuditViewModel
    {
        public List<Audit> Audits { get; set; }

        public string ActiveType { get; set; }

        public string[] AuditTypes = new string[] { "Cash In", "Cash Out", "Win", "Lose" };
    }
}
