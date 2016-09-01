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
using System.IO;
using System.Text;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using iTextSharp.text.html;

namespace ThienNga2.Controllers
{

    [Authorize(Roles = "Admin,Bán hàng,Admin Hà Nội")]
    public class ProductItemController : EntitiesAM
    {
        private int number = 1;
        // GET: ProductItem
        public ActionResult Index()
        {

            ViewData["dsk"] = am.tb_inventory_name.ToList();

            ViewData["sdct"] = am.CustomerTypes.ToList();
            return View("NewProductItem", new NewItemViewModel());

        }
        public String getdataKhachHang(String sdt)
        {
            cusInfo cus = new cusInfo();
            try
            {
                tb_customer findcus = am.tb_customer.SqlQuery("SELECT * FROM tb_customer WHERE phonenumber='" + sdt + "'").FirstOrDefault();
                if (findcus != null)
                {
                    cus.cusadd = findcus.address;
                    cus.cusadd2 = findcus.address2;
                    cus.cusname = findcus.customerName;
                    cus.cussdt = findcus.phonenumber;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Serialize(cus);
                }
            }
            catch (Exception e)
            {

            }
            return "";
        }
        public String getAllData(String name)
        {
            try
            {
                if (name != null)
                    if (name.Trim().Length >= 1)
                    {
                        List<tb_product_detail> lst = am.tb_product_detail.SqlQuery("SELECT * FROM dbo.tb_product_detail WHERE productStoreID='" + name + "'").ToList();
                        System.Diagnostics.Debug.WriteLine("da load xong het " + lst.Count());
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string result = "";
                        productView vi = new productView();
                        if (lst.Count == 1)
                        {
                            if (lst.ElementAt(0).productName.Length >= 17)
                                vi.name = lst.ElementAt(0).productName;

                            vi.price = lst.ElementAt(0).price + "";
                        }
                        result = serializer.Serialize(vi);
                        System.Diagnostics.Debug.WriteLine(result);
                        return result;
                    }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
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
                        double temp = Math.Floor(detail.price);
                        String pri = Convert.ToDecimal(temp).ToString("#,##0");

                        vi.price = pri;
                        vi.name = detail.productName.Trim();

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
            List<String> result = new List<string>();
            try
            {
                allname = am.ThienNga_FindProductName2("").ToList();

                foreach (tb_product_detail a in am.tb_product_detail.ToList())
                {
                    if (!a.productStoreID.Contains("NULL"))
                        allname.Add(a.producFactoryID);
                    allname.Add(a.productStoreID);
                }

               
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
            catch (Exception e) { }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: ProductItem/Details/5
        public ActionResult Details(int id)
        {
            ViewData["sdct"] = am.CustomerTypes.ToList();
            return View("NewProductItem");
        }
        public ActionResult Confirm()
        {
            ViewData["sdct"] = am.CustomerTypes.ToList();
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
            float total = 0;

            DateTime soldDate = DateTime.Now;
            try { soldDate = new DateTime(tuple.year, tuple.month, tuple.date);
                soldDate=soldDate.AddHours(DateTime.Now.Hour + 7);
                soldDate=soldDate.AddMinutes(DateTime.Now.Minute);
            }


            catch (Exception e) { }

            System.Diagnostics.Debug.WriteLine("HAHAHA " + soldDate.ToString());
            int inventoryID = tuple.inventoryID;
            tb_inventory_name inname = am.tb_inventory_name.Find(inventoryID);
            order ord = new order();
            String yearr = tuple.year.ToString();
            String dayy = tuple.date.ToString();
            String monthh = tuple.month.ToString();
            if (dayy.Length == 1) dayy = "0" + dayy;
            if (monthh.Length == 1) monthh = "0" + monthh;
            if (yearr.Length == 4)
            {
                yearr = yearr.Substring(2);
            }
            String ivname = "SH";
            if (inname.InventoryName.Equals("Kho Hà Nội")) { ivname = "HL"; }
            if (inname.InventoryName.Equals("Kho Tổng")) { ivname = "TB"; }
            String no = number.ToString();
            if (no.Length == 1) no = "00" + no;
            if (no.Length == 2) no = "0" + no;
            ord.MaBill = dayy + monthh + yearr + ivname;
            order oldOrder = null;
            try
            {
                oldOrder = am.orders.SqlQuery("SELECT * FROM [order] where MaBill like '%" + ord.MaBill + "%'").Last();
            }
            catch (Exception e) { }

            String temp2 = "";
            if (oldOrder == null) ord.MaBill = ord.MaBill + "001";

            else
            {
                temp2 = oldOrder.MaBill;
                temp2 = temp2.Substring(oldOrder.MaBill.Length - 3);
                int a = int.Parse(temp2);
                a = a + 1;
                temp2 = a.ToString();
                if (temp2.Length == 1) temp2 = "00" + temp2;
                if (temp2.Length == 2) temp2 = "0" + temp2;
            }
            ord.MaBill = ord.MaBill + temp2;
            if (ModelState.IsValid)
            {
                if (tuple.phoneNumber == null) tuple.phoneNumber = "ko co";
                if (tuple.Adress == null) tuple.Adress = "ko co";
                if (tuple.cusName == null) tuple.cusName = "ko co";
                if (tuple.Adress2 == null) tuple.Adress2 = "ko co";
                tb_customer cus = am.ThienNga_TimSDT2(tuple.phoneNumber).FirstOrDefault();
                if (cus == null)
                {
                    cus = new tb_customer();
                    cus.customerName = tuple.cusName;
                    cus.phonenumber = tuple.phoneNumber;
                    cus.address = tuple.Adress;
                    cus.address2 = tuple.Adress2;
                    if (tuple.Email != null)
                        cus.Email = tuple.Email;
                    am.tb_customer.Add(cus);
                    am.SaveChanges();


                }
                ord.date = soldDate;
                System.Diagnostics.Debug.WriteLine("HAHAHA 2 " + ord.date.ToString());
                int TEMP = am.items.ToList().Count() + 2;


                foreach (AnOrderDetail ao in tuple.items)
                {
                    if (ao != null)
                    {
                        total = total + (float)Math.Floor(ao.thanhTien);
                        if (ao.thanhTien < 10 && ao.thanhTienS != null)
                        {
                            String temppppp = ao.thanhTienS;
                            while (temppppp.IndexOf(",") > 0)
                            {
                                temppppp = temppppp.Replace(",", "");

                            }
                            try
                            {
                                total = total + float.Parse(temppppp);
                            }
                            catch (Exception e) { }
                        }

                    }
                }
                ord.total = total;

                String tt = Convert.ToDecimal(total).ToString("#,##0");
                String vat10 = Convert.ToDecimal(total * 0.1).ToString("#,##0");
                String tt11 = Convert.ToDecimal(total * 1.1).ToString("#,##0");
                ord.customerID = cus.id;
                am.orders.Add(ord);
                am.SaveChanges();

                lstOrderID.Add(ord.id);
                foreach (AnOrderDetail ao in tuple.items)
                {
                    if (ao.SKU != null || ao.newSKU != null)
                    {
                        tb_product_detail pd = null;
                        if (ao.SKU != null)
                        {
                            if (ao.SKU.Trim().Length > 0)
                                pd = am.ThienNga_FindProduct2(ao.SKU).FirstOrDefault();
                        }

                        if (pd != null)
                        {
                            inventory ivenQuantity
                                 = am.inventories.SqlQuery("SELECT * FROM inventory WHERE productStoreCode='" + pd.productStoreID + "' and inventoryID=" + inventoryID).FirstOrDefault();
                            ivenQuantity.quantity = ivenQuantity.quantity - ao.quantity;
                            am.SaveChanges();
                            orderDetail detail = new orderDetail();
                            detail.ChietKhauPhanTram = ao.chietKhauPhanTram + "";
                            detail.ChietKhauTrucTiep = ao.chietKhauTrucTiep + "";
                            detail.Quantity = ao.quantity + "";
                            detail.SoLuong = ao.quantity + "";
                            detail.warrantyAvailable = ao.warrantyAvailable;
                            detail.orderID = ord.id;
                            detail.productDetailID = ao.SKU;
                            detail.QuiCach = ao.quicach;
                            am.orderDetails.Add(detail);
                            am.SaveChanges();
                            lstOrderDetaiLID.Add(detail.id);
                            for (int i = 0; i < ao.quantity; i++)
                            {
                                item it = new item();
                                it.warrantyAvailable = ao.warrantyAvailable;
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
                                String subphone = "000000";
                                if (cus.phonenumber.Length > 6) subphone = cus.phonenumber.Substring(cus.phonenumber.Length - 6);
                                it.productID = ord.MaBill + "." + pd.productStoreID + "." + subphone + "." + i;
                                it.DateOfSold = soldDate;
                                if (am.CustomerTypes.Find(tuple.custype) != null)
                                {
                                    it.CustomerType = tuple.custype;
                                    it.CustomerType1 = am.CustomerTypes.Find(tuple.custype);
                                }


                                am.items.Add(it);
                                am.SaveChanges();
                                lstItemID.Add(it.id);
                                ao.productID = ord.MaBill + "." + pd.productStoreID + "." + subphone + ".(" + i + ")";
                                if (pd.productName != null)
                                {
                              
                                        ao.productName = pd.productName;
                                }
                                ao.thanhTienS = Convert.ToDecimal(ao.thanhTien).ToString("#,##0");
                                ao.DonGiaS = Convert.ToDecimal((ao.thanhTien / ao.quantity)).ToString("#,##0");
                                ao.chietKhauTrucTiepS = Convert.ToDecimal(ao.chietKhauTrucTiep).ToString("#,##0");

                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("AAAAAAAAA 2");
                            if (ao.newSKU != null)
                                if (ao.newSKU.Trim().Length > 0)
                                {
                                    //  inventory ivenQuantity
                                    //       = am.inventories.SqlQuery("SELECT * FROM inventory WHERE productStoreCode='" + pd.productStoreID + "' and inventoryID=" + inventoryID).FirstOrDefault();
                                    //   ivenQuantity.quantity = ivenQuantity.quantity - ao.quantity;
                                    //   am.SaveChanges();
                                    orderDetail detail = new orderDetail();
                                    detail.ChietKhauPhanTram = ao.chietKhauPhanTram + "";
                                    detail.ChietKhauTrucTiep = ao.chietKhauTrucTiep + "";
                                    detail.Quantity = ao.quantity + "";
                                    detail.SoLuong = ao.quantity + "";
                                    detail.orderID = ord.id;
                                    detail.warrantyAvailable = ao.warrantyAvailable;
                                    detail.productDetailID = "00000000";
                                    detail.DonGia = ao.DonGiaS2;
                                    detail.ThanhTien = ao.thanhTienS;
                                    detail.TempName = ao.newSKU;
                                    am.orderDetails.Add(detail);
                                    am.SaveChanges();
                                    lstOrderDetaiLID.Add(detail.id);
                                    for (int i = 0; i < ao.quantity; i++)
                                    {
                                        item it = new item();
                                        it.warrantyAvailable = ao.warrantyAvailable;
                                        it.customerID = cus.id;
                                        it.orderID = ord.id;
                                        it.inventoryID = inventoryID;
                                        it.productDetailID = 499;
                                        it.tempname = ao.newSKU;

                                        String day = DateTime.Today.Day + ""; if (day.Length == 1) day = "0" + day;
                                        String month = DateTime.Today.Month + ""; if (month.Length == 1) month = "0" + month;
                                        String year = DateTime.Today.Year + ""; if (year.Length == 1) year = "0" + year;
                                        String hour = DateTime.Now.Hour + ""; if (hour.Length == 1) hour = "0" + hour;
                                        String minute = DateTime.Now.Minute + ""; if (minute.Length == 1) minute = "0" + minute;
                                        String second = DateTime.Now.Second + ""; if (second.Length == 1) second = "0" + second;
                                        String subphone = "000000";
                                        if (cus.phonenumber.Length > 6) subphone = cus.phonenumber.Substring(cus.phonenumber.Length - 6);

                                        it.productID = ord.MaBill + "." + "******" + "." + subphone + "." + TEMP;
                                        TEMP = TEMP + 1;
                                        it.DateOfSold = soldDate;
                                        if (am.CustomerTypes.Find(tuple.custype) != null)
                                        {
                                            it.CustomerType = tuple.custype;
                                            it.CustomerType1 = am.CustomerTypes.Find(tuple.custype);
                                        }


                                        am.items.Add(it);
                                        am.SaveChanges();
                                        lstItemID.Add(it.id);
                                        ao.productName = ao.newSKU;
                                        ao.productID = ord.MaBill + "." + "***" + "." + subphone + ".(" + i + ")";
                                        ao.quantity = ao.quantity;
                                        ao.thanhTienS = ao.thanhTienS;
                                       
                                        //ao.DonGiaS = Convert.ToDecimal((ao.thanhTien / ao.quantity)).ToString("#,##0.00");
                                        ao.chietKhauTrucTiepS = Convert.ToDecimal(ao.chietKhauTrucTiepS).ToString("#,##0");
                                        System.Diagnostics.Debug.WriteLine("AAAAAAAAA ");
                                        System.Diagnostics.Debug.WriteLine("AAAAAAAAA " + ao.newSKU + "  " + ao.productID);
                                    
                                    }
                                }
                        }
                    }
                }
                //foreach (tb_warranty war in warranties) {
                //    am.tb_warranty.Add(war);
                //}
                if (tuple.phoneNumber.Equals("ko co")) tuple.phoneNumber = "";
                if (tuple.Adress.Equals("ko co")) tuple.Adress = "";
                if (tuple.cusName.Equals("ko co")) tuple.cusName = "";
                if (tuple.Adress2.Equals("ko co")) tuple.Adress2 = "";
                TempData["tuple"] = tuple;
                TempData["totalVAT"] = vat10;
                TempData["TotalAfterVAT"] = tt11;
                TempData["total"] = tt;
                if (tuple.VAT)
                {
                    TempData["VAT"] = "true";
                }
                else TempData["VAT"] = "false";
                Session["oderID"] = lstOrderID;
                Session["orderDetailID"] = lstOrderDetaiLID;
                Session["itemID"] = lstItemID;


                return RedirectToAction("confirmNewItem");
            }


            ViewData["sdct"] = am.CustomerTypes.ToList();
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
            ViewData["sdct"] = am.CustomerTypes.ToList();
            return View("NewProductItem", new NewItemViewModel());
        }


        public void BaoGia(String dataString, bool covat)
        {
            String[] rex1 = new string[] { ":eachrow" };
            String[] rex2 = new string[] { ":split" };
            String[] rows = dataString.Split(rex1, StringSplitOptions.None);
            float totalprice = 0;

            String MaBill = "";
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[13] {
                            new DataColumn("SKU", typeof(string)),
                            new DataColumn("Mã tạm", typeof(string)),
                            new DataColumn("Tên sản phẩm", typeof(string)),
                            new DataColumn("Tên sản phẩm2", typeof(string)),
                            new DataColumn("Tên sản phẩm3", typeof(string)),
                            new DataColumn(" Số lượng", typeof(string)),
                            new DataColumn(" Qui Cách", typeof(string)),
                            new DataColumn(" Đơn giá", typeof(string)),
                            new DataColumn("CK %", typeof(string)),
                            new DataColumn("CKTM", typeof(string)),
                            new DataColumn("  Thành tiền", typeof(string)),
                            new DataColumn("  Thành tiền2", typeof(string)),
                            new DataColumn("Bảo hành", typeof(string))});
            String cusname = "";
            String add = "";
            String sdt = "";
            String add2 = "";
            try
            {
                cusname = rows[0].Split(rex2, StringSplitOptions.None)[1];
                add = rows[2].Split(rex2, StringSplitOptions.None)[1];
                sdt = rows[1].Split(rex2, StringSplitOptions.None)[1];
                add2 = rows[3].Split(rex2, StringSplitOptions.None)[1];
            }
            catch (Exception e)
            {

            }

            for (int i = 4; i < rows.Length; i++)
            {
                String[] temp2;
                System.Diagnostics.Debug.WriteLine(rows[i]);
                try
                {
                  
                    temp2 = rows[i].Split(rex2, StringSplitOptions.None);
       
                    if (temp2.Length > 5)
                    {
                        if (temp2[4].Trim().Length > 0 && temp2[2].Trim().Length > 0 && temp2[3].Trim().Length > 0)
                            dt.Rows.Add( temp2[0],temp2[1], temp2[2],"","" ,temp2[3], temp2[4], temp2[5], temp2[6], temp2[7], temp2[8],"", temp2[9]);
                        System.Diagnostics.Debug.WriteLine(temp2[0]);
                        System.Diagnostics.Debug.WriteLine(temp2[1]);
                        System.Diagnostics.Debug.WriteLine(temp2[2]);
                        System.Diagnostics.Debug.WriteLine(temp2[3]);
                        System.Diagnostics.Debug.WriteLine(temp2[4]);
                        System.Diagnostics.Debug.WriteLine(temp2[5]);
                        System.Diagnostics.Debug.WriteLine(temp2[6]);
                        System.Diagnostics.Debug.WriteLine(temp2[7]);
                        String price = temp2[8];
                      
                        while (price.IndexOf(",") > 1)
                        {
                            price = price.Replace(",", "");
                        }

                        totalprice = float.Parse(price) + totalprice;

                    }
                }
                catch (Exception e) { }



            }
            if (add.Equals("ko co")) add = "";
            if (cusname.Equals("ko co")) cusname = "";
            if (sdt.Equals("ko co")) sdt = "";
            String total = Convert.ToDecimal(totalprice).ToString("#,##0");
            String vatt = Convert.ToDecimal(totalprice * 0.1).ToString("#,##0");
            String vattt = Convert.ToDecimal(totalprice * 1.1).ToString("#,##0");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' colspan = '2'><b>PHIẾU BÁO GIÁ</b></td></tr>");
                    sb.Append("<tr><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now.AddHours(7));
                    sb.Append(" </td></tr>");
            
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Địa chỉ giao hàng </b>");
                    sb.Append(add);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Ghi chú thêm</b>");
                    sb.Append(add2);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table border = '1' style='width: 100 %;'");
                    sb.Append("<tr>");
                    int index = 0;
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (index != 1 && index != 3 && index != 4 && index != 11)
                        {
                            if (index == 2) { sb.Append("<th colspan='3'> <font size='2' >"); }
                            else
                            if (index == 10) { sb.Append("<th colspan='2'>  <font size='2'>"); }
                            else
                            if (index == 0) { sb.Append("<th colspan='2'>  <font size='2'>"); }
                            else
                            {
                                sb.Append("<th style='width:1px; '> <font size='2'>");
                            }


                            sb.Append(column.ColumnName);
                            sb.Append(" </font></th>");
                        }
                        index = index + 1;
                    }
                    sb.Append("</tr>");

                    foreach (DataRow row in dt.Rows)
                    {
                        int index2 = 0;
                        sb.Append("<tr>");
                        String temp = "";

                        if (! ((String)row[dt.Columns[1]]).Equals("-")) {
                            temp = row[dt.Columns[1]] + "";
                        }
                        foreach (DataColumn column in dt.Columns)
                        {
                           
                          
                            if (index2 != 1 && index2 != 3 && index2 != 4 && index2 != 11) {
                                if (!temp.Equals("") && (index2 == 0 || index2==1 || index2==2))
                                {
                                    if (index2 == 0) {
                                        sb.Append("<td align='center' colspan='5'>");
                                        sb.Append(" <font size='1'>");
                                
                                            sb.Append( "(Mã tạm) " +temp);
                                        sb.Append(" </font>");
                                        sb.Append("</td>");
                                    }
                               
                                }
                                else
                                {
                                    if (index2 == 2) {
                                        sb.Append("<td colspan='3'>");
                                    }
                                    else if (index2 == 10)
                                    {
                                        sb.Append("<td colspan='2'>");
                                    }
                                    else if (index2 == 0)
                                    {
                                        sb.Append("<td colspan='2'>");
                                    }
                                    else
                                    {
                                        sb.Append("<td>");
                                    }


                                    sb.Append(" <font size='1'>");
                                    if (index2 == 2 && !temp.Equals("")) { sb.Append(temp); }
                                    else
                                        sb.Append(row[column]);
                                    sb.Append(" </font>");
                                    sb.Append("</td>");
                                }

                               
                            }
                  
                            index2 = index2 + 1;
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 3);
                    sb.Append("'>TỔNG CỘNG</td>");
                    sb.Append("<td colspan = '3' >");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    if (covat) {
                        sb.Append("<tr><td align = 'right' colspan = '");
                        sb.Append(dt.Columns.Count - 1);
                        sb.Append("'>VAT 10%</td>");
                        sb.Append("<td>");
                        sb.Append(vatt + "");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr><td align = 'right' colspan = '");
                        sb.Append(dt.Columns.Count - 1);
                        sb.Append("'>Tổng thanh toán</td>");
                        sb.Append("<td>");
                        sb.Append(vattt + "");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }

                    sb.Append(" </table>");
                    sb.Append("<p> Ghi chú: </p>");
                    sb.Append("<p> -Đơn giá trên có giá trị trong vòn 7 ngày kể từ ngày lập báo giá </p>");
                    sb.Append("<p> -Hóa đơn đỏ được giao cho khách hàng sau ngày 20 của mỗi tháng </p>");
                    sb.Append("<p> -Các sản phẩm được bảo hành theo chính sách của từng hãng khác nhau </p>");
                    sb.Append("<p>vui lòng tham khảo file phụ lục hoặc trên website của chúng tôi </p>");
                    sb.Append("<p> -Mọi thắc mắc hay yêu cầu khác, quý khách vui long liên hệ bộ phận </p>");
                    sb.Append("<p>sale qua số điện thoại 08.3835.3962 </p>");
                    //Export HTML String as PDF.
                    Encoding encoding = Encoding.Unicode;
                    var bytes = encoding.GetBytes(sb.ToString());
                    string str = System.Text.Encoding.Unicode.GetString(bytes);
                    StringReader sr = new StringReader(str);
                    Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);

                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    FontFactory.Register(Server.MapPath("~/fonts/arial-unicode-ms.ttf"), "Arial Unicode MS");
                    StyleSheet style = new StyleSheet();
                    style.LoadTagStyle("body", "face", "Arial Unicode MS");
                    style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                    htmlparser.Style = style;
                    htmlparser.StartDocument();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentEncoding = Encoding.Unicode;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=PhieuBaoGia_" + MaBill + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
        // POST: ProductItem/Delete/5
        public void GenerateInvoicePDF(String dataString)
        {
            String[] rex1 = new string[] { ":eachrow" };
            String[] rex2 = new string[] { ":split" };
            String[] rows = dataString.Split(rex1, StringSplitOptions.None);
            float totalprice = 0;
            //Dummy data for Invoice (Bill).

            String MaBill = "";


            DataTable dt = new DataTable();
            String cusname = "";
            String add = "";
            String add2 = "";
            String sdt = "";
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
                add2 = rows[4].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }


            dt.Columns.AddRange(new DataColumn[9] {
                            new DataColumn("Mã xuất kho", typeof(string)),
                            new DataColumn("Tên sản phẩm", typeof(string)),
                            new DataColumn(" Số lượng", typeof(string)),
                            new DataColumn(" Qui Cách", typeof(string)),
                            new DataColumn(" Đơn giá", typeof(string)),
                            new DataColumn("CK %", typeof(string)),
                            new DataColumn("CKTM", typeof(string)),
                            new DataColumn("  Thành tiền", typeof(string)),
                            new DataColumn("Bảo hành", typeof(string))});
            int indextemp = 0;

            for (int i = 6; i < rows.Length; i++)
            {
                String[] temp2;
                System.Diagnostics.Debug.WriteLine(rows[i]);
                try
                {
                    temp2 = rows[i].Split(rex2, StringSplitOptions.None);
              
                    if (temp2.Length > 5)
                    {
                        if (temp2[4].Trim().Length > 0 && temp2[2].Trim().Length > 0 && temp2[3].Trim().Length > 0)
                            dt.Rows.Add(temp2[1], temp2[2], temp2[3], temp2[4], temp2[5], temp2[6], temp2[7], temp2[8], temp2[9]);
                        String price = temp2[8];
                        MaBill = temp2[1].Substring(0, temp2[1].IndexOf("."));
                        while (price.IndexOf(",") > 1)
                        {
                            price = price.Replace(",", "");
                        }

                        totalprice = float.Parse(price) + totalprice;
                    }
                }
                catch (Exception e) { }



            }
            String total = Convert.ToDecimal(totalprice).ToString("#,##0");
            if (add.Equals("ko co")) add = "";
            if (cusname.Equals("ko co")) cusname = "";
            if (sdt.Equals("ko co")) sdt = "";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center'  colspan = '2'><b>PHIẾU BÁO GIÁ</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã bill: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now.AddHours(7));
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Địa chỉ giao hàng </b>");
                    sb.Append(add);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Ghi chú thêm </b>");
                    sb.Append(add2);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");
                    int trIndex = 0;
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (trIndex < 2)
                        {
                            sb.Append("<th width='18.75%'>");
                        }
                        else if (trIndex >= 2 && trIndex < 4)
                        {
                            sb.Append("<th width='6.25%'>");
                        }
                        else if (trIndex >= 4)
                        {
                            sb.Append("<th width='12.5%'>");
                        }
                     
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                        trIndex = trIndex + 1;
                    }
                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        int index = 0;
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (index < 2)
                            {
                                sb.Append("<td width='18.75%'>");
                            }
                            else if (index >= 2 && index < 4)
                            {
                                sb.Append("<td width='6.25%'>");
                            }
                            else if (index >= 4) {
                                sb.Append("<td width='12.5%'>");
                            }
                            System.Diagnostics.Debug.WriteLine("CCCCCCCC " + index);
                            sb.Append(row[column]);
                            sb.Append("</td>");
                            index = index + 1;
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>Tổng</td>");
                    sb.Append("<td>");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr></table>");
                    sb.Append("<p> Ghi chú: </p>");
                    sb.Append("<p> -Đơn giá trên có giá trị trong vòn 7 ngày kể từ ngày lập báo giá </p>");
                    sb.Append("<p> -Hóa đơn đỏ được giao cho khách hàng sau ngày 20 của mỗi tháng </p>");
                    sb.Append("<p> -Các sản phẩm được bảo hành theo chính sách của từng hãng khác nhau </p>");
                    sb.Append("<p>vui lòng tham khảo file phụ lục hoặc trên website của chúng tôi </p>");
                    sb.Append("<p> -Mọi thắc mắc hay yêu cầu khác, quý khách vui long liên hệ bộ phận </p>");
                    sb.Append("<p>sale qua số điện thoại 08.3835.3962 </p>");
                    //Export HTML String as PDF.
                    Encoding encoding = Encoding.Unicode;
                    var bytes = encoding.GetBytes(sb.ToString());
                    string str = System.Text.Encoding.Unicode.GetString(bytes);
                    StringReader sr = new StringReader(str);
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    FontFactory.Register(Server.MapPath("~/fonts/arial-unicode-ms.ttf"), "Arial Unicode MS");
                    StyleSheet style = new StyleSheet();
                    style.LoadTagStyle("body", "face", "Arial Unicode MS");
                    style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                    htmlparser.Style = style;
                    htmlparser.StartDocument();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentEncoding = Encoding.Unicode;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=PhieuBaoGia_" + MaBill + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
        public void GenerateInvoicePDF2(String dataString)
        {
            String[] rex1 = new string[] { ":eachrow" };
            String[] rex2 = new string[] { ":split" };
            String[] rows = dataString.Split(rex1, StringSplitOptions.None);
            float totalprice = 0;

            String MaBill = "";
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[9] {
                            new DataColumn("Mã xuất kho", typeof(string)),
                            new DataColumn("Tên sản phẩm", typeof(string)),
                            new DataColumn(" Số lượng", typeof(string)),
                            new DataColumn(" Qui Cách", typeof(string)),
                            new DataColumn(" Đơn giá", typeof(string)),
                            new DataColumn("CK %", typeof(string)),
                            new DataColumn("CKTM", typeof(string)),
                            new DataColumn("  Thành tiền", typeof(string)),
                            new DataColumn("Bảo hành", typeof(string))});
            String cusname = "";
            String add = "";
            String sdt = "";
            String add2 = "";
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
                add2 = rows[4].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }

            for (int i = 6; i < rows.Length; i++)
            {
                String[] temp2;
                System.Diagnostics.Debug.WriteLine(rows[i]);
                try
                {
                    temp2 = rows[i].Split(rex2, StringSplitOptions.None);
                    for (int eee = 0; eee < temp2.Length; eee++)
                    {

                    }
                    if (temp2.Length > 5)
                    {
                        if (temp2[4].Trim().Length > 0 && temp2[2].Trim().Length > 0 && temp2[3].Trim().Length > 0)
                            dt.Rows.Add(temp2[1], temp2[2], temp2[3], temp2[4], temp2[5], temp2[6], temp2[7], temp2[8], temp2[9]);
                        String price = temp2[8];
                        MaBill = temp2[1].Substring(0, temp2[1].IndexOf("."));
                        while (price.IndexOf(",") > 1)
                        {
                            price = price.Replace(",", "");
                        }

                        totalprice = float.Parse(price) + totalprice;

                    }
                }
                catch (Exception e) { }



            }
            if (add.Equals("ko co")) add = "";
            if (cusname.Equals("ko co")) cusname = "";
            if (sdt.Equals("ko co")) sdt = "";
            String total = Convert.ToDecimal(totalprice).ToString("#,##0");
            String vatt = Convert.ToDecimal(totalprice * 0.1).ToString("#,##0");
            String vattt = Convert.ToDecimal(totalprice * 1.1).ToString("#,##0");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' colspan = '2'><b>PHIẾU BÁO GIÁ</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã bill: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now.AddHours(7));
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Địa chỉ giao hàng </b>");
                    sb.Append(add);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Ghi chú thêm</b>");
                    sb.Append(add2);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th style = 'background-color: #000;color:#ffffff'>");
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        int index = 0;
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (index < 2)
                            {
                                sb.Append("<td width='18.75%'>");
                            }
                            else if (index >= 2 && index < 4)
                            {
                                sb.Append("<td width='6.25%'>");
                            }
                            else if (index >= 4)
                            {
                                sb.Append("<td width='12.5%'>");
                            }
                            index = index + 1;
                 
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>TỔNG CỘNG/td>");
                    sb.Append("<td>");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>VAT 10%</td>");
                    sb.Append("<td>");
                    sb.Append(vatt + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>Tổng thanh toán</td>");
                    sb.Append("<td>");
                    sb.Append(vattt + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append(" </table>");
                    sb.Append("<p> Ghi chú: </p>");
                    sb.Append("<p> -Đơn giá trên có giá trị trong vòn 7 ngày kể từ ngày lập báo giá </p>");
                    sb.Append("<p> -Hóa đơn đỏ được giao cho khách hàng sau ngày 20 của mỗi tháng </p>");
                    sb.Append("<p> -Các sản phẩm được bảo hành theo chính sách của từng hãng khác nhau </p>");
                    sb.Append("<p>vui lòng tham khảo file phụ lục hoặc trên website của chúng tôi </p>");
                    sb.Append("<p> -Mọi thắc mắc hay yêu cầu khác, quý khách vui long liên hệ bộ phận </p>");
                    sb.Append("<p>sale qua số điện thoại 08.3835.3962 </p>");
                    //Export HTML String as PDF.
                    Encoding encoding = Encoding.Unicode;
                    var bytes = encoding.GetBytes(sb.ToString());
                    string str = System.Text.Encoding.Unicode.GetString(bytes);
                    StringReader sr = new StringReader(str);
                    Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);

                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    FontFactory.Register(Server.MapPath("~/fonts/arial-unicode-ms.ttf"), "Arial Unicode MS");
                    StyleSheet style = new StyleSheet();
                    style.LoadTagStyle("body", "face", "Arial Unicode MS");
                    style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                    htmlparser.Style = style;
                    htmlparser.StartDocument();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentEncoding = Encoding.Unicode;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=PhieuBaoGia_" + MaBill + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
        public void GenerateInvoiceBill2(String dataString)
        {
            float totalheigh = 100;
            String[] rex1 = new string[] { ":eachrow" };
            String[] rex2 = new string[] { ":split" };
            String[] rows = dataString.Split(rex1, StringSplitOptions.None);
            float totalprice = 0;

            String MaBill = "";
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[3] {

                            new DataColumn("Tên sản phẩm/Số lượng", typeof(string)),

                            new DataColumn(" Đơn giá", typeof(string)),

                            new DataColumn("  Thành tiền", typeof(string))});

            String cusname = "";
            String add = "";
            String sdt = "";
            String add2 = "";
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                add2 = rows[4].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }

            for (int i = 6; i < rows.Length; i++)
            {
                String[] temp2;
                System.Diagnostics.Debug.WriteLine(rows[i]);
                try
                {
                    temp2 = rows[i].Split(rex2, StringSplitOptions.None);
                    for (int eee = 0; eee < temp2.Length; eee++)
                    {

                    }
                    if (temp2.Length > 5)
                    {
                        if (temp2[4].Trim().Length > 0 && temp2[2].Trim().Length > 0 && temp2[3].Trim().Length > 0)
                        {
                            dt.Rows.Add(temp2[2]);

                            dt.Rows.Add(temp2[3], temp2[5], temp2[8]);
                            totalheigh = totalheigh + 20;
                        }
                        String price = temp2[8];
                        MaBill = temp2[1].Substring(0, temp2[1].IndexOf("."));
                        while (price.IndexOf(",") > 1)
                        {
                            price = price.Replace(",", "");
                        }

                        totalprice = float.Parse(price) + totalprice;

                    }
                }
                catch (Exception e) { }



            }
            String total = Convert.ToDecimal(totalprice).ToString("#,##0");
            String vatt = Convert.ToDecimal(totalprice * 0.1).ToString("#,##0");
            String vattt = Convert.ToDecimal(totalprice * 1.1).ToString("#,##0");
            if (add.Equals("ko co")) add = "";
            if (cusname.Equals("ko co")) cusname = "";
            if (sdt.Equals("ko co")) sdt = "";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center'  colspan = '2'><b>PHIẾU BÁO GIÁ</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã bill: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now.AddHours(7));
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Địa chỉ giao hàng</b>");
                    sb.Append(add);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Ghi chú thêm</b>");
                    sb.Append(add2);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table cellpadding='1' >");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th style = 'background-color: #000;color:#ffffff'>");
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    int merge = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (merge % 2 == 0)
                                sb.Append("<td height='1' colspan='3'><font size='2'>");
                            else
                            {
                                sb.Append("<td height='1'> <font size='2'>");
                            }
                            sb.Append(row[column]);
                            sb.Append("</font></td>");
                        }
                        sb.Append("</tr>");
                        merge = merge + 1;
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append("1");
                    sb.Append("'>TỔNG CỘNG</td>");
                    sb.Append("<td  align='right' colspan = '2'>");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(1 + "");
                    sb.Append("'>VAT 10%</td>");
                    sb.Append("<td align='right' colspan = '2'>");
                    sb.Append(vatt + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append("1");
                    sb.Append("'>Tổng thanh toán</td>");
                    sb.Append("<td  align='right' colspan = '2'>");
                    sb.Append(vattt + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append(" </table>");

                    //Export HTML String as PDF.
                    Encoding encoding = Encoding.Unicode;
                    var bytes = encoding.GetBytes(sb.ToString());
                    string str = System.Text.Encoding.Unicode.GetString(bytes);
                    StringReader sr = new StringReader(str);

                    Utilities.MillimetersToPoints(78f);


                    Document pdfDoc = new Document(new Rectangle(Utilities.MillimetersToPoints(78f), Utilities.MillimetersToPoints(totalheigh)), 0, 0, 0, 0);

                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    FontFactory.Register(Server.MapPath("~/fonts/arial-unicode-ms.ttf"), "Arial Unicode MS");
                    StyleSheet style = new StyleSheet();
                    style.LoadTagStyle("body", "face", "Arial Unicode MS");
                    style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                    htmlparser.Style = style;
                    htmlparser.StartDocument();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentEncoding = Encoding.Unicode;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=PhieuBaoGia_" + MaBill + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
        public void GenerateInvoiceBill(String dataString)
        {
            String[] rex1 = new string[] { ":eachrow" };
            String[] rex2 = new string[] { ":split" };
            String[] rows = dataString.Split(rex1, StringSplitOptions.None);
            float totalprice = 0;
            float totalheigh = 100;
            String MaBill = "";
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[3] {

                            new DataColumn("Tên sản phẩm/Số lượng", typeof(string)),

                            new DataColumn(" Đơn giá", typeof(string)),

                            new DataColumn("  Thành tiền", typeof(string))});

            String cusname = "";
            String add = "";
            String sdt = "";
            String add2 = "";
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                add2 = rows[4].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }

            for (int i = 6; i < rows.Length; i++)
            {
                String[] temp2;
                System.Diagnostics.Debug.WriteLine(rows[i]);
                try
                {
                    temp2 = rows[i].Split(rex2, StringSplitOptions.None);
                    for (int eee = 0; eee < temp2.Length; eee++)
                    {

                    }
                    if (temp2.Length > 5)
                    {
                        if (temp2[4].Trim().Length > 0 && temp2[2].Trim().Length > 0 && temp2[3].Trim().Length > 0)
                        {
                            dt.Rows.Add(temp2[2]);

                            dt.Rows.Add(temp2[3], temp2[5], temp2[8]);
                            totalheigh = totalheigh + 20;
                        }
                        String price = temp2[8];
                        MaBill = temp2[1].Substring(0, temp2[1].IndexOf("."));
                        while (price.IndexOf(",") > 1)
                        {
                            price = price.Replace(",", "");
                        }

                        totalprice = float.Parse(price) + totalprice;

                    }
                }
                catch (Exception e) { }



            }
            String total = Convert.ToDecimal(totalprice).ToString("#,##0");
            String vatt = Convert.ToDecimal(totalprice * 0.1).ToString("#,##0");
            String vattt = Convert.ToDecimal(totalprice * 1.1).ToString("#,##0");
            if (add.Equals("ko co")) add = "";
            if (cusname.Equals("ko co")) cusname = "";
            if (sdt.Equals("ko co")) sdt = "";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' colspan = '2'><b>PHIẾU BÁO GIÁ</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã bill: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now.AddHours(7));
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Địa chỉ giao hàng </b>");
                    sb.Append(add);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Ghi chú thêm </b>");
                    sb.Append(add2);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table cellpadding='1' >");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th style = 'background-color: #000;color:#ffffff'>");
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    int merge = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (merge % 2 == 0)
                                sb.Append("<td height='1' colspan='3'><font size='2'>");
                            else
                            {
                                sb.Append("<td height='1'> <font size='2'>");
                            }
                            sb.Append(row[column]);
                            sb.Append("</font></td>");
                        }
                        sb.Append("</tr>");
                        merge = merge + 1;
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append("1");
                    sb.Append("'>TỔNG CỘNG</td>");
                    sb.Append("<td align='right' colspan = '2'>");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                    sb.Append(" </table>");

                    //Export HTML String as PDF.
                    Encoding encoding = Encoding.Unicode;
                    var bytes = encoding.GetBytes(sb.ToString());
                    string str = System.Text.Encoding.Unicode.GetString(bytes);
                    StringReader sr = new StringReader(str);
                    Document pdfDoc = new Document(new Rectangle(Utilities.MillimetersToPoints(78), Utilities.MillimetersToPoints(totalheigh)), 0, 0, 0, 0);

                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    FontFactory.Register(Server.MapPath("~/fonts/arial-unicode-ms.ttf"), "Arial Unicode MS");
                    StyleSheet style = new StyleSheet();
                    style.LoadTagStyle("body", "face", "Arial Unicode MS");
                    style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                    htmlparser.Style = style;
                    htmlparser.StartDocument();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentEncoding = Encoding.Unicode;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=PhieuBaoGia_" + MaBill + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
    }



}
