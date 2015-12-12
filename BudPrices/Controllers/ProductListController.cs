using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using BudPrices.Models;

namespace BudPrices.Controllers
{
    public class ProductListController : Controller
    {
		public ActionResult Index()
		{
			return View();
		}   // Index()

		[HttpGet]
		[ActionName("AveragePrice")]
		public string ReturnPriceMode(string id, string type)
		{
			List<List<Product>> data = new List<List<Product>>();

			if (type == "concentrates")
			{
				IEnumerable<Products> concentratePrices = GenerateListOfPrices(id, type);
				List<List<Product>> modeConcentrate = GeneratePriceMode(concentratePrices);
				data = modeConcentrate;
			}	// if
			else if (type == "edibles")
			{
				IEnumerable<Products> ediblePrices = GenerateListOfPrices(id, type);
				List<List<Product>> modeEdible = GeneratePriceMode(ediblePrices);
				data = modeEdible;
			}	// else if
			else if (type == "flowers")
			{
				IEnumerable<Products> flowerPrices = GenerateListOfPrices(id, type);
				List<List<Product>> modeFlower = GeneratePriceMode(flowerPrices);
				data = modeFlower;
			}	// else if
			else if (type == "other")
			{
				IEnumerable<Products> otherPrices = GenerateListOfPrices(id, type);
				List<List<Product>> modeOther = GeneratePriceMode(otherPrices);
				data = modeOther;
			}	// else if

			var json = JsonConvert.SerializeObject(data);
			return json;
		}   // ReturnPriceMode(string, string)

