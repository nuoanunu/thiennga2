using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class WarrantyController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        // GET: Warranty
        public ActionResult Index()
        {
            return View("WarrantyCheck");
        }


        public ActionResult Autocomplete(string term)
        {
            List<employee> lst = am.employees.ToList();

            List<String> result = new List<string>();
            foreach (employee e in lst) {
                if (e.employeeName.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                    result.Add(e.employeeName );
                }
            }
          //  return Json(result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: Warranty/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Warranty/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Update(tb_warranty_activities item)
        {
            tb_warranty_activities newStatusa= am.tb_warranty_activities.Find(item.id);
            newStatusa.status = item.status;
            am.SaveChanges();
            ViewData["warrantydetail"] = (tb_warranty)am.ThienNga_findwarranty2(item.warrantyID).FirstOrDefault();
            ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(item.warrantyID).ToList();
            //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("WarrantyCheck");

        }
        [HttpPost]
        public ActionResult Add(tb_warranty_activities t2)
        {
            t2.status = 1;
            t2.employee1 = am.employees.Find(t2.employee);
            t2.tb_warrnaty_status = am.tb_warrnaty_status.Find(t2.status);
            am.tb_warranty_activities.Add(t2);

            am.SaveChanges();
            ViewData["warrantydetail"] = (tb_warranty)am.ThienNga_findwarranty2(t2.warrantyID).FirstOrDefault();
            ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(t2.warrantyID).ToList();
            //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("WarrantyCheck");

        }
        // POST: Warranty/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Warranty/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Warranty/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Search(string code)
        {


            ViewData["warrantydetail"] = (tb_warranty) am.ThienNga_findwarranty2(code).FirstOrDefault();
            ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(code).ToList();
            //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("WarrantyCheck");
        }
        // GET: Warranty/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Warranty/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
