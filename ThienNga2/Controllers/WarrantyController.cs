using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    [Authorize(Roles = "admin")]

    public class WarrantyController : Controller
    {
        private ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
        // GET: Warranty
        public ActionResult Index()
        {
            return View("WarrantyCheck");
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
        public double loadPrice(String code, int quantity)
        {

            ViewData["allInvenName"] = am.tb_inventory_name.ToList();
            if (code == null || code.Equals("")) return 0;
            if (code.IndexOf("StoreSKU") > 0)
            {
                code = code.Substring(code.IndexOf("StoreSKU") + 10, code.Length - code.IndexOf("StoreSKU") - 10);
            }
            tb_product_detail t = am.ThienNga_FindProduct2(code).FirstOrDefault();
            if (t == null) return 0;
            else return t.price * quantity;
        }
        public ActionResult UpdateWithFee(tb_warranty_activities item)
        {
            
            tb_warranty_activities newStatusa = am.tb_warranty_activities.Find(item.id);
            
            newStatusa.status = item.status;
            am.SaveChanges();
            ViewData["warrantydetail"] = (tb_warranty)am.ThienNga_findwarranty2(item.warrantyID).FirstOrDefault();
            ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(item.warrantyID).ToList();
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
        public ActionResult XoaFee(String feeID,  String activitiesID)
        {
            am.warrantyActivityFees.Remove(am.warrantyActivityFees.Find(int.Parse(feeID)));
            am.SaveChanges();
            return RedirectToAction("Search", "Warranty", new { code = activitiesID, searchType = "warrantyActID" });

        }
        public ActionResult XoaFixingFee(String feeID, String activitiesID)
        {
            am.warrantyActivityFixingFees.Remove(am.warrantyActivityFixingFees.Find(int.Parse(feeID)));
            am.SaveChanges();
            return RedirectToAction("Search", "Warranty", new { code = activitiesID, searchType = "warrantyActID" });

        }
        public ActionResult AddFee( String ksu, String quantity , String fixPrice, String activitiesID )
        {
            if (ksu != null && ksu.Trim().Length > 0)
            {
                tb_product_detail pd = am.ThienNga_FindProduct2(ksu).FirstOrDefault();
                if (pd != null)
                {
                    tb_warranty_activities act = am.tb_warranty_activities.Find(int.Parse(activitiesID));
                    if (act != null) {
                        warrantyActivityFee a = new warrantyActivityFee();
                        a.activityID = int.Parse(activitiesID);
                        a.productSKU = ksu;
                        a.fixingfee = float.Parse(fixPrice);
                        a.quantity = int.Parse(quantity);
                        am.warrantyActivityFees.Add(a);
                        am.SaveChanges();
                            }
                }
            }
            return RedirectToAction("Search", "Warranty", new { code = activitiesID , searchType= "warrantyActID" });
        }
        public ActionResult AddFixingFee(String fixDetail, String price, String activitiesID)
        {
            tb_warranty_activities act = am.tb_warranty_activities.Find(int.Parse(activitiesID));
            if (fixDetail != null && fixDetail.Trim().Length > 0)
            {
                tb_product_detail pd = am.ThienNga_FindProduct2(price).FirstOrDefault();
                if (pd != null)
                {
                    
                    if (act != null)
                    {
                        warrantyActivityFixingFee a = new warrantyActivityFixingFee();
                        a.activityID = int.Parse(activitiesID);
                        a.FixDetail = fixDetail;
                        a.fee = float.Parse(price);
                        am.warrantyActivityFixingFees.Add(a);
                        am.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Search", "Warranty", new { code = activitiesID, searchType = "warrantyActID" });
        }
        public ActionResult Search(string code ,string searchType)
        {
            if (searchType ==null ) searchType = "warrantyActID";
            if (searchType.Equals("item")) {
                item thatitem = (item)am.ThienNga_findbyIMEI2(code).FirstOrDefault();
                if (thatitem != null) {
                    ViewData["warrantydetail"] = (List<tb_warranty>)am.ThienNga_findwarrantyByIMEI2(code).ToList();
                    List<tb_warranty> temp1 = (List<tb_warranty>)am.ThienNga_findwarrantyByIMEI2(code).ToList();
                    List<tb_warranty_activities> temp2 = new List<tb_warranty_activities>();
                    foreach (tb_warranty a in temp1)
                    {
                        List<tb_warranty_activities> temp3 = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(a.warrantyID).ToList();
                        if (temp3 != null && temp3.Count() > 0)
                            temp2.AddRange(temp3);
                    }
                    ViewData["lsbh"] = temp2;
                }
            }
            if (searchType.Equals("warrantyIMEI")) {
                ViewData["warrantydetail"] = (List<tb_warranty>)am.ThienNga_findwarranty2(code).ToList();
                ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(code).ToList();
            }
            if (searchType.Equals("warrantyCODE")) {
                var activity = am.tb_warranty_activities.SqlQuery("SELECT * FROM dbo.tb_warranty_activities WHERE CodeBaoHanh='" + code + "'").ToList();
                if (activity.Count == 1)
                {
                    var act = activity.First();
                    ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(act.itemID).ToList();
                    ViewData["warrantydetail"] = (List<tb_warranty>)am.ThienNga_findwarranty2(act.warrantyID).ToList();
                }
                else {
                    ViewData["lsbh"] = activity;
                }
            }
            if (searchType.Equals("warrantyCODEDate"))
            {
                var activity = am.tb_warranty_activities.SqlQuery("SELECT * FROM dbo.tb_warranty_activities WHERE CodeBaoHanh LIKE '%" + code + "-%'").ToList();
                ViewData["lsbh"] = activity;

            }
            if (searchType.Equals("warrantyCODEPhone"))
            {
                var activity = am.tb_warranty_activities.SqlQuery("SELECT * FROM dbo.tb_warranty_activities WHERE CodeBaoHanh Like '%-" + code + "%'").ToList();
                ViewData["lsbh"] = activity;
    
            }
            if (searchType.Equals("warrantyActID"))
            {
                var activity = am.tb_warranty_activities.SqlQuery("SELECT * FROM dbo.tb_warranty_activities WHERE id = " + code ).ToList();
                ViewData["lsbh"] = activity;
                ViewData["warrantydetail"] = (List<tb_warranty>)am.ThienNga_findwarranty2(activity.First().warrantyID).ToList();
            }




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
