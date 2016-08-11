using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using ThienNga2.Models.Entities;

namespace ThienNga2.Controllers
{
    public class XMLReader 
    {
       
        public static  void reader(XmlDocument doc) {
            XDocument xDoc = XDocument.Load(new XmlNodeReader(doc));
            XElement all = xDoc.Root;
            ThienNgaDatabaseEntities am = new ThienNgaDatabaseEntities();
            foreach (XElement element in all.Descendants("Product"))
            {
                String Name = element.Element("Name").Value;
                String FullDescription = element.Element("FullDescription").Value;
                XElement ProductVariants = element.Element("ProductVariants");
                foreach (XElement element2 in ProductVariants.Descendants("ProductVariant"))
                {
                    String SKU = element2.Element("SKU").Value;
                    if (SKU == null) SKU = "";

                    if (SKU.Length > 0)
                    {
                        String ManufacturerPartNumber = element2.Element("ManufacturerPartNumber").Value;
                        if (ManufacturerPartNumber == null) ManufacturerPartNumber = "";
                        String Price = element2.Element("Price").Value;
                        float pricef = float.Parse(Price) / 10000;

                        tb_product_detail a = new tb_product_detail();
                        if (a.producFactoryID != null && a.producFactoryID.Length > 1)
                            a.producFactoryID = ManufacturerPartNumber;
                        else
                            a.producFactoryID = SKU;
                        a.productStoreID = SKU;
                        a.productName = Name;
                        a.price = pricef;
                        int temp;
                        tb_product_detail b = am.ThienNga_FindProduct2(a.productStoreID).FirstOrDefault();
                        if (b!= null)
                        {
                            if (a.producFactoryID != null && a.producFactoryID.Length > 1)
                                b.producFactoryID = a.producFactoryID;
                            else
                                b.producFactoryID = b.productStoreID;
                            b.productName = Name;
                            b.price = a.price;
                            System.Diagnostics.Debug.WriteLine("UPDATED " + SKU);
                            am.SaveChangesAsync ();
                        }
                        else {
                            am.tb_product_detail.Add(a);
                            am.SaveChangesAsync();
                        }
                        
                      
                    }
                }
            }
        }
    }
}