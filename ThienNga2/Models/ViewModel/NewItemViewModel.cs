using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThienNga2.Models.Entities;

namespace ThienNga2.Models.ViewModel
{
    public class NewItemViewModel
    {
        public item item { get; set; }
        public List<tb_warranty> warranties { get; set; }
        public NewItemViewModel() {
            warranties = new List<tb_warranty>();
            item = new item();
            for (int i = 0; i < 20; i++) {
                tb_warranty war = new tb_warranty();
                war.itemID = "0";
                warranties.Add(war);
            }
        }
    }
}