using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;
using Twilio;

namespace ThienNga2.Areas.Admin.Controllers
{
    [Authorize(Roles ="admin")]
    [Authorize(Roles = "InventoryController")]
    public class InventoryController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        public ActionResult Autocomplete(string term)
        {
            List<tb_product_detail> items = am.tb_product_detail.ToList();
            List<String> result = new List<String>();
            foreach (tb_product_detail t in items)
            {
                if (t.productName.ToString().IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0) result.Add(t.productName.ToString());
                if (t.productStoreID.ToString().IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0) result.Add(t.productStoreID.ToString());
                if (t.producFactoryID.ToString().IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0) result.Add(t.producFactoryID.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin/Invenotry

        public ActionResult Index()
        {
            ViewData["dssp"] = am.inventories.ToList();
            
            return View("Inventory");
        }
        // GET: Admin/Search
        public ActionResult Search(string code)
        {
           

            ViewData["productdetail"] = am.ThienNga_FindProduct2(code).FirstOrDefault();
            ViewData["dsspdt"] = am.ThienNga_checkkho2(code).ToList(); 
          //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("Inventory");
        }
        public ActionResult Search2(string code)
        {


            ViewData["productdetail"] = am.ThienNga_FindProduct2(code).FirstOrDefault();
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(code).FirstOrDefault();
            //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("NhapKho");
        }
        public ActionResult Search3(string code)
        {


            ViewData["productdetail"] = am.ThienNga_FindProduct2(code).FirstOrDefault();
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(code).FirstOrDefault();
            //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("XuatKho");
        }

        // GET: Admin/Invenotry/Details/
        public ActionResult Details(int id)
        {
           
            return View();
        }
        public ActionResult themkho( )
        {

            return View("NhapKho");
        }
        public ActionResult addkho(InvenotyChangeModel fixkho)
        {
            inventory t =am.inventories.Find(fixkho.inven.id);
            if (fixkho.newadd <= 0) { }
            else
            {
                t.quantity = t.quantity + fixkho.newadd;
                am.SaveChanges();
            }
            ViewData["productdetail"] = am.ThienNga_FindProduct2(fixkho.inven.productFactoryCode).FirstOrDefault();
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(fixkho.inven.productFactoryCode).FirstOrDefault();
            return View("NhapKho");
        }

        public ActionResult trukho()
        {

            return View("XuatKho");
        }
        public ActionResult removeKho(InvenotyChangeModel fixkho)
        {
            inventory t = am.inventories.Find(fixkho.inven.id);
            t.quantity = t.quantity - fixkho.newadd;
            am.SaveChanges();
            ViewData["productdetail"] = am.ThienNga_FindProduct2(fixkho.inven.productFactoryCode).FirstOrDefault();
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(fixkho.inven.productFactoryCode).FirstOrDefault();
            return View("XuatKho");
        }


        // GET: Admin/Invenotry/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Invenotry/Create
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

        // GET: Admin/Invenotry/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Invenotry/Edit/5
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

        // GET: Admin/Invenotry/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Invenotry/Delete/5
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
