using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudPrices.Models
{
	public class Product
	{
		public string date;
		public string quantity;
		public double mode;
		public double average;
		public double lowest;
		public double highest;

		public Product(string date, string quantity, double mode, double average, double lowest, double highest)
		{
			this.date = date;
			this.quantity = quantity;
			this.mode = mode;
			this.average = average;
			this.lowest = lowest;
			this.highest = highest;
		}
	}
}