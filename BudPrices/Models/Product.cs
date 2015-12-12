using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudPrices.Models
{
	public class Product
	{
		public string date;
		public double mode;
		public string quantity;
		public double average;
		public double lowest;
		public double highest;

		public Product(string quantity, string date, double mode, double average, double lowest, double highest)
		{
			this.quantity = quantity;
			this.date = date;
			this.mode = mode;
			this.average = average;
			this.lowest = lowest;
			this.highest = highest;
		}
	}
}