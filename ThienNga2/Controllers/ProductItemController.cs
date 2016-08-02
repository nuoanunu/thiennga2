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
            System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAA " + term);

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
        public ActionResult Confirm( )
        {
            return View("NewProductItem", new NewItemViewModel());

        }
        // GET: ProductItem/Create
        [HttpPost]
        public ActionResult CreateWhenSale( NewItemViewModel tuple)
        {
            item temp = new item();
            if (tuple.quantity == 0) tuple.quantity = 1;
            List<String> lst = new List<String>();
            List<String> lst2 = new List<String>();
            List<ConfirmItemView> ConfirmItemViewList = new List<ConfirmItemView>();
            String todelete = "";


            if (ModelState.IsValid)
            {
     
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
                for (int j = 0; j < tuple.items.Count; j++)
                {

                    item newitem = tuple.items[j];
                    if (newitem.tb_product_detail.producFactoryID != null ) { 
                    var productdetailid = am.ThienNga_FindProductDetailID(newitem.tb_product_detail.producFactoryID).FirstOrDefault();
                    tb_product_detail itt = am.ThienNga_FindProduct2(newitem.tb_product_detail.producFactoryID).FirstOrDefault();
                    if (newitem.tb_product_detail.producFactoryID == null || newitem.tb_product_detail.producFactoryID.Equals("")) productdetailid = null;
                    String skuu = am.tb_product_detail.Find(productdetailid).productStoreID;

                    if (productdetailid != null)
                    {
                        ConfirmItemView templist = new ConfirmItemView();
                        templist.lst = new List<String>();
                        templist.quantity = (int)newitem.customerID;
                        templist.price = itt.price * templist.quantity;
                        List<inventory> inv = am.ThienNga_checkkho2(newitem.tb_product_detail.producFactoryID).ToList();
                        foreach (inventory invv in inv)
                        {
                            if (invv.inventoryID == newitem.inventoryID)
                            {
                                invv.quantity = invv.quantity - (int)newitem.customerID;
                                am.SaveChanges();
                            }
                        }


                        for (int i = 0; i < templist.quantity; i++)
                        {
                            System.Diagnostics.Debug.WriteLine("HEREEEEEEEEEEEEE");
                            item newI = new item();
                            String date = DateTime.Today.Day.ToString(); if (date.Length == 1) date = "0" + date;
                            String month = DateTime.Today.Month.ToString(); if (month.Length == 1) month = "0" + month;
                            newitem.productID = DateTime.Today.Hour.ToString() + DateTime.Today.Minute.ToString() + date + month + DateTime.Today.Year.ToString() + "-" + skuu + "-" + cus.phonenumber + "-" + i;
                            newitem.productDetailID = (int)productdetailid;
                            newitem.customerID = cus.id;
                            newitem.inventoryID = newitem.inventoryID;
                            newI.inventoryID = newitem.inventoryID;
                            newI.customerID = cus.id;
                            newI.productDetailID = newitem.productDetailID;
                            newI.productID = newitem.productID;
                            if (newitem.DateOfSold.Equals("01/01/0001 00:00:00")) newitem.DateOfSold = DateTime.Today;
                            newI.DateOfSold = newitem.DateOfSold;
                            am.items.Add(newI);
                            lst.Add(newI.productID);
                            lst2.Add(newI.id + "");
                            temp = newI;
                            templist.lst.Add(newI.productID);
                            am.SaveChanges();
                                todelete = todelete + "," + newI.id;
                            }
                        ConfirmItemViewList.Add(templist);
                    }
                }
                }
                    //foreach (tb_warranty war in warranties) {
                    //    am.tb_warranty.Add(war);
                    //}
               

                    ViewData["tuple"] = tuple;
                 
                    ViewData["codes"] = lst;
                ViewData["todelete"] = todelete;
                ViewData["ConfirmItemViewList"] = ConfirmItemViewList;
                ConfirmItemView te = new ConfirmItemView();
 
                    return View("ConfirmNewProductItem") ;
                
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
        public ActionResult Delete(String todelete) {
            String[] ids = todelete.Split(',');
            for(int i=0; i<ids.Length; i++) {
                if (ids[i] != null && ids[i].Length > 0)
                {
                    
                    var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE id =" + ids[i]).ToList();
                    if (dssp != null)
                    {
                        if (dssp.Count == 1)
                        {
                            item temp = dssp.ElementAt(0);
                            am.items.Remove(temp);
                            am.SaveChanges();
                        }
                    }
                }
            }
            am.SaveChanges();
            return View("NewProductItem", new NewItemViewModel());
        }

  

        // POST: ProductItem/Delete/5
   
    }
}
