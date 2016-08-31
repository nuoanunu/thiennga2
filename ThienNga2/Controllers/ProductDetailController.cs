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
            ViewData["last50"] = am.tb_product_detail.SqlQuery(" select   * from tb_product_detail order by id desc  ").ToList();
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
        public String fixxit(int id, String name, String SKU, String factCode, float price, int mini) {
         
                tb_product_detail dt = am.tb_product_detail.Find(id);
            List<inventory> invens = am.inventories.SqlQuery("SELECT * FROM inventory where productStoreCode='" + dt.productStoreID + "' and productFactoryCode='" + dt.producFactoryID + "'").ToList();
            foreach (inventory inv in invens) {
                inv.productFactoryCode = "00000000";
                inv.productStoreCode = "0000000";
            }
            am.SaveChanges();
                if (dt != null) {
                    dt.productName = name;
                    dt.productStoreID = SKU;
                    dt.producFactoryID = factCode;
                    dt.minThresHold = mini;
                    dt.price = price;
                }
                am.SaveChanges();
            foreach (inventory inv in invens)
            {
                inv.productFactoryCode = factCode;
                inv.productStoreCode = SKU;
            }
            am.SaveChanges();
            checkwarModel model = new checkwarModel();
                model.name = "succeed";
                JavaScriptSerializer javas = new JavaScriptSerializer();
                return javas.Serialize(model);
        
            
            return "";
        }
        [HttpPost]
        // POST: ProductDetail/Create
        public ActionResult Create(tb_product_detail Model)
        {   try
            {
                if (Model != null)
                {
                    int id = -1;
                    
                    if (id < 1)
                    {
                        Model.tb_cate = am.tb_cate.Find(Model.cateID);
                        if (Model.producFactoryID == null || Model.producFactoryID.Equals("")) {
                            Model.producFactoryID = Model.productStoreID;
                        }
                        if (Model.productStoreID == null || Model.productStoreID.Equals(""))
                        {
                            Model.productStoreID = Model.producFactoryID;
                        }
                        am.tb_product_detail.Add(Model);
                        am.SaveChanges();
                    }
                    
                    ViewData["newproduct"] = Model;
                    return View("ConfirmNewProduct");
                }
            }
            catch (Exception e) {
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
