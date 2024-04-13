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
					cmd.CommandText = "SELECT itemname FROM item WHERE registeredphonenumber = " + oitemRq.registeredphonenumber + " AND itemname = '" + oitemRq.itemname + "'";
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
				if (exist)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "Update item SET typeofpay = @typeofpay, itemname = @itemname, itemhsn = @itemhsn, baseunit = @baseunit, secondaryunit = @secondaryunit, conversionrates = @conversionrates, category = @category," +
								"itemcode = @itemcode, saleprice = @saleprice, salewithorwithouttax = @salewithorwithouttax, discountonsaleprice = @discountonsaleprice, percentageoramount = @percentageoramount," +
								"wholesaleprice = @wholesaleprice, wholesalewithorwithouttax = @wholesalewithorwithouttax, minimumwholesalequantity = @minimumwholesalequantity, purchaseprice = @purchaseprice, purchasewithorwithouttax = @purchasewithorwithouttax," +
								"taxrate = @taxrate, openingquantity = @openingquantity, remainingquantity = @remainingquantity, atprice = @atprice, asofdate = @asofdate, minimumstocktomaintain = @minimumstocktomaintain, _location = @_location WHERE " +
								"registeredphonenumber = " + oitemRq.registeredphonenumber + " AND itemname = '" + oitemRq.itemname + "'";
							cmd.Parameters.AddWithValue("@typeofpay", oitemRq.typeofpay);
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
							oitemRs.status = "Item Updated Successfully";
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return oitemRs;
		}

		public GetItemListRs GetItemList(GetItemListRq oGetItemListRq)
		{
			GetItemListRs oGetItemListRs = new GetItemListRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT itemname, remainingquantity from item where registeredphonenumber = " + oGetItemListRq.registeredphonenumber;
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetItemList oGetItemList = new GetItemList();
								oGetItemList.itemname = Convert.ToString(reader["itemname"]);
								oGetItemList.remainingquantity = Convert.ToInt64(reader["remainingquantity"]);
								oGetItemListRs.getItemList.Add(oGetItemList);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetItemListRs;
		}

		public GetItemRs GetItemDetails(GetItemRq oGetItemRq)
		{
			GetItemRs oGetItemRs = new GetItemRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT typeofpay, itemhsn, baseunit, secondaryunit, conversionrates, category, itemcode, saleprice, salewithorwithouttax, discountonsaleprice, percentageoramount, wholesaleprice, wholesalewithorwithouttax," +
						"minimumwholesalequantity, purchaseprice, purchasewithorwithouttax, taxrate, openingquantity, remainingquantity, atprice, asofdate, minimumstocktomaintain, _location FROM item WHERE registeredphonenumber = " + oGetItemRq.registeredPhoneNumber + " AND itemname = '" +
						oGetItemRq.itemName + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetAllItemList oGetAllItemList = new GetAllItemList();
								oGetAllItemList.typeofpay = Convert.ToString(reader["typeofpay"]);
								oGetAllItemList.itemhsn = Convert.ToString(reader["itemhsn"]);
								oGetAllItemList.baseunit = Convert.ToString(reader["baseunit"]);
								oGetAllItemList.secondaryunit = Convert.ToString(reader["secondaryunit"]);
								oGetAllItemList.conversionrates = Convert.ToInt64(reader["conversionrates"]);
								oGetAllItemList.category = Convert.ToString(reader["category"]);
								oGetAllItemList.itemcode = Convert.ToString(reader["itemcode"]);
								oGetAllItemList.saleprice = Convert.ToInt64(reader["saleprice"]);
								oGetAllItemList.salewithorwithouttax = Convert.ToString(reader["salewithorwithouttax"]);
								oGetAllItemList.discountonsaleprice = Convert.ToInt64(reader["discountonsaleprice"]);
								oGetAllItemList.percentageoramount = Convert.ToInt64(reader["percentageoramount"]);
								oGetAllItemList.wholesaleprice = Convert.ToInt64(reader["wholesaleprice"]);
								oGetAllItemList.wholesalewithorwithouttax = Convert.ToString(reader["wholesalewithorwithouttax"]);
								oGetAllItemList.minimumwholesalequantity = Convert.ToInt64(reader["minimumwholesalequantity"]);
								oGetAllItemList.purchaseprice = Convert.ToInt64(reader["purchaseprice"]);
								oGetAllItemList.purchasewithorwithouttax = Convert.ToString(reader["purchasewithorwithouttax"]);
								oGetAllItemList.taxrate = Convert.ToString(reader["taxrate"]);
								oGetAllItemList.openingquantity = Convert.ToInt64(reader["openingquantity"]);
								oGetAllItemList.remainingquantity = Convert.ToInt64(reader["remainingquantity"]);
								oGetAllItemList.atprice = Convert.ToInt64(reader["atprice"]);
								oGetAllItemList.asofdate = reader["asofdate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["asofdate"]);
								oGetAllItemList.minimumstocktomaintain = Convert.ToInt64(reader["minimumstocktomaintain"]);
								oGetAllItemList._location = Convert.ToString(reader["_location"]);
								oGetItemRs.itemList.Add(oGetAllItemList);
								oGetItemRs.status = "SUCCESS";
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
					else
					{
						oGetItemRs.status = "FAILED";
						oGetItemRs.statusMessage = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetItemRs;
		}
	}
}
