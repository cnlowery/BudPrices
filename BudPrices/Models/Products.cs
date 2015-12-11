using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace BudPrices.Models
{
	public class Products
	{
		public string Strain { get; set; }
		public string Price { get; set; }
		public string Quantity { get; set; }
		public string Date { get; set; }
	}
}