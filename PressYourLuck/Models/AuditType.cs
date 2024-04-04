using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PressYourLuck.Models
{
    public class AuditType
    {
        // a primary key
        public int AuditTypeId { get; set; }

        [Required(ErrorMessage = "Please enter Audit Type Name.")]
        public string Name { get; set; }


    }
}
