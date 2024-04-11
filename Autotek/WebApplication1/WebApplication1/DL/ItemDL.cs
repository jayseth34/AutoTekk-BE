using Npgsql;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.DL
{
	public class ItemDL
	{
		private readonly string _connectionFactory;
		private static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		private static readonly string dbConn = root.GetValue<string>("ConnectionStrings");

		public ItemDL(IConfiguration configuration)
		{
			this._connectionFactory = configuration.GetValue<string>("ConnectionStrings");
		}

		public ItemRs AddItem(ItemRq oitemRq)
		{
			ItemRs oitemRs = new ItemRs();
			bool exist = false;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT itemname FROM item WHERE registeredphonenumber = " + oitemRq.registeredphonenumber;
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						oitemRs.status = "Item already exists.";
						exist = true;
					}
				}
				if (!exist)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "INSERT INTO item (typeofpay, registeredphonenumber, itemname, itemhsn, baseunit, secondaryunit, conversionrates, category, itemcode, saleprice, salewithorwithouttax, discountonsaleprice, percentageoramount," +
								"wholesaleprice, wholesalewithorwithouttax, minimumwholesalequantity, purchaseprice, purchasewithorwithouttax, taxrate, openingquantity, remainingquantity, atprice, asofdate, minimumstocktomaintain, _location) " +
								"VALUES (@typeofpay, @registeredphonenumber, @itemname, @itemhsn, @baseunit, @secondaryunit, @conversionrates, @category, @itemcode, @saleprice, @salewithorwithouttax, @discountonsaleprice, @percentageoramount," +
								" @wholesaleprice, @wholesalewithorwithouttax, @minimumwholesalequantity, @purchaseprice, @purchasewithorwithouttax, @taxrate, @openingquantity, @remainingquantity, @atprice, @asofdate, @minimumstocktomaintain, @_location)";
							cmd.Parameters.AddWithValue("@typeofpay", oitemRq.typeofpay);
							cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@itemname", oitemRq.itemname);
							cmd.Parameters.AddWithValue("@itemhsn", oitemRq.itemhsn);
							cmd.Parameters.AddWithValue("@baseunit", oitemRq.baseunit);
							cmd.Parameters.AddWithValue("@secondaryunit", oitemRq.secondaryunit);
							cmd.Parameters.AddWithValue("@conversionrates", oitemRq.conversionrates);
							cmd.Parameters.AddWithValue("@category", oitemRq.category);
							cmd.Parameters.AddWithValue("@itemcode", oitemRq.itemcode);
							cmd.Parameters.AddWithValue("@saleprice", oitemRq.saleprice);
							cmd.Parameters.AddWithValue("@salewithorwithouttax", oitemRq.salewithorwithouttax);
							cmd.Parameters.AddWithValue("@discountonsaleprice", oitemRq.discountonsaleprice);
							cmd.Parameters.AddWithValue("@percentageoramount", oitemRq.percentageoramount);
							cmd.Parameters.AddWithValue("@wholesaleprice", oitemRq.wholesaleprice);
							cmd.Parameters.AddWithValue("@wholesalewithorwithouttax", oitemRq.wholesalewithorwithouttax);
							cmd.Parameters.AddWithValue("@minimumwholesalequantity", oitemRq.minimumwholesalequantity);
							cmd.Parameters.AddWithValue("@purchaseprice", oitemRq.purchaseprice);
							cmd.Parameters.AddWithValue("@purchasewithorwithouttax", oitemRq.purchasewithorwithouttax);
							cmd.Parameters.AddWithValue("@taxrate", oitemRq.taxrate);
							cmd.Parameters.AddWithValue("@openingquantity", oitemRq.openingquantity);
							cmd.Parameters.AddWithValue("@remainingquantity", oitemRq.remainingquantity);
							cmd.Parameters.AddWithValue("@atprice", oitemRq.atprice);
							cmd.Parameters.AddWithValue("@asofdate", oitemRq.asofdate);
							cmd.Parameters.AddWithValue("@minimumstocktomaintain", oitemRq.minimumstocktomaintain);
							cmd.Parameters.AddWithValue("@_location", oitemRq._location);
							cmd.ExecuteNonQuery();
							oitemRs.status = "Inserted Successfully";
						}
					}
					catch (Exception ex)
					{
						oitemRs.status = "Data Could Not Be Inserted";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return oitemRs;
		}
	}
}
