using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PressYourLuck.Helpers;
using PressYourLuck.Models;
using Microsoft.EntityFrameworkCore;

namespace PressYourLuck.Controllers
{
    public class AuditController : Controller
    {
        private AuditContext _auditContext;

        public AuditController(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        // GET action method for display a list of audit records, stored in reverse chronological
        [HttpGet("audit/{type?}")]
        public IActionResult Index(string type)
        {
            var audit = _auditContext.Audits.Include(m => m.AuditType).OrderByDescending(g => g.CreatedDate).ToList();
            string selectedType = HttpContext.Session.GetString("selected-type");

            if (type == null)
            {
                if (selectedType != null && selectedType != "All")
                {
                    type = selectedType;
                    audit = audit.Where(t => t.AuditType.Name == type).ToList();
                    HttpContext.Session.SetString("selected-type", type);
                }
                else
                {
                    audit = _auditContext.Audits.Include(m => m.AuditType).OrderByDescending(g => g.CreatedDate).ToList();
                    type = "All";
                    HttpContext.Session.SetString("selected-type", type);
                }
            }
            else
            {
                if (type == "All")
                {
                    audit = _auditContext.Audits.Include(m => m.AuditType).OrderByDescending(g => g.CreatedDate).ToList();
                    type = "All";
                    HttpContext.Session.SetString("selected-type", type);
                }
                else
                {
                    audit = audit.Where(t => t.AuditType.Name == type).ToList();
                    HttpContext.Session.SetString("selected-type", type);
                }
            }

            AuditViewModel viewModel = new AuditViewModel()
            {
                Audits = audit,
                ActiveType = type,
                AuditTypes = new string[] { "Cash In", "Cash Out", "Win", "Lose" }
            };

            return View(viewModel);
        }
    }
}
