using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;

namespace ThienNga2.Controllers
{
    public class BaoGiaController : EntitiesAM
    {

        [Authorize(Roles = "Admin")]
        // GET: BaoGia
        public ActionResult Index()
        {
            
           
            ViewData["allBaoGia"] = am.orders.SqlQuery("Select * from [order] where total > " +0).ToList();
            return View("BaoGiaList");
        }
        public ActionResult Search(String code)
        {
            try
            {
                NewItemViewModel tuple = new NewItemViewModel();
                order ord = am.orders.Find(int.Parse(code));
                tuple.Adress = ord.tb_customer.address;
                tuple.Adress2 = ord.tb_customer.address2;
                tuple.cusName = ord.tb_customer.customerName;
                tuple.phoneNumber = ord.tb_customer.phonenumber;
                tuple.month = ord.date.Month;
                tuple.date = ord.date.Day;
                tuple.year = ord.date.Year;
                tuple.inventoryID = ord.items.First().inventoryID;

                String tt = Convert.ToDecimal(ord.total).ToString("#,##0");
                String vat10 = Convert.ToDecimal(ord.total * 0.1).ToString("#,##0");
                String tt11 = Convert.ToDecimal(ord.total * 1.1).ToString("#,##0");
                List<orderDetail> lst = am.orderDetails.SqlQuery("SELECT * FROM orderDetail WHERE orderID=" + code).ToList();
                int index = 0;
                if (lst.Count() > 0)
                {
                    ViewData["oderdetails"] = lst;
                    foreach (orderDetail orddetail in lst)
                    {
                        tuple.items[index].chietKhauPhanTram = float.Parse(orddetail.ChietKhauPhanTram);
                        tuple.items[index].chietKhauTrucTiepS =(orddetail.ChietKhauTrucTiep);
                        tb_product_detail dt = am.ThienNga_FindProduct2(orddetail.productDetailID).First();
                        tuple.items[index].chietKhauTrucTiepS = Convert.ToDecimal(tuple.items[index].chietKhauTrucTiep).ToString("#,##0");
                        tuple.items[index].SKU = orddetail.productDetailID;
                        if (orddetail.QuiCach != null)
                            tuple.items[index].quicach = orddetail.QuiCach;
                        else {
                            tuple.items[index].quicach ="Cái";
                        }
                        if (orddetail.warrantyAvailable != null)
                        {
                            tuple.items[index].warrantyAvailable = (bool)orddetail.warrantyAvailable;
                        }
                        else {
                            tuple.items[index].warrantyAvailable = true; 
                        }
                        tuple.items[index].quantity = int.Parse(orddetail.Quantity);
                        tuple.items[index].thanhTien = (float)(int.Parse(orddetail.Quantity) * dt.price);
                        tuple.items[index].dongia = (float)dt.price;
                        if (dt.id == 499) {
                            tuple.items[index].productName = orddetail.TempName;
                        
                        }
                        else
                        tuple.items[index].productName = dt.productName;
                        if (orddetail.DonGia == null && orddetail.productDetailID != "00000000")
                        {
                            tuple.items[index].DonGiaS = Convert.ToDecimal(tuple.items[index].dongia).ToString("#,##0");
                            tuple.items[index].thanhTienS = Convert.ToDecimal(tuple.items[index].thanhTien).ToString("#,##0");
     
                        }
                        else {
                            tuple.items[index].DonGiaS = orddetail.DonGia;
                            tuple.items[index].thanhTienS = orddetail.ThanhTien;
                        }
          
                        String temp = "000000";
                        if (ord.tb_customer.phonenumber.Length >= 6) temp = ord.tb_customer.phonenumber.Substring(ord.tb_customer.phonenumber.Length - 6);
                        String temp2 = "***";
                        if (!dt.productStoreID.Equals("0000000")) temp2 = dt.productStoreID;
                        tuple.items[index].productID = ord.MaBill  +"."+dt.productStoreID+  "." + temp;
                        index = index + 1;
                    }
                }
                if (tuple.phoneNumber.Equals("ko co")) tuple.phoneNumber = "";
                if (tuple.cusName.Equals("ko co")) tuple.cusName = "";
                if (tuple.Adress.Equals("ko co")) tuple.Adress = "";
                TempData["tuple"] = tuple;
                TempData["totalVAT"] = vat10;
                TempData["TotalAfterVAT"] = tt11;
                TempData["total"] = tt;
                if (ord.vat != null)
                {
                    if ((Boolean)ord.vat) tuple.VAT = true;
                    else tuple.VAT = false;
                }
             
                else tuple.VAT = false;
                if (tuple.VAT)
                {
                    TempData["VAT"] = "true";
                }
                else TempData["VAT"] = "false";
                return View("BaoGiaChiTiet", tuple);
            }
            catch (Exception e) { }
            ViewData["allBaoGia"] = am.orders.SqlQuery("Select * from [order] where total > " + 0 + " order by id desc ").ToList();
            return View("BaoGiaList");
        }
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
                            else if (index >= 4)
                            {
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
                            sb.Append("<td>");
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