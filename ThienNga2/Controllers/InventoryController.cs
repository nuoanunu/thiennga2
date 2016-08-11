using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Controllers;
using ThienNga2.Models;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;
using Twilio;

namespace ThienNga2.Areas.Admin.Controllers
{
    [Authorize(Roles ="admin, InventoryManager")]
    public class InventoryController : Controller
    {
        private ThienNgaDatabaseEntities am = EntitiesAM.am;
        private List<String> allname = new List<String>();
       

 
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
        // GET: Admin/Invenotry

        public ActionResult Index()
        {

            System.Diagnostics.Debug.WriteLine("SIZE " + allname.Count());
            ViewData["allInvenName"] = am.tb_inventory_name.ToList();
            return View("Inventory");
        }
 
        // GET: Admin/Search
        public ActionResult Search(string code)
        {
            if (code.Trim().Length > 0)
            {
                ViewData["allInvenName"] = am.tb_inventory_name.ToList();
                if (code == null || code.Equals("")) return View("Inventory");
                if (code.IndexOf("StoreSKU") > 0)
                {
                    code = code.Substring(code.IndexOf("StoreSKU") + 10, code.Length - code.IndexOf("StoreSKU") - 10);
                }
                tb_product_detail t = am.ThienNga_FindProduct2(code).FirstOrDefault();
                if (t == null) return View("Inventory");
                ViewData["productdetail"] = am.ThienNga_FindProduct2(code).FirstOrDefault();
                ViewData["dsspdt"] = am.ThienNga_checkkho2(t.productStoreID).ToList();
            }
          //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("Inventory");
        }
    
        public ActionResult SearchGetAll( String idd )
        {
            ViewData["allInvenName"] = am.tb_inventory_name.ToList();
            if (idd == null || idd.Equals("")) {
                
                return View("Inventory");

            }
          
         
            int id = int.Parse(idd);
            List<ThienNga_getkho_Result2> lstt = am.ThienNga_getkhoFinal(id).ToList();
            ViewData["invename"] =  am.tb_inventory_name.Find(1).InventoryName;
            ViewData["allInven"] = lstt;
            return View("Inventory");
        }
   
        [HttpPost]
        public ActionResult Search2(string code, string invID)
        {
            if( code.IndexOf("StoreSKU") > 0 ) {
                code = code.Substring(code.IndexOf("StoreSKU") + 10, code.Length - code.IndexOf("StoreSKU") - 10);
            }
             
            System.Diagnostics.Debug.WriteLine(" no dayu ne " + invID);

            ViewData["productdetail"] = am.ThienNga_FindProduct2(code).FirstOrDefault();

            List<inventory> lst = am.ThienNga_checkkho2(code).ToList();
            System.Diagnostics.Debug.WriteLine(" no day ne " + invID + " no day ne" + code + "SIZE NE " + lst.Count() ) ;

            foreach (inventory i in lst) {
                if (i.inventoryID == int.Parse(invID)) ViewData["inventoryDetail"] = i;
            }
            
            //  ViewData["dsspdt"] = am.inventories.ToList();
            return View("NhapKho");
        }
        [HttpPost]
        public ActionResult Search3(string code, string invID)
        {
            if (code.IndexOf("StoreSKU") > 0)
            {
                code = code.Substring(code.IndexOf("StoreSKU") + 10, code.Length - code.IndexOf("StoreSKU") - 10);
            }

            ViewData["productdetail"] = am.ThienNga_FindProduct2(code).FirstOrDefault();
            List<inventory> lst = am.ThienNga_checkkho2(code).ToList();
            foreach (inventory i in lst)
            {
                if (i.inventoryID == int.Parse(invID)) ViewData["inventoryDetail"] = i;
            }
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
            List<tb_inventory_name> nameList = am.tb_inventory_name.ToList();

            List<SelectListItem> ls = new List<SelectListItem>();
            SelectList ls2 = new SelectList(ls);
             
            
            foreach (tb_inventory_name y in nameList) {
                ls.Add(new SelectListItem { Text = y.InventoryName, Value = y.id + "" });
            }
            ViewData["invID"] = ls2;
            ViewBag.invID = new SelectList(ls, "Value", "Text");
            return View("Inventory");
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
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(fixkho.inven.productFactoryCode).ToList();
            return RedirectToAction("Search", "Inventory", new { code = t.productStoreCode+"" });
          
        }

        public ActionResult trukho()
        {

            return View("XuatKho");
        }
        public ActionResult removeKho(InvenotyChangeModel fixkho)
        {
            inventory t = am.inventories.Find(fixkho.inven.id);
            if (fixkho.newadd > t.quantity) return RedirectToAction("Search", "Inventory", new { code = t.productStoreCode + "" });
            t.quantity = t.quantity - fixkho.newadd;
            am.SaveChanges();
            ViewData["productdetail"] = am.ThienNga_FindProduct2(fixkho.inven.productFactoryCode).FirstOrDefault();
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(fixkho.inven.productFactoryCode).ToList();
            return RedirectToAction("Search", "Inventory", new { code = t.productStoreCode + "" });
        }
        public ActionResult changeKho(InvenotyChangeModel fixkho)
        {

            System.Diagnostics.Debug.WriteLine( "  check ID nòa  "  + fixkho.inven.productStoreCode);
            inventory t = am.inventories.Find(fixkho.inven.id);
            int id2 = int.Parse(fixkho.inven.productStoreCode); 
            inventory t2 = am.inventories.Find(id2);
            if(fixkho.newadd > t.quantity) return RedirectToAction("Search", "Inventory", new { code = t.productStoreCode + "" });
            t.quantity = t.quantity - fixkho.newadd;
            t2.quantity = t2.quantity + fixkho.newadd;
            am.SaveChanges();
            ViewData["productdetail"] = am.ThienNga_FindProduct2(fixkho.inven.productFactoryCode).FirstOrDefault();
            ViewData["inventoryDetail"] = am.ThienNga_checkkho2(fixkho.inven.productFactoryCode).ToList();
            return RedirectToAction("Search", "Inventory", new { code = t.productStoreCode + "" });
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
