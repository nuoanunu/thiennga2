using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;
using Microsoft.AspNet.Identity;
namespace ThienNga2.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên kỹ thuật, Bán hàng,Admin Hà Nội")]

    public class WarrantyController : EntitiesAM
    {

        // GET: Warranty
        public ActionResult Index()
        {
            return View("WarrantyCheck");
        }
        public String getTen(String email)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                AspNetUser asp = am.AspNetUsers.SqlQuery("SELECT * FROM AspNetUsers where Email = '" + email + "'").First();
                if (asp != null)
                {
                    emp em = new emp();
                    em.email = email;
                    em.sdt = asp.PhoneNumber;
                    em.fullname = asp.FullName;
                    return serializer.Serialize(em);
                }
            }
            catch (Exception e)
            {

            }
            return "";

        }
        private List<String> allname = new List<String>();

        public ActionResult ConfirmBaoHanh(String idwar, String iduser)
        {
            try
            {
                tb_warranty_activities act = am.tb_warranty_activities.SqlQuery("SELECT * from tb_warranty_activities where CodeBaoHanh= '" + idwar + "'").First();
                AspNetUser user = am.AspNetUsers.SqlQuery("SELECT * FROM AspNetUsers where Email='" + iduser + "'").First();
                System.Diagnostics.Debug.WriteLine("act " + idwar + " iduser " + iduser);
                if (act != null && user != null)
                {
                    act.empFixer = user.Email;
                    act.AspNetUser1 = user;
                    am.SaveChanges();
                    return RedirectToAction("Search", "Warranty", new { code = act.id, searchType = "warrantyActID" });
                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Index", "Warranty");



        }
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

        public ActionResult Update(String actid, String itemstatus, String day1, String month1, String year1, String day2, String month2, String year2)
        {
            try
            {
                tb_warranty_activities act = am.tb_warranty_activities.Find(int.Parse(actid));
                if (act != null)
                    if (User.Identity.GetUserName().Equals(act.AspNetUser1.Email))
                    {

                        act.status = int.Parse(itemstatus);
                        if (day1 != null && month1 != null && year1 != null)
                        {
                            if (day1.Trim().Length > 0 && month1.Trim().Length > 0 && year1.Trim().Length > 0)
                            {
                                int day = int.Parse(year1);
                                int month = int.Parse(month1);
                                int year = int.Parse(day1);
                                DateTime date = new DateTime(year, month, day);
                                act.realeaseDATE = date;
                            }
                            if (day2 != null && month2 != null && year2 != null)
                            {
                                if (day2.Trim().Length > 0 && month2.Trim().Length > 0 && year2.Trim().Length > 0)
                                {
                                    int day = int.Parse(year2);
                                    int month = int.Parse(month2);
                                    int year = int.Parse(day2);
                                    DateTime date = new DateTime(year, month, day);
                                    act.realeaseDATE = date;
                                }


                            }
                            am.SaveChanges();
                            ViewData["warrantydetail"] = (tb_warranty)am.ThienNga_findwarranty2(act.warrantyID).FirstOrDefault();
                            ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(act.warrantyID).ToList();
                            //  ViewData["dsspdt"] = am.inventories.ToList();
                        }
                    }
            }
            catch (Exception e) { }

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
            if (User.Identity.GetUserName().Equals(item.AspNetUser1.Email))
            {
                tb_warranty_activities newStatusa = am.tb_warranty_activities.Find(item.id);

                newStatusa.status = item.status;
                am.SaveChanges();
            }
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
        public ActionResult XoaFee(String feeID, String activitiesID)
        {
            warrantyActivityFee item = am.warrantyActivityFees.Find(int.Parse(feeID));
            if (User.Identity.GetUserName().Equals(item.tb_warranty_activities.AspNetUser1.Email))
            {
                warrantyActivityFee editor = am.warrantyActivityFees.Find(int.Parse(feeID));
                editor.active = false;
                am.SaveChanges();
            }
            return RedirectToAction("Search", "Warranty", new { code = activitiesID, searchType = "warrantyActID" });

        }
        public ActionResult XoaFixingFee(String feeID, String activitiesID)
        {
            warrantyActivityFixingFee item = am.warrantyActivityFixingFees.Find(int.Parse(feeID));
            if (User.Identity.GetUserName().Equals(item.tb_warranty_activities.AspNetUser1.Email))
            {
                warrantyActivityFixingFee editor = am.warrantyActivityFixingFees.Find(int.Parse(feeID));
                editor.active = false;
                am.SaveChanges();
            }
            return RedirectToAction("Search", "Warranty", new { code = activitiesID, searchType = "warrantyActID" });

        }
        public ActionResult AddFee(String activitiesID, String ksu, String quantity, String fixPrice)
        {
            if (ksu != null && ksu.Trim().Length > 0)
            {
                tb_product_detail pd = am.ThienNga_FindProduct2(ksu).FirstOrDefault();
                if (pd != null)
                {
                    tb_warranty_activities act = am.tb_warranty_activities.Find(int.Parse(activitiesID));
                    if (act != null)
                    {

                        if (act.AspNetUser1.Email.Equals(User.Identity.GetUserName()))
                        {
                            System.Diagnostics.Debug.WriteLine("dafug");
                            warrantyActivityFee a = new warrantyActivityFee();
                            a.activityID = int.Parse(activitiesID);
                            a.productSKU = ksu;
                            a.fixingfee = act.tb_product_detail.price * int.Parse(quantity);
                            a.quantity = int.Parse(quantity);
                            am.warrantyActivityFees.Add(a);
                            am.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("Search", "Warranty", new { code = activitiesID, searchType = "warrantyActID" });
        }

        public ActionResult AllWarranty()
        {
            ViewData["allact"] = am.tb_warranty_activities.ToList();
            return View("WarrantyList");
        }
        public ActionResult AddFixingFee(String fixDetail, String price, String activitiesID)
        {
            tb_warranty_activities act = am.tb_warranty_activities.Find(int.Parse(activitiesID));
            if (fixDetail != null && fixDetail.Trim().Length > 0)
            {


                if (act != null)
                {
                    if (act.AspNetUser1.Email.Equals(User.Identity.GetUserName()))
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
        public ActionResult Search(string code, string searchType)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("searchType " + searchType);
                if (searchType == null) searchType = "warrantyActID";
                if (searchType.Equals("item"))
                {
                    item thatitem = (item)am.ThienNga_findbyIMEI2(code).FirstOrDefault();
                    if (thatitem != null)
                    {
                        ViewData["warrantydetail"] = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE warrantyID='" + code + "'").FirstOrDefault();
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
                if (searchType.Equals("warrantyIMEI"))
                {
                    ViewData["warrantydetail"] = am.ThienNga_findwarranty2(code).FirstOrDefault();
                    ViewData["lsbh"] = (List<tb_warranty_activities>)am.ThienNga_warrantyHistory2(code).ToList();
                }
                if (searchType.Equals("warrantyCODE"))
                {
                    System.Diagnostics.Debug.WriteLine("HEREEEEEEEEEEEEEEEEE " + searchType);
                    var activity = am.tb_warranty_activities.SqlQuery("SELECT * FROM dbo.tb_warranty_activities WHERE CodeBaoHanh='" + code + "'").ToList();
                    if (activity.Count == 1)
                    {

                        tb_warranty_activities act = activity.ElementAt(0);
                        ViewData["lsbh"] = activity;
                        ViewData["warrantydetail"] = am.ThienNga_findwarranty2(act.warrantyID).FirstOrDefault();
                    }
                    else
                    {
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


                    var activity = am.tb_warranty_activities.SqlQuery("SELECT * FROM dbo.tb_warranty_activities WHERE id = " + code).ToList();
                    ViewData["lsbh"] = activity;
                    ViewData["warrantydetail"] = am.ThienNga_findwarranty2(activity.First().warrantyID).FirstOrDefault();
                }
            }

            catch (Exception e) { }

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
