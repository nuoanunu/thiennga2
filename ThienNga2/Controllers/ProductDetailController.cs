using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin, InventoryManager")]
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
                int id = -1;
                if (am.ThienNga_FindProductDetailID(Model.productStoreID).FirstOrDefault().HasValue )
                    id = (int)am.ThienNga_FindProductDetailID(Model.productStoreID).FirstOrDefault().Value;
                else if (am.ThienNga_FindProductDetailID(Model.producFactoryID).FirstOrDefault().HasValue)
                    id = (int)am.ThienNga_FindProductDetailID(Model.producFactoryID).FirstOrDefault().Value;
                else if (am.ThienNga_FindProductDetailID(Model.productName).FirstOrDefault().HasValue )
                    id = (int)am.ThienNga_FindProductDetailID(Model.productName).FirstOrDefault().Value;
                if (id < 1 )
                {
                    Model.tb_cate = am.tb_cate.Find(Model.cateID);
                    if (Model.producFactoryID == null || Model.producFactoryID.Equals("")) { Model.producFactoryID = Model.productStoreID; }
                    am.tb_product_detail.Add(Model);
                    am.SaveChangesAsync();
                }
                else {
       
                    tb_product_detail edit = am.tb_product_detail.Find(id);
                    edit.cateID = Model.cateID;
             
                    edit.price = Model.price;
                    edit.producFactoryID = Model.producFactoryID;
                    edit.productStoreID = Model.productStoreID;

                    edit.productName = Model.productName;
                    am.SaveChanges();
                }
                ViewData["newproduct"] = Model;
                return View("ConfirmNewProduct");
            }
            return View("NewProduct");
        }

       
    

        [HttpPost]
        public ActionResult Edit(tb_product_detail t , string Command)
        {
           
            if (Command.Equals("save")) {
                ModelState.Clear();
                return View("NewProduct");
            }
            else if(Command.Equals ( "edit")) {
                return View("NewProduct" ,t);
            }
            return View(Command);

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
