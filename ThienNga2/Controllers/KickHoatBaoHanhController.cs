using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
            ViewData["dspspgd"] = am.items.SqlQuery("SELECT TOP 100 * FROM dbo.item  where productDetailID != 481 ORDER BY DateOfSold DESC ").ToList();
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
            try
            {
                code = code.Trim();
                if (searchType == null) searchType = "id";

                if (code.Trim().Length > 0)
                {
                    if (searchType.Equals("id")) {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE id =" + code).ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;

                            if (lst.Count == 1)
                            {


                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID =" + it.id).ToList();
                            }
                        }
                    }
                    else
                    if (searchType.Equals("masp"))
                    {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%" + code + "%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;

                            if (lst.Count == 1)
                            {


                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID =" + it.id).ToList();
                            }
                        }
                    }
                    else if (searchType.Equals("date"))
                    {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%" + code + "%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                            if (lst.Count == 1)
                            {
                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID = " + it.id).ToList();
                            }
                        }
                    }
                    else if (searchType.Equals("sdt"))
                    {
                        if (code.Length > 6) code = code.Substring(code.Length - 6);
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%" + code + "%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                            if (lst.Count == 1)
                            {
                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID = " + it.id).ToList();
                            }
                        }
                    }
                    else if (searchType.Equals("sku"))
                    {
                        var dssp = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID LIKE '%" + code + "%'").ToList();
                        if (dssp != null)
                        {
                            ViewData["dspsp"] = dssp;
                            List<item> lst = dssp;
                            if (lst.Count == 1)
                            {
                                item it = lst.ElementAt(0);
                                ViewData["warList"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE itemID  =" + it.id).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            var model = new KichHoatBaoHanh();
            return View("index", model);
        }

        public ActionResult Delete(String warrantyID, String itemID)
        {
            System.Diagnostics.Debug.WriteLine("cc j vay " + int.Parse(warrantyID));


            tb_warranty a = am.tb_warranty.Find(int.Parse(warrantyID));
            am.tb_warranty.Remove(a);
            am.SaveChanges();
            return RedirectToAction("Search", "KickHoatBaoHanh", new { code = itemID, searchType = "id" });
        }
        public ActionResult Delete2(String warrantyID, String itemID)
        {

            try
            {
                tb_warranty a = am.tb_warranty.SqlQuery("SELECT * FROM tb_warranty where warrantyID='" + warrantyID + "'").FirstOrDefault();
                am.tb_warranty.Remove(a);
                am.SaveChanges();
                return RedirectToAction("Search", "KickHoatBaoHanh", new { code = itemID, searchType = "id" });
            }
            catch (Exception e) { }
            return RedirectToAction("Search", "KickHoatBaoHanh", new { code = itemID, searchType = "id" });

        }
        public String checking(String IMEI)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<tb_warranty> war = am.tb_warranty.SqlQuery("SELECT * FROM tb_warranty WHERE warrantyID='" + IMEI + "'").ToList();
                if (war != null)
                {
                    if (war.Count > 0)
                    {
                        checkwarModel wwww = new checkwarModel();
                        wwww.name = "co";
                        return serializer.Serialize(wwww);
                    }

                }
            }
            catch (Exception e)
            {

            }
            return "";
        }
        public String newWarranty(String imei, int day, int month, int year, int duration, String mieuta, String chinhphu, int itemID)
        {
            tb_warranty war = new tb_warranty();
            war.itemID = itemID;
            war.warrantyID = imei;
            war.duration = duration;
            war.MaChinh = bool.Parse(chinhphu);
            war.description = mieuta;
            try
            {
                DateTime date = new DateTime(year, month, day);
                if (date != null)
                {
                    if (date.Year > 2000 && date.Month > 0 && date.Day > 0)
                        if (war.itemID > 0 && war.warrantyID != null && war.duration > 0)
                        {


                            war.startdate = date;
                            am.tb_warranty.Add(war);
                            am.SaveChanges();

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            checkwarModel model = new checkwarModel();
                            model.name = "thanhcong";
                            return serializer.Serialize(model);
                        }
                }
            }
            catch (Exception e)
            {

            }



            return "";
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
