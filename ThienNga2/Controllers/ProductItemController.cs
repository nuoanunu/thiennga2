using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;

namespace ThienNga2.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductItemController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        // GET: ProductItem
        public ActionResult Index()
        {

            ViewData["dsk"] = am.tb_inventory_name.ToList<tb_inventory_name>();
            return View("NewProductItem",new NewItemViewModel());
      
        }
        private List<String> allname = new List<String>();


        public void getAllName()
        {
            allname = am.ThienNga_FindProductName2("").ToList();
            foreach (String e in allname)
            {
                tb_product_detail t = am.ThienNga_FindProduct2(e).FirstOrDefault();
                allname.Add(t.productStoreID);
            }
        }
        public ActionResult Autocomplete(string term)
        {
            allname = am.ThienNga_FindProductName2("").ToList();
            System.Diagnostics.Debug.WriteLine("SIZE " + allname.Count());

            List<String> result = new List<string>();
            foreach (String e in allname)
            {
                if (e.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    result.Add(e);
                }
            }
            //  return Json(result);
            return Json(result, JsonRequestBehavior.AllowGet);
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
            item temp = new item();
            if (tuple.quantity == 0) tuple.quantity = 1;
            if (ModelState.IsValid)
            {
                item newitem = tuple.item;
            var productdetailid = am.ThienNga_FindProductDetailID(  newitem.tb_product_detail.producFactoryID).FirstOrDefault();
            tb_customer cus = am.ThienNga_TimSDT2(tuple.phoneNumber).FirstOrDefault();
            while (cus == null) {
                cus = new tb_customer();
                cus.customerName = tuple.cusName;
                cus.phonenumber = tuple.phoneNumber;
                cus.address = tuple.Adress;
                am.tb_customer.Add(cus);
                am.SaveChanges();
                cus = am.ThienNga_TimSDT2(tuple.phoneNumber).FirstOrDefault();
            }
                if (productdetailid != null)
                {
                    List<inventory> inv = am.ThienNga_checkkho2(newitem.tb_product_detail.producFactoryID).ToList();
                    foreach (inventory invv in inv)
                    {
                        if (invv.inventoryID == newitem.inventoryID)
                        {
                            invv.quantity = invv.quantity - tuple.quantity;
                            am.SaveChanges();
                        }
                    }

                    List<String> lst = new List<String>();
                    List<String> lst2 = new List<String>();
                    for (int i = 0; i < tuple.quantity; i++)
                    {
                        item newI = new item();

  
                        newitem.productID = DateTime.Today.Hour.ToString()+ DateTime.Today.Minute.ToString() +  DateTime.Today.Day.ToString()+ DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + "-" + productdetailid + "-" + cus.phonenumber + "-" + i;
                        newitem.productDetailID = (int)productdetailid;
                        newitem.customerID = cus.id;
                        newitem.inventoryID = newitem.inventoryID;
                        newI.inventoryID = newitem.inventoryID;
                        newI.customerID = cus.id;
                        newI.productDetailID = newitem.productDetailID;
                        newI.productID = newitem.productID;
                        newI.DateOfSold = newitem.DateOfSold;
                        am.items.Add(newI);
                        lst.Add(newI.productID);
                        lst2.Add(newI.id +"" );
                        temp = newI;
                       
                    }
                    //foreach (tb_warranty war in warranties) {
                    //    am.tb_warranty.Add(war);
                    //}
                    am.SaveChanges();

                    ViewData["tuple"] = tuple;
                    ViewData["itemname"] = tuple.item.tb_product_detail.producFactoryID;
                    ViewData["codes"] = lst;
                    ViewData["ids"] = lst2;
                    return View("ConfirmNewProductItem");
                }
            }
            return View("NewProductItem", tuple);
            
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
