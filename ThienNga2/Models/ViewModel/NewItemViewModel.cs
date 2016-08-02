using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThienNga2.Models.Entities;
using System.ComponentModel.DataAnnotations;
namespace ThienNga2.Models.ViewModel
{
    public class NewItemViewModel
    {
        [Required]
        [Display(Name = "Tên khách hàng")]
        public String cusName { get; set; }
        [Required]
        [Display(Name = "SDT khách hàng")]
        public String phoneNumber { get; set; }
        [Display(Name = "Adress")]
        public String Adress { get; set; }
        public item item { get; set; }
        public List<item> items { get; set; }
        public NewItemViewModel() {
            items = new List<item>();
            item = new item();
            for (int i = 0; i < 20; i++) {
                item war = new item();
                war.DateOfSold = DateTime.Today;
                items.Add(war);
                
            }
        }
        public int quantity { get; set; }
 
    }
    public class InvenotyChangeModel
    {
        public int newadd { get; set; }
        public inventory inven { get; set; }
        public InvenotyChangeModel()
        {
            newadd = 0;
            inven = new inventory();
        }
        public InvenotyChangeModel(inventory i)
        {
            newadd = 0;
            inven = i;
        }

    }
    public class InventoryView
    {
        public tb_product_detail productdetail { get; set; }
        public inventory invendetail { get; set; }
        public InventoryView()
        {
            productdetail = new tb_product_detail();
            invendetail = new inventory();
        }
        public InventoryView(inventory i)
        {
            productdetail = new tb_product_detail();
            invendetail = i;
        }

    }
    public class KichHoatBaoHanh
    {
        public String itemID { get; set; }
        public List<tb_warranty> lst { get; set; }
        public KichHoatBaoHanh()
        {
            itemID = "";
            lst = new List<tb_warranty>();
            for (int i = 0; i < 10; i++) {
                tb_warranty a = new tb_warranty();
                a.warrantyID = "";
                lst.Add(a);
            }
        }
     

    }
    public class ConfirmItemView
    {
        public String itemName;
        public int quantity;
        public double price;
        public List<String> lst;
        public ConfirmItemView (){
            lst = new List<String>();
        }

    }
    }