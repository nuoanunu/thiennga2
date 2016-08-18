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
        public Boolean VAT { get; set; }
        public String cusName { get; set; }
        public String phoneNumber { get; set; }
        public int custype { get; set; }
        public String Adress { get; set; }
        public String Adress2 { get; set; }
        public int inventoryID { get; set; }
        public int date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public List<AnOrderDetail> items { get; set; }

        public NewItemViewModel() {
            items = new List<AnOrderDetail>();
            for (int i = 0; i < 20; i++) {
                AnOrderDetail war = new AnOrderDetail();
                items.Add(war);

            }
        }
        public int quantity { get; set; }

    }
    public class InvenotyChangeModel
    {
        public int newadd { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
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
        public List<int> lstDay { get; set; }
        public List<int> lstMonth { get; set; }
        public List<int> lstYear { get; set; }
        public KichHoatBaoHanh()
        {
            itemID = "";
            lst = new List<tb_warranty>();
            lstDay = new List<int>();
            lstMonth = new List<int>();
            lstYear = new List<int>();
            for (int i = 0; i < 10; i++) {
                tb_warranty a = new tb_warranty();
                a.warrantyID = "";
                lst.Add(a);
                lstDay.Add(DateTime.Now.Day);
                lstMonth.Add(DateTime.Now.Month);
                lstYear.Add(DateTime.Now.Year);
            }
        }



    }
    public class productDetailView {
        public String productName { get; set; }
        public String SKU { get; set; }
        public String factCoe { get; set; }
        public String minThreashold { get; set; }
        public String price { get; set; }
    }
    public class ConfirmItemView
    {
        public String itemName;
        public int quantity;
        public double price;
        public List<String> lst;
        public ConfirmItemView() {
            lst = new List<String>();
        }

    }
    public class cusInfo
    {
        public String cusname { get; set; }
        public String cussdt { get; set; }
        public String cusadd { get; set; }
        public String  cusadd2 { get; set; }


    }
    public class productView {
        public String name { get; set; }
        public String price { get; set; }

    }
    public class AnOrderDetail{
        public String SKU { get; set; }
        public int quantity { get; set; }
        public float chietKhauPhanTram { get; set; }
        public float chietKhauTrucTiep { get; set; }
        public float thanhTien { get; set; }
        public float dongia { get; set; }
        public string productID { get; set; }
        public string productName { get; set; }
        public string thanhTienS { get; set; }
        public string chietKhauTrucTiepS { get; set; }
        public string DonGiaS { get; set; }

    }
    public class HoaDonBaoHanhCheck {
        public String productName { get; set; }
        public String remainingTime { get; set; }
        public String mieuTa { get; set; }
        public int sepcial { get; set; }
    }
    public class emp {
        public String sdt { get; set; }
        public String email { get; set; }
        public String fullname { get; set; }
    }
    }