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
        public ActionResult CreateGroup(String name, String color ,String thongtinbaohanh) {
            try {
                CustomerType ct = new CustomerType();
                ct.Color = color;
                ct.GroupName = name;
                ct.MoTaChinhSach = thongtinbaohanh;
                am.CustomerTypes.Add(ct);
                am.SaveChanges();
            }
            catch (Exception e) {

            }
         
            
            ViewData["typleList"] =am.CustomerTypes.ToList();
            return RedirectToAction("Index","Customer");
        }
        public ActionResult deletethisid(String id) {
            try
            {
                am.CustomerTypes.Remove(am.CustomerTypes.Find(int.Parse(id)));
                am.SaveChanges();
            }
            catch (Exception e) { }
            return RedirectToAction("Index");
        }
    }
}