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
        private int number =1;
        // GET: ProductItem
        public ActionResult Index()
        {

            ViewData["dsk"] = am.tb_inventory_name.ToList();

            ViewData["sdct"] = am.CustomerTypes.ToList();
            return View("NewProductItem", new NewItemViewModel());

        }
        public String getdataKhachHang(String sdt) {
            cusInfo cus = new cusInfo();
            try {
                tb_customer findcus = am.tb_customer.SqlQuery("SELECT * FROM tb_customer WHERE phonenumber='" + sdt+"'").FirstOrDefault();
                if (findcus != null) {
                    cus.cusadd = findcus.address;
                    cus.cusadd2 = findcus.address2;
                    cus.cusname = findcus.customerName;
                    cus.cussdt = findcus.phonenumber;
                    
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                   return serializer.Serialize(cus);
                }
            }
            catch (Exception e) {

            }
            return "";
        }
        public String getAllData(String name) {
            try {
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
                            vi.name = lst.ElementAt(0).productName.Substring(0, 16);

                            vi.price = lst.ElementAt(0).price + "";
                        }
                        result = serializer.Serialize(vi);
                        System.Diagnostics.Debug.WriteLine(result);
                        return result;
                    }
            }
            catch (Exception e) {
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
            try { soldDate = new DateTime(tuple.year, tuple.month, tuple.date); }
            catch (Exception e) { }
            int inventoryID = tuple.inventoryID;
            tb_inventory_name inname = am.tb_inventory_name.Find(inventoryID);
            order ord = new order();
            String yearr = tuple.year.ToString();
            String dayy = tuple.date.ToString();
            String monthh = tuple.month.ToString();
            if (dayy.Length == 1) dayy = "0" + dayy;
            if (monthh.Length == 1) monthh = "0" + monthh;
            if (yearr.Length == 4) {
                yearr = yearr.Substring(2);
            }
            String ivname = "SH";
            if (inname.InventoryName.Equals("Kho Hà Nội")) { ivname = "HL"; }
            if (inname.InventoryName.Equals("Kho Tổng")) { ivname = "TB"; }
            String no = number.ToString();
            if (no.Length == 1) no = "00" + no;
            if (no.Length == 2) no = "0" + no;
            ord.MaBill = dayy + monthh + yearr + ivname ;
            order oldOrder = null;
            try {
                oldOrder = am.orders.SqlQuery("SELECT * FROM [order] where MaBill like '%" + ord.MaBill + "%'").Last();
            }
            catch (Exception e) { }
            
            String temp2 = "";
            if (oldOrder == null) ord.MaBill = ord.MaBill + "001";

            else {
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
                tb_customer cus = am.ThienNga_TimSDT2(tuple.phoneNumber).FirstOrDefault();
                if (cus == null)
                {
                    cus = new tb_customer();
                    cus.customerName = tuple.cusName;
                    cus.phonenumber = tuple.phoneNumber;
                    cus.address = tuple.Adress;
                    if(tuple.Email != null)
                    cus.Email = tuple.Email;
                    am.tb_customer.Add(cus);
                    am.SaveChanges();
                  
             
                }
                ord.date = soldDate;
                int TEMP = am.items.ToList().Count() + 2;

           
                foreach (AnOrderDetail ao in tuple.items)
                {
                    total = total +  (float)Math.Floor(ao.thanhTien);
                    if (ao.thanhTien < 10 && ao.thanhTienS != null) {
                        String tempppp = ao.thanhTienS;
                        while (tempppp.IndexOf(",") > 0)
                        {
                            tempppp = tempppp.Replace(",", "");

                        }
                        try
                        {
                            total = total + float.Parse(tempppp);
                        }
                        catch (Exception e) { }
                    }

                }
                ord.total = total;

                String tt =Convert.ToDecimal(total).ToString("#,##0");
                String vat10 = Convert.ToDecimal(total * 0.1).ToString("#,##0");
                String tt11 = Convert.ToDecimal(total * 1.1).ToString("#,##0");
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
                        inventory ivenQuantity
                             = am.inventories.SqlQuery("SELECT * FROM inventory WHERE productStoreCode='" + pd.productStoreID + "' and inventoryID=" + inventoryID).FirstOrDefault();
                        ivenQuantity.quantity = ivenQuantity.quantity - ao.quantity;
                        am.SaveChanges();
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
                            ao.productName = pd.productName;
                            ao.thanhTienS = Convert.ToDecimal(ao.thanhTien).ToString("#,##0.00");
                            ao.DonGiaS = Convert.ToDecimal((ao.thanhTien / ao.quantity)).ToString("#,##0.00");
                            ao.chietKhauTrucTiepS = Convert.ToDecimal(ao.chietKhauTrucTiep).ToString("#,##0.00");

                        }
                    }
                    else {
                        System.Diagnostics.Debug.WriteLine("AAAAAAAAA 2");
                        if(ao.newSKU != null)
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
                            detail.productDetailID = "00000000";
                            am.orderDetails.Add(detail);
                            am.SaveChanges();
                            lstOrderDetaiLID.Add(detail.id);
                            for (int i = 0; i < ao.quantity; i++)
                            {
                                item it = new item();
                                it.customerID = cus.id;
                                it.orderID = ord.id;
                                it.inventoryID = inventoryID;
                                it.productDetailID =499;
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
                                ao.productID = ord.MaBill + "." + "*******"+ "." + subphone + ".(" + i + ")";
                                ao.quantity = ao.quantity;
                                ao.thanhTienS = ao.thanhTienS;
                  
                                //ao.DonGiaS = Convert.ToDecimal((ao.thanhTien / ao.quantity)).ToString("#,##0.00");
                                ao.chietKhauTrucTiepS = Convert.ToDecimal(ao.chietKhauTrucTiepS).ToString("#,##0.00");
                                System.Diagnostics.Debug.WriteLine("AAAAAAAAA ");
                                System.Diagnostics.Debug.WriteLine("AAAAAAAAA "  + ao.newSKU + "  " + ao.productID);
                            }
                        }
                    }
                }
                //foreach (tb_warranty war in warranties) {
                //    am.tb_warranty.Add(war);
                //}


                TempData["tuple"] = tuple;
                TempData["totalVAT"] = vat10;
                TempData["TotalAfterVAT"] = tt11;
                TempData["total"] = tt;
                if (tuple.VAT) {
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



        // POST: ProductItem/Delete/5
        public void GenerateInvoicePDF(String dataString)
        {
            String[] rex1= new string[] { ":eachrow" };
            String[] rex2 = new string[] { ":split" };
            String[] rows = dataString.Split(rex1, StringSplitOptions.None);
            float totalprice = 0;
            //Dummy data for Invoice (Bill).
          
            String MaBill = "";


            DataTable dt = new DataTable();
            String cusname = "";
                    String add = "";
            String sdt = "";
            try {
                 cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                 add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                 sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e) {

            }
    

            dt.Columns.AddRange(new DataColumn[7] {
                            new DataColumn("Mã", typeof(string)),
                            new DataColumn("Tên sản phẩm", typeof(string)),
                            new DataColumn(" Số lượng", typeof(string)),
                            new DataColumn(" Đơn giá", typeof(string)),
                            new DataColumn("Chiết Khấu phần trăm", typeof(string)),
                            new DataColumn("    Chiết Khấu trực tiếp  ", typeof(string)),
                            new DataColumn("  Thành tiền", typeof(string))});
            
            for (int i = 5; i < rows.Length; i++)
            {
                String[] temp2;
                System.Diagnostics.Debug.WriteLine(rows[i]);
                try {
                    temp2 = rows[i].Split(rex2, StringSplitOptions.None);
                    for (int eee = 0; eee < temp2.Length; eee++) {
                       
                    }
                    if (temp2.Length > 5) {
                        if( temp2[4].Trim().Length > 0 && temp2[2].Trim().Length > 0 && temp2[3].Trim().Length > 0)
                            dt.Rows.Add(temp2[1], temp2[2], temp2[3], temp2[4], temp2[5], temp2[6], temp2[7]);
                        String price = temp2[7];
                         MaBill= temp2[1].Substring(0, temp2[1].IndexOf("."));
                        while (price.IndexOf(",") > 1) {
                           price =price.Replace(",", "");
                        }
                   
                        totalprice = float.Parse(price) +totalprice;
                    }
                } catch (Exception e) { }
                

                
            }
            String total =Convert.ToDecimal(totalprice).ToString("#,##0.00");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Phiếu báo giá</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã số: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Dia chi giao hang </b>");
                    sb.Append(add);
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
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>Total</td>");
                    sb.Append("<td>");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr></table>");

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

            dt.Columns.AddRange(new DataColumn[7] {
                            new DataColumn("Mã xuất kho", typeof(string)),
                            new DataColumn("Tên sản phẩm", typeof(string)),
                            new DataColumn(" Số lượng", typeof(string)),
                            new DataColumn(" Đơn giá", typeof(string)),
                            new DataColumn("Chiết Khấu %", typeof(string)),
                            new DataColumn("    Chiết Khấu tt ", typeof(string)),
                            new DataColumn("  Thành tiền", typeof(string))});
            String cusname = "";
            String add = "";
            String sdt = "";
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }

            for (int i = 5; i < rows.Length; i++)
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
                            dt.Rows.Add(temp2[1], temp2[2], temp2[3], temp2[4], temp2[5], temp2[6], temp2[7]);
                        String price = temp2[7];
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
            String total = Convert.ToDecimal(totalprice).ToString("#,##0.00");
            String vatt = Convert.ToDecimal(totalprice*0.1).ToString("#,##0.00");
            String vattt = Convert.ToDecimal(totalprice * 1.1).ToString("#,##0.00");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Phiếu báo giá</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã số: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Dia chi giao hang </b>");
                    sb.Append(add);
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
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>Tong</td>");
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
                    sb.Append("'>Thanh toan</td>");
                    sb.Append("<td>");
                    sb.Append(vattt + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append(" </table>");

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
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }

            for (int i = 5; i < rows.Length; i++)
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

                            dt.Rows.Add(temp2[3], temp2[4], temp2[7]);
                            totalheigh = totalheigh + 20;
                        }
                        String price = temp2[7];
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
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Phiếu báo giá</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã số: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Dia chi giao hang </b>");
                    sb.Append(add);
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
                            if ( merge%2 == 0)
                                sb.Append("<td height='1' colspan='3'><font size='2'>");
                            else{
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
                    sb.Append("'>Tong</td>");
                    sb.Append("<td  align='right' colspan = '2'>");
                    sb.Append(total + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(1+"");
                    sb.Append("'>VAT 10%</td>");
                    sb.Append("<td align='right' colspan = '2'>");
                    sb.Append(vatt + "");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append("1");
                    sb.Append("'>Thanh toan</td>");
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
           
                
                    Document pdfDoc = new Document(new Rectangle(Utilities.MillimetersToPoints(78f),Utilities.MillimetersToPoints(totalheigh) ), 0, 0, 0, 0);

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
            try
            {
                cusname = rows[1].Split(rex2, StringSplitOptions.None)[2];
                add = rows[3].Split(rex2, StringSplitOptions.None)[2];
                sdt = rows[2].Split(rex2, StringSplitOptions.None)[2];
            }
            catch (Exception e)
            {

            }

            for (int i = 5; i < rows.Length; i++)
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

                            dt.Rows.Add(temp2[3], temp2[4], temp2[7]);
                            totalheigh = totalheigh + 20;
                        }
                        String price = temp2[7];
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
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Phiếu báo giá</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Mã số: </b>");
                    sb.Append(MaBill);
                    sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Tên khách </b>");
                    sb.Append(cusname);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                    sb.Append(sdt);
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Dia chi giao hang </b>");
                    sb.Append(add);
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
                    sb.Append("'>Tong</td>");
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
                    Document pdfDoc = new Document(new Rectangle(Utilities.MillimetersToPoints(78), Utilities.MillimetersToPoints(totalheigh) ), 0, 0, 0, 0);

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
