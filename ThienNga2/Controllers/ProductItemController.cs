using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;

namespace ThienNga2.Controllers
{
    public class ProductItemController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        // GET: ProductItem
        public ActionResult Index()
        {
           
            return View("NewProductItem",new NewItemViewModel());
      
        }

        // GET: ProductItem/Details/5
        public ActionResult Details(int id)
        {
            return View("NewProductItem");
        }

        // GET: ProductItem/Create
        [HttpPost]
        public ActionResult CreateWhenSale( NewItemViewModel tuple)
        {
         
            item newitem = tuple.item;
            var productdetailid = am.ThienNga_FindProductDetailID(  newitem.tb_product_detail.producFactoryID).FirstOrDefault();
            tb_customer cus = am.ThienNga_TimSDT2(newitem.tb_customer.phonenumber).FirstOrDefault();
            while (cus == null) {
                cus = new tb_customer();
                cus.customerName = newitem.tb_customer.customerName;
                cus.phonenumber = newitem.tb_customer.phonenumber;
                cus.address = newitem.tb_customer.address;
                am.tb_customer.Add(cus);
                am.SaveChanges();
                cus = am.ThienNga_TimSDT2(newitem.tb_customer.phonenumber).FirstOrDefault();
            }
            Console.WriteLine("CUSID NE " + cus.id);
            if (productdetailid != null)
            {
                newitem.tb_customer = null;
                newitem.tb_product_detail = null;
                
                newitem.productDetailID = (int)productdetailid;
                newitem.customerID =cus.id;
                newitem.inventoryID = newitem.inventoryID;
                
                am.items.Add(newitem);
                //foreach (tb_warranty war in warranties) {
                //    am.tb_warranty.Add(war);
                //}
                am.SaveChanges();
   
                foreach(tb_warranty war in tuple.warranties) {
                    if (war.warrantyID != null)
                    {
                        if (war.warrantyID.Length > 2)
                        {
                            war.itemID = newitem.productID;
                            war.startdate = newitem.DateOfSold;
                            am.tb_warranty.Add(war);
                        }
                                                                       
                    }
                }
                am.SaveChanges();
                return View("NewProductItem");
            }
            else return View("NewProductItem", tuple);
            
        }

        // POST: ProductItem/Create
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

        // GET: ProductItem/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductItem/Edit/5
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

        // GET: ProductItem/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductItem/Delete/5
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