		public IEnumerable<Products> GenerateListOfPrices(string id, string type)
		{
			List<Products> prices = new List<Products>();
			string[] xmlArray = Directory.GetFiles($@"c:\WeedPrices\{type}\");

			foreach (var xml in xmlArray)
			{
				try
				{
					XmlDocument layoutXml = new XmlDocument();
					layoutXml.Load(xml);
					StringReader sr = new StringReader(layoutXml.DocumentElement.OuterXml);
					DataSet ds = new DataSet();
					ds.ReadXml(sr);

					// trim the last 12-4 digits from each file name for the date
					string date = xml.Substring(xml.Length - 12);
					date = date.Remove(date.Length - 4);

					if (ds.Tables.Count > 0)
					{
						var products = new List<Products>();
						products = (from rows in ds.Tables[0].AsEnumerable()
									select new Products
									{
										Strain = rows[0].ToString().ToLower(), 
										Price = rows[1].ToString(),
										Quantity = rows[2].ToString(),
										Date = date,
									}).ToList();

						foreach (var product in products)
						{
							if (product.Strain.Contains(id.ToLower()))
							{
                                prices.Add(new Products { Strain = product.Strain, Price = product.Price, Quantity = product.Quantity, Date = product.Date });
							}   // if
						}   // foreach
					}   // if
				}   // try
				catch (Exception e)
				{
					throw e;
				}   // catch
			}   // foreach
			return prices;
		}   // GenerateListOfPrices(string, string)

		public List<KeyValuePair<string, double>> CalculateModeByQuantity(List<KeyValuePair<string, double>> priceList)
		{
			Dictionary<string, double> dateAndAveragePriceDictionary = new Dictionary<string, double>();
			List<string> dates = new List<string>();
			List<double> prices = new List<double>();
			double price;
			var myList = priceList.OrderBy(d => d.Key);

			foreach (var data in myList)
			{
				dates.Add(data.Key);
				prices.Add(data.Value);
			}   // foreach

			for (int i = 0; i < dates.Count(); i++)
			{
				List<double> pricesOnASpecificDayForASpecificProduct = new List<double>();
				int j = i;
				int whileCount = 0;
				while (dates[i] == dates[j])
				{
					pricesOnASpecificDayForASpecificProduct.Add(prices[j]);
					if (dates.Count() > (j + 1))
					{
						j++;
					}   // if
					else
					{
						break;
					}   // else
					whileCount++;
				}   // while

				var group = pricesOnASpecificDayForASpecificProduct.GroupBy(v => v);
				int count = group.Max(g => g.Count());
				price = group.First(g => g.Count() == count).Key;

				if (!dateAndAveragePriceDictionary.ContainsKey(dates[i]))
				{
					dateAndAveragePriceDictionary.Add(dates[i], Math.Round(price, 2, MidpointRounding.AwayFromZero));
				}   // if

				if (whileCount != 0)
				{
					i = i + (whileCount - 1);
				}   // if
			}   // for
			List<KeyValuePair<string, double>> dateAndAveragePriceList = dateAndAveragePriceDictionary.ToList();
			return dateAndAveragePriceList;
		}   // CalculateModeByQuantity(List<KeyValuePair<string, double>>)

		public List<List<Product>> GeneratePriceMode(IEnumerable<Products> allPrices)
		{
			List<List<Product>> modePrices = new List<List<Product>>();

			List<KeyValuePair<string, double>> halfGramPrices = new List<KeyValuePair<string, double>>();
			List<KeyValuePair<string, double>> gramPrices = new List<KeyValuePair<string, double>>();
			List<KeyValuePair<string, double>> twoGramPrices = new List<KeyValuePair<string, double>>();
			List<KeyValuePair<string, double>> eighthPrices = new List<KeyValuePair<string, double>>();
			List<KeyValuePair<string, double>> quarterPrices = new List<KeyValuePair<string, double>>();
			List<KeyValuePair<string, double>> halfPrices = new List<KeyValuePair<string, double>>();
			List<KeyValuePair<string, double>> ouncePrices = new List<KeyValuePair<string, double>>();

			foreach (var entry in allPrices)
			{
				string price = entry.Price.Replace("$", string.Empty);
				string quantity = entry.Quantity.ToString();
				double priceInt = double.Parse(price);

				if (quantity == "1/2 Gram" || quantity == "Half Gram" || quantity == "HalfGram")
				{
					halfGramPrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}   // else if
				else if (quantity == "Gram" || quantity == "1 Gram")
				{
					gramPrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}   // else if
				else if (quantity == "2 Grams")
				{
					twoGramPrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}   // else if
				else if (quantity == "Eighth")
				{
					eighthPrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}   // else if
				else if (quantity == "Quarter")
				{
					quarterPrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}   // else if
				else if (quantity == "Half")
				{
					halfPrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}   // else if
				else if (quantity == "Ounce")
				{
					ouncePrices.Add(new KeyValuePair<string, double>(entry.Date, priceInt));
				}	// else if
			}   // foreach

			if (halfGramPrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(halfGramPrices, "halfGram");
				modePrices.Add(solution);
			}   // if

			if (gramPrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(gramPrices, "gram");
				modePrices.Add(solution);
			}   // if

			if (twoGramPrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(twoGramPrices, "twoGram");
				modePrices.Add(solution);
			}   // if

			if (eighthPrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(eighthPrices, "eighth");
				modePrices.Add(solution);
			}   // if

			if (quarterPrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(quarterPrices, "quarter");
				modePrices.Add(solution);
			}   // if

			if (halfPrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(halfPrices, "half");
				modePrices.Add(solution);
			}   // if

			if (ouncePrices.Count != 0)
			{
				List<Product> solution = new List<Product>();
				solution = CreateProductList(ouncePrices, "ounce");
				modePrices.Add(solution);
			}	// if

			return modePrices;
		}   // GeneratePriceMode(IEnumerable<Products>)

		public List<Product> CreateProductList(List<KeyValuePair<string, double>> priceList, string listName)
		{
			List<Product> solution = new List<Product>();
			List<KeyValuePair<string, double>> mode = new List<KeyValuePair<string, double>>(CalculateModeByQuantity(priceList));
			double average = Math.Round(priceList.Average(x => x.Value), 2, MidpointRounding.AwayFromZero);
			double lowest = Math.Round(priceList.OrderBy(x => x.Value).First().Value, 2, MidpointRounding.AwayFromZero);
			double highest = Math.Round(priceList.OrderBy(x => x.Value).Last().Value, 2, MidpointRounding.AwayFromZero);

			for (int i = 0; i < mode.Count(); i++)
			{
				solution.Add(new Product(listName, mode[i].Key, mode[i].Value, average, lowest, highest));
			}
			return solution;
		}   // CreateListOfModes(List<KeyValuePair<string, double>>, string)
	}
}