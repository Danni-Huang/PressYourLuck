using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PressYourLuck.Helpers;

namespace PressYourLuck.Models
{
    public class Audit 
    {
        // a primary key
        public int AuditId { get; set; }

        public string PlayerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Amount { get; set; }

        // foreign key relationship to the AuditType
        public int AuditTypeId { get; set; }
        public AuditType AuditType { get; set; }



     


    }
}
