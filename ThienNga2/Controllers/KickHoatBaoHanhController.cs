using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;
namespace ThienNga2.Controllers
{
    public class KickHoatBaoHanhController : EntitiesAM
    {
        [Authorize(Roles = "Admin,Bán hàng,Admin Hà Nội")]
        // GET: KickHoatBaoHanh
        public ActionResult Index()
        {
            var model = new KichHoatBaoHanh();
            return View(model);
        }

        // GET: KickHoatBaoHanh/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: KickHoatBaoHanh/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: KickHoatBaoHanh/Search
        public ActionResult Search(String code, String searchType)
        {
            try {  
            code = code.Trim();
            if (searchType == null) searchType = "id";          
                if (code.Trim().Length > 0) {
                    if (searchType.Equals("masp")) {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%" + code + "%'").ToList();
                        if(dssp!=null){
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                     
                            if (lst.Count == 1) {
               

                              item it = lst.ElementAt(0);
                               ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID LIKE '%" + it.productID + "%'").ToList();
                            }
                        }
                    }
                    else if (searchType.Equals("date")) {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%" + code + "-%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                            if (lst.Count == 1)
                            {
                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID LIKE '%" + it.productID + "%'").ToList();
                            }
                        }
                    }
                    else if (searchType.Equals("sdt")) {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%-" + code + "-%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                            if (lst.Count == 1)
                            {
                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID LIKE '%" + it.productID + "%'").ToList();
                            }
                        }
                    }
                    else if (searchType.Equals("sku")) {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%-" + code + "-%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                            if (lst.Count == 1)
                            {
                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID LIKE '%" + it.productID + "%'").ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
               
            }
            var model = new KichHoatBaoHanh();
            return View("index" , model);
        }
        [HttpPost]
        public ActionResult Delete(String warrantyID, String itemID)
        {
            System.Diagnostics.Debug.WriteLine("cc j vay " + int.Parse(warrantyID));

           
            tb_warranty a = am.tb_warranty.Find(int.Parse(warrantyID));
            am.tb_warranty.Remove(a);
            am.SaveChanges();
            return RedirectToAction("Search", "KickHoatBaoHanh", new { code = itemID, searchType = "masp" });
        }
        public ActionResult newWarranty( KichHoatBaoHanh kick) {
           
            try
            {
                int i = 0;
                foreach (tb_warranty war in kick.lst)
                {
                   
                    
                    System.Diagnostics.Debug.WriteLine("dahel " + i + " "+ war.itemID + "   " + war.warrantyID + "  " + war.startdate);
                    try
                    {
                        DateTime date = new DateTime(kick.lstYear[i], kick.lstMonth[i], kick.lstDay[i]);
                        if (date != null)
                        {
                            if (date.Year > 2000 && date.Month > 0 && date.Day > 0)
                                if (war.itemID != null && war.warrantyID != null && war.duration > 0)
                                {


                                    war.startdate = date;
                                    am.tb_warranty.Add(war);
                                }
                        }
                    }
                    catch (Exception e) {

                    }
                    i = i + 1;
                }
                am.SaveChanges();
            }
            catch (Exception e) { }
            return RedirectToAction("Search", "KickHoatBaoHanh", new { code = kick.itemID, searchType = "masp" });
        }
        
        // POST: KickHoatBaoHanh/Create
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

        // GET: KickHoatBaoHanh/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KickHoatBaoHanh/Edit/5
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

        // GET: KickHoatBaoHanh/Delete/5
  

        // POST: KickHoatBaoHanh/Delete/5
        
    }
}
