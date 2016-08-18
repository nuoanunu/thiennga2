using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class BaoGiaController : EntitiesAM
    {

        [Authorize(Roles = "Admin")]
        // GET: BaoGia
        public ActionResult Index()
        {
            ViewData["allBaoGia"] = am.orders.ToList();
            return View("BaoGiaList");
        }
        public ActionResult Search(String code)
        {
            List<orderDetail> lst = am.orderDetails.SqlQuery("SELECT * FROM orderDetail WHERE orderID=" + code).ToList();
            if (lst.Count() > 0) {
                ViewData["oderdetails"] = lst;
            }
            return View("BaoGiaChiTiet");
        }

    }
}