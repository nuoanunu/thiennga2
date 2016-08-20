using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : EntitiesAM
    {


        // GET: Product
        public ActionResult Index()
        {

            //List<tb_product_detail> dlst = am.tb_product_detail.ToList();
            //List<temp> tlst = am.temps.ToList();
            //int ty = 2000;
            //String[] rex1 = new string[] { " " };
            //foreach (temp t in tlst)
            //{

            //    try
            //    {
            //        if (t.sdt == null) t.sdt = "1231231231";
            //        tb_customer cuss = am.tb_customer.SqlQuery("SELECT * from tb_customer where phonenumber='" + t.sdt + "'").First();
            //        t.date = t.date.Trim();
            //        t.series = t.series.Trim();
            //        if (t.series.Length > 0)
            //        {
            //            String[] rows = t.series.Split(rex1, StringSplitOptions.None);
            //            DateTime date = DateTime.ParseExact(t.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //            System.Diagnostics.Debug.WriteLine("da heo " + date.ToString() + " cus  id " + cuss.id);
            //            item ite = new item();
            //            ite.productDetailID = 481;

            //            ite.customerID = cuss.id;
            //            ite.DateOfSold = date;
            //            ite.orderID = 117;
            //            ite.inventoryID = 1;
            //            ite.productID = "TEMP" + ty;
            //            am.items.Add(ite);
            //            am.SaveChanges();
            //            System.Diagnostics.Debug.WriteLine("da heo " + ite.id);
            //            foreach (String ttt in rows)
            //            {
            //                if (ttt.Trim().Length > 0)
            //                {


            //                    if (am.tb_warranty.SqlQuery("SELECT * FROM tb_warranty WHERE warrantyID='" + ttt + "'").ToList().Count() < 1)
            //                    {
            //                        tb_warranty newar = new tb_warranty();
            //                        newar.warrantyID = ttt.Trim();
            //                        newar.startdate = date;

            //                        newar.itemID = ite.id;
            //                        newar.duration = 24;
            //                        newar.description = " Tạm chưa sữa";
            //                        if (ttt.Equals(rows[0])) newar.MaChinh = true;
            //                        else newar.MaChinh = false;
            //                        am.tb_warranty.Add(newar);

            //                        am.SaveChanges();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //    }


            //}


            return View();
        }
        public List<String> getAll()
        {

            List<String> lst = new List<String>();
            lst.Add("CCC");
            lst.Add("ZZZ");
            foreach (tb_product_detail t in am.tb_product_detail.ToList())
            {
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
