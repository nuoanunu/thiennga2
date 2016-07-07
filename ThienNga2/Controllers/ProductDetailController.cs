using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class ProductDetailController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        // GET: ProductDetail
        public ActionResult Index()
        {
           
            return View("NewProduct");
        }

        // GET: ProductDetail/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [HttpPost]
        // POST: ProductDetail/Create
        public ActionResult Create(tb_product_detail Model)
        {
            if (Model != null)
            {
                am.tb_product_detail.Add(Model);
                am.SaveChanges();
                return View("NewProduct", Model);
            }
            return View("NewProduct");
        }

       
    

        // GET: ProductDetail/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductDetail/Edit/5
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

        // GET: ProductDetail/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductDetail/Delete/5
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
