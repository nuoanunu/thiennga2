using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class EntitiesAM
    {
        public static ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        public static ThienNgaDatabaseEntities getAM() {
            if (am == null) {
                am = new ThienNgaDatabaseEntities();
            }
            return am;
        }
    }
}