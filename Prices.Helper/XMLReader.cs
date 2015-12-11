using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.IO;
using Prices.Helper.External.Models;
using System.Xml;

namespace Prices.Helper
{
	public class XMLReader
	{
		public List<Products> ReturnListOfProducts(string id)
		{
			//Dictionary<int, List<Products>> allPrices = new Dictionary<int, List<Products>>();
			List<Products> allPrices = new List<Products>();
            string[] xmlArray = Directory.GetFiles(@"c:\WeedPrices\");
			int i = 0;

			foreach (var xml in xmlArray)
			{
				try
				{
					//string xmlData = File.ReadAllText(xml);
					//xmlData = xmlData.Replace("\r\n", "");//Path of the xml script
					XmlDocument layoutXml = new XmlDocument();
					layoutXml.Load(xml);
					StringReader sr = new StringReader(layoutXml.DocumentElement.OuterXml);
					DataSet ds = new DataSet();//Using dataset to read xml file  
					ds.ReadXml(sr);

					if (ds.Tables.Count > 0)
					{
						var products = new List<Products>();
						products = (from rows in ds.Tables[0].AsEnumerable()
									select new Products
									{
										Strain = rows[0].ToString(), //Convert row to int  
										Price = rows[1].ToString(),
										Quantity = rows[2].ToString(),
									}).ToList();

						foreach (var product in products)
						{
							if (product.Strain.Contains(id))
							{
								allPrices.Add(product);
								i++;
							}	// if
						}	// foreach
					}	// if
				}	// try
				catch (Exception e)
				{
					throw e;
				}	// catch
			}	// foreach
			return allPrices;
		}   // ReturnListOfProducts(string)
	}
}
