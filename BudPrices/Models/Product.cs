using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudPrices.Models
{
	public class Product
	{
		public string date;
		public double price;
		public string quantity;

		public Product(string quantity, string date, double price)
		{
			this.quantity = quantity;
			this.date = date;
			this.price = price;
		}
	}
}