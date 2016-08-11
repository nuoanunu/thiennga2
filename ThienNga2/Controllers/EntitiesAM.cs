using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public abstract class EntitiesAM : Controller
    {
        public EntitiesAM()
        {
            am = new ThienNgaDatabaseEntities();
        }

        protected ThienNgaDatabaseEntities am { get; set; }

        protected override void Dispose(bool disposing)
        {
            am.Dispose();
            base.Dispose(disposing);
        }
    }
}