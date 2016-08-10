using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
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

        
            return View("NewProductItem", new NewItemViewModel());

        }
        public String getAllData(String name) {
            if (name != null)
                if(name.Trim().Length >=1)
            {
                List<tb_product_detail> lst = am.tb_product_detail.SqlQuery("SELECT * FROM dbo.tb_product_detail WHERE productStoreID='" + name +"'").ToList();
                System.Diagnostics.Debug.WriteLine("da load xong het " + lst.Count());
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string result = "";
                productView vi = new productView();
                if (lst.Count == 1)
                {

                    vi.name = lst.ElementAt(0).productStoreID;
                    vi.price = lst.ElementAt(0).price + "";
                }
                result = serializer.Serialize(vi);
                System.Diagnostics.Debug.WriteLine(result);
                return result;
            }
            return "";
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
        public string getPrice(String code)
        {
            if (code != null)
            {
                if (code.Length > 0)
                {
                    tb_product_detail detail = am.ThienNga_FindProduct2(code).FirstOrDefault();
                    if (detail != null)
                    {
                        productView vi = new productView();
                        vi.price = detail.price + "";
                        vi.name = detail.productName;

                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        return serializer.Serialize(vi);


                    }
                    productView vi2 = new productView();
                    JavaScriptSerializer serializer2 = new JavaScriptSerializer();
                    vi2.price = "";
                    vi2.name = "";
                    return serializer2.Serialize(vi2);
                }
            }
            return null;
        }
        public ActionResult Autocomplete(string term)
        {

            allname = am.ThienNga_FindProductName2("").ToList();

            foreach (tb_product_detail a in am.tb_product_detail.ToList())
            {
                if (!a.productStoreID.Contains("NULL"))
                    allname.Add(a.producFactoryID);
                allname.Add(a.productStoreID);
            }

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
        public ActionResult Confirm()
        {
            return View("NewProductItem", new NewItemViewModel());

        }
        // GET: ProductItem/Create
        [HttpPost]
        public ActionResult CreateWhenSale(NewItemViewModel tuple)
        {
            item temp = new item();
            if (tuple.quantity == 0) tuple.quantity = 1;
            List<int> lstOrderID = new List<int>();
            List<int> lstOrderDetaiLID = new List<int>();
            List<int> lstItemID = new List<int>();
            List<ConfirmItemView> ConfirmItemViewList = new List<ConfirmItemView>();
            String todelete = "";
            int inventoryID = tuple.inventoryID;
            order ord = new order();
            if (ModelState.IsValid)
            {

                tb_customer cus = am.ThienNga_TimSDT2(tuple.phoneNumber).FirstOrDefault();
                while (cus == null)
                {
                    cus = new tb_customer();
                    cus.customerName = tuple.cusName;
                    cus.phonenumber = tuple.phoneNumber;
                    cus.address = tuple.Adress;
                    am.tb_customer.Add(cus);
                    am.SaveChanges();
                    cus = am.ThienNga_TimSDT2(tuple.phoneNumber).FirstOrDefault();
                    ord.date = DateTime.Today;
                    
                  
         

                }
                float total = 0;
                foreach (AnOrderDetail ao in tuple.items)
                {
                    total = total + ao.thanhTien;
                }
                ord.total = total;
                ord.customerID = cus.id;
                am.orders.Add(ord);
                am.SaveChanges();

                lstOrderID.Add(ord.id);
                foreach (AnOrderDetail ao in tuple.items)
                {
                    tb_product_detail pd = null;
                    if (ao.SKU != null) {
                        if(ao.SKU.Trim().Length > 0)
                        pd = am.ThienNga_FindProduct2(ao.SKU).FirstOrDefault();
                    }
                       
                    if (pd != null)
                    {
                        orderDetail detail = new orderDetail();
                        detail.ChietKhauPhanTram = ao.chietKhauPhanTram + "";
                        detail.ChietKhauTrucTiep = ao.chietKhauTrucTiep + "";
                        detail.Quantity = ao.quantity + "";
                        detail.SoLuong = ao.quantity + "";
                        detail.orderID = ord.id;
                        detail.productDetailID = ao.SKU;
                        am.orderDetails.Add(detail);
                        am.SaveChanges();
                        lstOrderDetaiLID.Add(detail.id);
                        for (int i = 0; i < ao.quantity; i++)
                        {
                            item it = new item();
                            it.customerID = cus.id;
                            it.orderID = ord.id;
                            it.inventoryID = inventoryID;
                            it.productDetailID = pd.id;
                            String day = DateTime.Today.Day + ""; if (day.Length == 1) day = "0" + day;
                            String month = DateTime.Today.Month + ""; if (month.Length == 1) month = "0" + month;
                            String year = DateTime.Today.Year + ""; if (year.Length == 1) year = "0" + year;
                            String hour = DateTime.Now.Hour + ""; if (hour.Length == 1) hour = "0" + hour;
                            String minute = DateTime.Now.Minute + ""; if (minute.Length == 1) minute = "0" + minute;
                            String second = DateTime.Now.Second + ""; if (second.Length == 1) second = "0" + second;
                            it.productID =second+minute+ hour + day + month + year + "-"+pd.productStoreID+"-" + cus.phonenumber+"-"+i;
                            it.DateOfSold = DateTime.Today;
                            am.items.Add(it);
                            am.SaveChanges();
                            lstItemID.Add(it.id);
                            ao.productID = it.productID;
                            ao.productName = pd.productName;
                            ao.thanhTienS  = Convert.ToDecimal(ao.thanhTien).ToString("#,##0.00");
                            ao.DonGiaS = Convert.ToDecimal( (ao.thanhTien/ao.quantity)).ToString("#,##0.00");
                            ao.chietKhauTrucTiepS = Convert.ToDecimal(ao.chietKhauTrucTiep).ToString("#,##0.00");
                        }
                    }
                }
                //foreach (tb_warranty war in warranties) {
                //    am.tb_warranty.Add(war);
                //}


                TempData["tuple"] = tuple;

                Session["oderID"] = lstOrderID;
                Session["orderDetailID"] = lstOrderDetaiLID;
                Session["itemID"] = lstItemID;

                return RedirectToAction("confirmNewItem");

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
        public ActionResult confirmNewItem()
        {
       
            return View("ConfirmNewProductItem");

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
        public ActionResult Delete(String todelete)
        {
            String[] ids = todelete.Split(',');
            for (int i = 0; i < ids.Length; i++)
            {
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
