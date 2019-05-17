using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindConsole.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "nah - Enter the  Product name aqui!")]
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        [Required(ErrorMessage = "nah - Enter a digit for the  Product price aqui!")]
        public decimal? UnitPrice { get; set; }
        public Int16? UnitsInStock { get; set; }
        public Int16? UnitsOnOrder { get; set; }
        public Int16? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}