using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using ThienNga2.Controllers;
using ThienNga2.Models.Entities;
using ThienNga2.Models.ViewModel;

namespace ThienNga2.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductDetailController : EntitiesAM
    {
        

        // GET: ProductDetail
        public ActionResult Index()
        {
            ViewData["last50"] = am.tb_product_detail.SqlQuery(" select  top 50  * from tb_product_detail order by id desc  ").ToList();
            return View("NewProduct");
        }

        // GET: ProductDetail/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        
        public String getAllData(String sku) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try {
                tb_product_detail dt = am.tb_product_detail.SqlQuery("SELECT * FROM tb_product_detail where productStoreID='" + sku + "' or producFactoryID='" + sku + "'").FirstOrDefault();
                if (dt != null)
                {
                    productDetailView pdv = new productDetailView();
                    if (dt.minThresHold != 0)
                        pdv.minThreashold = dt.minThresHold.ToString();
                    else pdv.minThreashold = "5";
                    pdv.SKU = dt.productStoreID;
                    pdv.productName = dt.productName;
                    pdv.factCoe = dt.producFactoryID;
                    pdv.price = dt.price.ToString();
                    return serializer.Serialize(pdv);
                }
                else {
                    productDetailView pdv = new productDetailView();
                    pdv.productName = "";
                    return serializer.Serialize(pdv);
                }
            }
            catch (Exception e) {
               
            }
            return "";
        }
        [HttpPost]
        // POST: ProductDetail/Create
        public ActionResult Create(tb_product_detail Model)
        {
            if (Model != null)
            {
                int id = -1;
                if (am.ThienNga_FindProductDetailID(Model.productStoreID).FirstOrDefault().HasValue )
                    id = (int)am.ThienNga_FindProductDetailID(Model.productStoreID).FirstOrDefault().Value;
                else if (am.ThienNga_FindProductDetailID(Model.producFactoryID).FirstOrDefault().HasValue)
                    id = (int)am.ThienNga_FindProductDetailID(Model.producFactoryID).FirstOrDefault().Value;
                else if (am.ThienNga_FindProductDetailID(Model.productName).FirstOrDefault().HasValue )
                    id = (int)am.ThienNga_FindProductDetailID(Model.productName).FirstOrDefault().Value;
                if (id < 1 )
                {
                    Model.tb_cate = am.tb_cate.Find(Model.cateID);
                    if (Model.producFactoryID == null || Model.producFactoryID.Equals("")) { Model.producFactoryID = Model.productStoreID; }
                    am.tb_product_detail.Add(Model);
                    am.SaveChangesAsync();
                }
                else {
       
                    tb_product_detail edit = am.tb_product_detail.Find(id);
                    edit.cateID = Model.cateID;
             
                    edit.price = Model.price;
                    edit.producFactoryID = Model.producFactoryID;
                    edit.productStoreID = Model.productStoreID;
                    
                    edit.minThresHold = Model.minThresHold;
                    System.Diagnostics.Debug.WriteLine("CCCCCCC " + edit.minThresHold);
                    edit.productName = Model.productName;
                    am.SaveChanges();
                }
                ViewData["newproduct"] = Model;
                return View("ConfirmNewProduct");
            }
            return View("NewProduct");
        }
        [HttpPost]
        public ActionResult UpdateFromXML(HttpPostedFileBase file)
        {
            try { 
            if (file != null && file.ContentLength > 0 && file.ContentType == "text/xml")
            {
                var document = new XmlDocument();
                document.Load(file.InputStream);
      
                     XMLReader.reader(document);
                    ViewData["updateResult"] = "Cập nhật thành công";
                }
            }
            catch (Exception e) {
                ViewData["updateResult"] = "Cập nhật thất bại";
            }
            
            return View("NewProduct");

        }



        [HttpPost]
        public ActionResult Edit(tb_product_detail t , string Command)
        {
           
            if (Command.Equals("save")) {
                ModelState.Clear();
                return RedirectToAction("index");
            }
            else if(Command.Equals ( "edit")) {
                return View("NewProduct" ,t);
            }
            return View(Command);

        }

  
        // GET: ProductDetail/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductDetail/Delete/5
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
