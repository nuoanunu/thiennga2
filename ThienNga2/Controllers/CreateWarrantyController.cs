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
using System.Web.Script.Serialization;
using System.Web.UI;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;

namespace ThienNga2.Controllers
{

    [Authorize(Roles = "Admin,Bán hàng,Tạo Hóa Đơn Bảo Hành,Nhân Viên Quản Lý Sửa Chữa")]
    public class CreateWarrantyController : EntitiesAM
    {

        private List<String> allname = new List<String>();
        public void getAllName()
        {
            List<AspNetUser> lst = am.AspNetUsers.ToList();
            foreach (AspNetUser au in lst)
            {
                allname.Add(au.Email);
            }
        }
        public ActionResult Autocomplete(string term)
        {
            getAllName();
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
        // GET: CreateWarranty
        public ActionResult Index()
        {
            return View("CreateWarranty");
        }

        // GET: CreateWarranty/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public String getCorrectData(String name)
        {
            if (name != null)
                if (name.Trim().Length >= 1)
                {
                    List<tb_warranty> lst = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE warrantyID='" + name + "'").ToList();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string result = "";
                    HoaDonBaoHanhCheck vi = new HoaDonBaoHanhCheck();
                    if (lst.Count == 1)
                    {
                        tb_warranty war = lst.ElementAt(0);

                        item detail = am.items.SqlQuery("SELECT * FROM dbo.item WHERE id=" + war.itemID ).FirstOrDefault();
                        if (detail != null)
                        {
                            vi.mieuTa = war.description;
                            vi.productName = detail.tb_product_detail.productName;
                            DateTime date = (DateTime)war.startdate;
                            date = date.AddMonths(war.duration);
                            vi.remainingTime = date.Day + "/" + date.Month + "/" + date.Year;
                            if (war.Special == 1)
                            {
                                vi.sepcial = 1;
                            }
                            result = serializer.Serialize(vi);
                            System.Diagnostics.Debug.WriteLine(result);
                            return result;

                        }


                    }

                }
            return "";
        }
        // GET: CreateWarranty/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreateWarranty/CreateNew
        [HttpPost]
        public ActionResult CreateNew(String actid, String phoneNumber, String cusname, String IMEI, String Descrip, String Emp1, String Emp2)
        {
            tb_warranty_activities act;
            if (actid != null)
            {
                if (actid.Trim().Length > 0)
                {
                    try
                    {
                        act = am.tb_warranty_activities.Find(int.Parse(actid));
                    }
                    catch { return View("CreateWarranty"); }
                }
                else act = new tb_warranty_activities();

            }
            else
                act = new tb_warranty_activities();
            phoneNumber = phoneNumber.Trim();
            var emplo1 = am.AspNetUsers.SqlQuery("SELECT * FROM dbo.AspNetUsers WHERE Email='" + Emp1 + "'").ToList().First();
            AspNetUser emplo2 =null;
            if (Emp2 != null)
                try {
                    emplo2 = am.AspNetUsers.SqlQuery("SELECT * FROM dbo.AspNetUsers WHERE Email='" + Emp2 + "'").ToList().First();
                }
                catch (Exception e) { emplo2 = null; }
                
            act.employee = (string)emplo1.Id;
            act.AspNetUser = emplo1;
            if (emplo2 != null)
            {
                act.empFixer = (string)emplo2.Id;
                act.AspNetUser1 = emplo2;
            }
            tb_warranty lst = am.tb_warranty.SqlQuery("SELECT * FROM dbo.tb_warranty WHERE warrantyID='" + IMEI + "'").FirstOrDefault();
            item detail = am.items.SqlQuery("SELECT * FROM dbo.item WHERE id=" + lst.itemID ).FirstOrDefault();
            act.TenKhach = cusname;
            act.SDT = phoneNumber;
            act.status = 1;
            act.Description = Descrip;
            act.warrantyID = lst.id;
            act.startDate = DateTime.Today;
            String date = act.startDate.Day.ToString();
            if (date.Length == 1) date = "0" + date;
            String month = act.startDate.Month.ToString();
            if (month.Length == 1) month = "0" + month;
            act.CodeBaoHanh = date + month + "." + phoneNumber.Substring(phoneNumber.Length - 6);
          
            act.productDetailID = detail.productDetailID;
            am.tb_warranty_activities.Add(act);
            am.SaveChanges();
            int id = act.id;
            System.Diagnostics.Debug.WriteLine("ID NAY " + id);

            act.id = id;
            act = am.tb_warranty_activities.Find(id);
            return RedirectToAction("ConfirmCreate", "CreateWarranty", new { acid = id });


        }

        public ActionResult ConfirmCreate(int acid)
        {

            tb_warranty_activities newact = am.tb_warranty_activities.Find(acid);
            ViewData["newwarranty"] = newact;
            return View("ConfirmCreate");
        }
        [HttpPost]
        public ActionResult EditAct(String actid)
        {
            tb_warranty_activities a = am.tb_warranty_activities.Find(int.Parse(actid));
            ViewData["newwarranty"] = a;

            return View("CreateWarranty");
        }
        [HttpPost]
        public ActionResult Confirrm(String actid)
        {

            return RedirectToAction("Search", "Warranty", new { code = actid, searchType = "warrantyActID" });
        }
        // GET: CreateWarranty/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CreateWarranty/Edit/5
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

        // GET: CreateWarranty/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CreateWarranty/Delete/5
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
        public void GenerateInvoiceBill(String actid)
        {

            try
            {
                tb_warranty_activities act = am.tb_warranty_activities.Find(int.Parse(actid));
                String MaBill = act.CodeBaoHanh;
                String cusname = act.TenKhach;
                String sdt = act.SDT;
                String sp = act.tb_warranty.item.tb_product_detail.productName;
                String mbh = act.tb_warranty.warrantyID;
                String datetake = act.startDate.ToString();
                String des = act.Description;
         
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        StringBuilder sb = new StringBuilder();

                        //Generate Invoice (Bill) Header.
                        sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                        sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Phiếu bảo hành</b></td></tr>");
                        sb.Append("<tr><td colspan = '2'></td></tr>");
                        sb.Append("<tr><td><b>Mã số: </b>");
                        sb.Append(MaBill);
                        sb.Append("</td><td align = 'right'><b>Ngày: </b>");
                        sb.Append(datetake);
                        sb.Append(" </td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Tên khách: </b>");
                        sb.Append(cusname);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>SDT </b>");
                        sb.Append(sdt);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Mã bảo hành: </b>");
                        sb.Append(mbh);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Tên sản phẩm: </b>");
                        sb.Append(sp);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Tình trạng lúc nhận: </b>");
                        sb.Append(des);
                        sb.Append("</td></tr>");
                        sb.Append("</table>");
                        sb.Append("<br />");



                        //Export HTML String as PDF.
                        Encoding encoding = Encoding.Unicode;
                        var bytes = encoding.GetBytes(sb.ToString());
                        string str = System.Text.Encoding.Unicode.GetString(bytes);
                        StringReader sr = new StringReader(str);
                        Document pdfDoc = new Document(new Rectangle(Utilities.MillimetersToPoints(78), Utilities.MillimetersToPoints(150)), 0, 0, 0, 0);

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
            catch (Exception e) {
            }
        }
    }
}
