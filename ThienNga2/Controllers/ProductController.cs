using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class ProductController : Controller
    {
        private ThienNgaEntities3 am = new ThienNgaEntities3();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public List<String> getAll()
        {
           
            List<String> lst = new List<String>();
            lst.Add("CCC");
            lst.Add("ZZZ");
            foreach (tb_product_detail t in am.tb_product_detail.ToList()) {
                lst.Add(t.producFactoryID);
                lst.Add(t.productName);
                lst.Add(t.productStoreID);
            }
            return lst;
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
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

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Product/Edit/5
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

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Product/Delete/5
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
