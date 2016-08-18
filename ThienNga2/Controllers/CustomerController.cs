using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class CustomerController : EntitiesAM
    {
        // GET: Customer
        public ActionResult Index()
        {
            ViewData["typleList"] = am.CustomerTypes.ToList();
            return View("NhomKhachHang");
        }
        public ActionResult CreateGroup(String name, String color) {
            try {
                CustomerType ct = new CustomerType();
                ct.Color = color;
                ct.GroupName = name;
                am.CustomerTypes.Add(ct);
                am.SaveChanges();
            }
            catch (Exception e) {

            }
         
            
            ViewData["typleList"] =am.CustomerTypes.ToList();
            return RedirectToAction("Index","Customer");
        }
    }
}