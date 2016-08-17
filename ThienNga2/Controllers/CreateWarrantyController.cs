using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;

namespace ThienNga2.Controllers
{
    public class CreateWarrantyController : EntitiesAM
    {
    
        private List<String> allname = new List<String>();
        public void getAllName()
        {
           List<AspNetUser> lst = am.AspNetUsers.ToList();
            foreach (AspNetUser au in lst) {
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
                       
                        item detail = am.items.SqlQuery("SELECT * FROM dbo.item WHERE productID='" + war.itemID + "'").FirstOrDefault();
                        if (detail != null) {
                            vi.mieuTa = war.description;
                            vi.productName = detail.tb_product_detail.productName;
                            DateTime date = (DateTime)war.startdate;
                            date=date.AddMonths(war.duration);
                            vi.remainingTime = date.Day + "/" + date.Month + "/" + date.Year;
                            if (war.Special == 1) {
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
        public ActionResult CreateNew(String actid, String phoneNumber ,String cusname, String IMEI, String Descrip, String Emp1, String Emp2)
        {
            tb_warranty_activities act;
            if (actid != null ) {
                if (actid.Trim().Length > 0) {
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
                var emplo2 = am.AspNetUsers.SqlQuery("SELECT * FROM dbo.AspNetUsers WHERE Email='" + Emp2 + "'").ToList().First();
                act.employee = (string)emplo1.Id;
                act.AspNetUser = emplo1;
                act.empFixer = (string)emplo2.Id;
                act.AspNetUser1 = emplo2;
                act.TenKhach = cusname;
                act.SDT = phoneNumber;
                act.status = 1;
                act.Description = Descrip;
                act.warrantyID = IMEI;
                act.startDate = DateTime.Today;
                String date = act.startDate.Day.ToString();
                if (date.Length == 1) date = "0" + date;
                String month = act.startDate.Month.ToString();
                 if (month.Length == 1) month = "0" + month;
                act.CodeBaoHanh =  date + month +"-"+ phoneNumber.Substring(phoneNumber.Length - 5);
                    am.tb_warranty_activities.Add(act);
                am.SaveChanges();
                int id = act.id;
            System.Diagnostics.Debug.WriteLine("ID NAY " + id); 
            
            act.id = id;
            act = am.tb_warranty_activities.Find(id);
            return RedirectToAction("ConfirmCreate", "CreateWarranty", new { acid = id });
       
       
        }
      
        public ActionResult ConfirmCreate(int acid) {
  
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
           
            return RedirectToAction("Search","Warranty", new { code = actid, searchType = "warrantyActID" });
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
    }
}
