using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models;
using ThienNga2.Models.Entities;


namespace ThienNga2.Areas.Admin.Controllers
{
    [Authorize(Roles ="admin")]
    public class InventoryController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
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

        // GET: Admin/Invenotry/Details/
        public ActionResult Details(int id)
        {
           
            return View();
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
