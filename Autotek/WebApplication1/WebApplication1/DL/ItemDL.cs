using Npgsql;
using NpgsqlTypes;
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
							cmd.CommandText = "INSERT INTO item (typeofpay, registeredphonenumber, itemname, itemhsn, baseunit, secondaryunit, conversionrates, category, itemcode, saleprice, salewithorwithouttax, discountonsaleprice," +
								"wholesaleprice, wholesalewithorwithouttax, minimumwholesalequantity, purchaseprice, purchasewithorwithouttax, taxrate, openingquantity, remainingquantity, atprice, asofdate, minimumstocktomaintain, _location, percentageoramounttype) " +
								"VALUES (@typeofpay, @registeredphonenumber, @itemname, @itemhsn, @baseunit, @secondaryunit, @conversionrates, @category, @itemcode, @saleprice, @salewithorwithouttax, @discountonsaleprice," +
								" @wholesaleprice, @wholesalewithorwithouttax, @minimumwholesalequantity, @purchaseprice, @purchasewithorwithouttax, @taxrate, @openingquantity, @remainingquantity, @atprice, @asofdate, @minimumstocktomaintain, @_location, @percentageoramounttype)";
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
							cmd.Parameters.AddWithValue("@percentageoramounttype", oitemRq.percentageoramounttype);
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
								"itemcode = @itemcode, saleprice = @saleprice, salewithorwithouttax = @salewithorwithouttax, discountonsaleprice = @discountonsaleprice," +
								"wholesaleprice = @wholesaleprice, wholesalewithorwithouttax = @wholesalewithorwithouttax, minimumwholesalequantity = @minimumwholesalequantity, purchaseprice = @purchaseprice, purchasewithorwithouttax = @purchasewithorwithouttax," +
								"taxrate = @taxrate, openingquantity = @openingquantity, remainingquantity = @remainingquantity, atprice = @atprice, asofdate = @asofdate, minimumstocktomaintain = @minimumstocktomaintain, _location = @_location, percentageoramounttype = @percentageoramounttype WHERE " +
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
							cmd.Parameters.AddWithValue("@percentageoramounttype", oitemRq.percentageoramounttype);
							cmd.ExecuteNonQuery();
							oitemRs.status = "Item Updated Successfully";
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if(!string.IsNullOrEmpty(oitemRq.category))
				{
					bool categoryexist = false;
					try
					{
						try
						{
							using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
							{
								conn.Open();
								NpgsqlCommand cmd = new NpgsqlCommand();
								cmd.Connection = conn;
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = "SELECT category FROM categorygroup WHERE registeredphonenumber = " + oitemRq.registeredphonenumber + " AND category = '" + oitemRq.category + "'";
								NpgsqlDataReader reader = cmd.ExecuteReader();
								while (reader.Read())
								{
									categoryexist = true;
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{
							if (!categoryexist)
							{
								using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
								{
									conn.Open();
									NpgsqlCommand cmd = new NpgsqlCommand();
									cmd.Connection = conn;
									cmd.CommandType = CommandType.Text;
									cmd.CommandText = "INSERT INTO categorygroup(category, registeredphonenumber) VALUES(@category, @registeredphonenumber)";
									cmd.Parameters.AddWithValue("@category", oitemRq.category);
									cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
									cmd.ExecuteNonQuery();
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
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

		public GetItemListRs GetItemList(Int64 registeredphonenumber)
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
					cmd.CommandText = "SELECT itemname, remainingquantity, saleprice, purchaseprice, wholesaleprice, minimumwholesalequantity, percentageoramounttype, discountonsaleprice from item where registeredphonenumber = " + registeredphonenumber;
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
								oGetItemList.saleprice = Convert.ToInt64(reader["saleprice"]);
								oGetItemList.purchaseprice = Convert.ToInt64(reader["purchaseprice"]);
								oGetItemList.wholesaleprice = Convert.ToInt64(reader["wholesaleprice"]);
								oGetItemList.minimumwholesalequantity = Convert.ToInt64(reader["minimumwholesalequantity"]);
								oGetItemList.discountonsaleprice = Convert.ToInt64(reader["discountonsaleprice"]);
								oGetItemList.percentageoramounttype = Convert.ToString(reader["percentageoramounttype"]);
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

		public GetItemRs GetItemDetails(Int64 registeredphonenumber, string itemname)
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
					cmd.CommandText = "SELECT typeofpay, itemhsn, baseunit, secondaryunit, conversionrates, category, itemcode, saleprice, salewithorwithouttax, discountonsaleprice, wholesaleprice, wholesalewithorwithouttax," +
						"minimumwholesalequantity, purchaseprice, purchasewithorwithouttax, taxrate, openingquantity, remainingquantity, atprice, asofdate, minimumstocktomaintain, _location, percentageoramounttype FROM item WHERE registeredphonenumber = " + registeredphonenumber + " AND itemname = '" +
						itemname + "'";
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
								oGetAllItemList.percentageoramounttype = Convert.ToString(reader["percentageoramounttype"]);
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

		public GetCategoryRs GetCategory(Int64 registeredphonenumber)
		{
			GetCategoryRs oGetCategoryRs = new GetCategoryRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT category, COUNT(*) AS categorycount FROM item WHERE registeredphonenumber = " + registeredphonenumber + " GROUP BY category";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetCategoryListtRs oGetCategoryListtRs = new GetCategoryListtRs();
								oGetCategoryListtRs.category = Convert.ToString(reader["category"]);
								oGetCategoryListtRs.categorycount = Convert.ToInt64(reader["categorycount"]);
								oGetCategoryRs.getCateogoryList.Add(oGetCategoryListtRs);
							}
							oGetCategoryRs.status = "SUCCESS";
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
			return oGetCategoryRs;
		}

		public GetItemByCategoryRs GetItemByCategory(Int64 registeredphonenumber, string category)
		{
			GetItemByCategoryRs oGetItemByCategoryRs = new GetItemByCategoryRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT itemname, remainingquantity from item where registeredphonenumber = " + registeredphonenumber + " AND category = '" + category + "'";
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
								oGetItemByCategoryRs.getItemList.Add(oGetItemList);
							}
							oGetItemByCategoryRs.status = "SUCCESS";
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
			return oGetItemByCategoryRs;
		}

		public AddUpdateCategoryRs AddUpdateCategory(AddUpdateCategoryRq oAddUpdateCategoryRq)
		{
			AddUpdateCategoryRs oAddUpdateCategoryRs = new AddUpdateCategoryRs();
			string outputResult = string.Empty;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(dbConn))
				{
					conn.Open(); // Open the connection outside the loop

					using (NpgsqlCommand cmd = new NpgsqlCommand("sp_addupdatecategory", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_oldcategory", NpgsqlDbType.Varchar).Value = oAddUpdateCategoryRq.oldcategory;
						cmd.Parameters.AddWithValue("v_newcategory", NpgsqlDbType.Varchar).Value = oAddUpdateCategoryRq.newcategory;
						cmd.Parameters.AddWithValue("v_registeredphonenumber", NpgsqlDbType.Numeric).Value = oAddUpdateCategoryRq.registeredphonenumber;
						var outputParameter = new NpgsqlParameter("output_result", NpgsqlDbType.Varchar);
						outputParameter.Direction = ParameterDirection.Output;
						cmd.Parameters.Add(outputParameter);
						cmd.ExecuteNonQuery();
						outputResult = cmd.Parameters["output_result"].Value.ToString();
					}
				}
				oAddUpdateCategoryRs.status = outputResult;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oAddUpdateCategoryRs;
		}
	}
}
