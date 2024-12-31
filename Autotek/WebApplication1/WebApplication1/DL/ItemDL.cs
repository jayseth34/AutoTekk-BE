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
					cmd.CommandText = "SELECT itemname FROM item WHERE registeredphonenumber = @registeredphonenumber AND itemname = @itemname";
					cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@itemname", oitemRq.itemname);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						oitemRs.statusmessage = "Item already exists.";
						oitemRs.status = "Failed";
						return oitemRs;
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
								"wholesaleprice, wholesalewithorwithouttax, minimumwholesalequantity, purchaseprice, purchasewithorwithouttax, taxrate, openingquantity, remainingquantity, atprice, asofdate, minimumstocktomaintain, _location, percentageoramounttype, mrp) " +
								"VALUES (@typeofpay, @registeredphonenumber, @itemname, @itemhsn, @baseunit, @secondaryunit, @conversionrates, @category, @itemcode, @saleprice, @salewithorwithouttax, @discountonsaleprice," +
								" @wholesaleprice, @wholesalewithorwithouttax, @minimumwholesalequantity, @purchaseprice, @purchasewithorwithouttax, @taxrate, @openingquantity, @remainingquantity, @atprice, @asofdate, @minimumstocktomaintain, @_location, @percentageoramounttype, @mrp)";
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
							cmd.Parameters.AddWithValue("@mrp", oitemRq.mrp);
							cmd.ExecuteNonQuery();
							oitemRs.statusmessage = "Inserted Successfully";
							oitemRs.status = "Success";
						}
					}
					catch (Exception ex)
					{
						oitemRs.statusmessage = "Data Could Not Be Inserted";
						oitemRs.status = "Failed";
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
								cmd.CommandText = "SELECT category FROM category WHERE registeredphonenumber = @registeredphonenumber AND category = @category";
								cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@category", oitemRq.category);
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
									cmd.CommandText = "INSERT INTO category (category, registeredphonenumber) VALUES(@category, @registeredphonenumber)";
									cmd.Parameters.AddWithValue("@category", oitemRq.category);
									cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
									cmd.ExecuteNonQuery();
									oitemRs.status = "Success";
									oitemRs.statusmessage = "Inserted Successfully";
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oitemRs.status = "Failed";
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						oitemRs.status = "Failed";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oitemRs.status = "Failed";
			}

			return oitemRs;
		}

		public ItemRs UpdateItem(ItemRq oitemRq)
		{
			ItemRs oitemRs = new ItemRs();
			bool exist = false;
			try
			{
				if (oitemRq.itemname != oitemRq.olditemname)
				{
					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "SELECT itemname FROM item WHERE registeredphonenumber = @registeredphonenumber AND itemname = @itemname";
						cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@itemname", oitemRq.itemname);
						NpgsqlDataReader reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							oitemRs.statusmessage = "Item already exists.";
							oitemRs.status = "Failed";
							return oitemRs;
						}
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
							cmd.CommandText = @"UPDATE item SET typeofpay = @typeofpay, itemname = @itemname, itemhsn = @itemhsn, baseunit = @baseunit, secondaryunit = @secondaryunit, conversionrates = @conversionrates, category = @category,
                    itemcode = @itemcode, saleprice = @saleprice, salewithorwithouttax = @salewithorwithouttax, discountonsaleprice = @discountonsaleprice,
                    wholesaleprice = @wholesaleprice, wholesalewithorwithouttax = @wholesalewithorwithouttax, minimumwholesalequantity = @minimumwholesalequantity, purchaseprice = @purchaseprice, purchasewithorwithouttax = @purchasewithorwithouttax,
                    taxrate = @taxrate, openingquantity = @openingquantity, remainingquantity = @remainingquantity, atprice = @atprice, asofdate = @asofdate, minimumstocktomaintain = @minimumstocktomaintain, _location = @_location, percentageoramounttype = @percentageoramounttype, mrp = @mrp
                    WHERE registeredphonenumber = @where_registeredphonenumber AND itemname = @where_itemname"; // Parameterized WHERE clause
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
							cmd.Parameters.AddWithValue("@mrp", oitemRq.mrp);
							cmd.Parameters.AddWithValue("@where_registeredphonenumber", oitemRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@where_itemname", oitemRq.itemname);
							cmd.ExecuteNonQuery();
							oitemRs.statusmessage = "Item Updated Successfully";
							oitemRs.status = "Success";
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						oitemRs.status = "Failed";
					}
				}
				if (!string.IsNullOrEmpty(oitemRq.category))
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
								cmd.CommandText = "SELECT category FROM category WHERE registeredphonenumber = @registeredphonenumber AND category = @category";
								cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@category", oitemRq.category);
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
									cmd.CommandText = "INSERT INTO category (category, registeredphonenumber) VALUES(@category, @registeredphonenumber)";
									cmd.Parameters.AddWithValue("@category", oitemRq.category);
									cmd.Parameters.AddWithValue("@registeredphonenumber", oitemRq.registeredphonenumber);
									cmd.ExecuteNonQuery();
									oitemRs.status = "Success";
									oitemRs.statusmessage = "Item Updated Successfully";
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oitemRs.status = "Failed";
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						oitemRs.status = "Failed";
					}
				}
				return oitemRs;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return oitemRs;
			}
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
					cmd.CommandText = "SELECT itemname, remainingquantity, saleprice, purchaseprice, baseunit, wholesaleprice, minimumwholesalequantity, percentageoramounttype, discountonsaleprice, mrp, itemcode FROM item WHERE registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetItemList oGetItemList = new GetItemList();
								oGetItemList.itemname = reader["itemname"] == DBNull.Value ? "" : Convert.ToString(reader["itemname"]);
								oGetItemList.remainingquantity = reader["remainingquantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["remainingquantity"]);
								oGetItemList.saleprice = reader["saleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["saleprice"]);
								oGetItemList.purchaseprice = reader["purchaseprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["purchaseprice"]);
								oGetItemList.wholesaleprice = reader["wholesaleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["wholesaleprice"]);
								oGetItemList.minimumwholesalequantity = reader["minimumwholesalequantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["minimumwholesalequantity"]);
								oGetItemList.discountonsaleprice = reader["discountonsaleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["discountonsaleprice"]);
								oGetItemList.mrp = reader["mrp"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["mrp"]);
								oGetItemList.percentageoramounttype = reader["percentageoramounttype"] == DBNull.Value ? "" : Convert.ToString(reader["percentageoramounttype"]);
								oGetItemList.baseunit = reader["baseunit"] == DBNull.Value ? "" : Convert.ToString(reader["baseunit"]);
								oGetItemList.itemcode = reader["itemcode"] == DBNull.Value ? "" : Convert.ToString(reader["itemcode"]);

								oGetItemListRs.getItemList.Add(oGetItemList);
							}
							oGetItemListRs.status = "SUCCESS";
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
					cmd.CommandText = @"SELECT typeofpay, itemhsn, baseunit, secondaryunit, conversionrates, category, itemcode, saleprice, salewithorwithouttax, discountonsaleprice, wholesaleprice, wholesalewithorwithouttax,
                    minimumwholesalequantity, purchaseprice, purchasewithorwithouttax, taxrate, openingquantity, remainingquantity, atprice, asofdate, minimumstocktomaintain, _location, percentageoramounttype 
                FROM item 
                WHERE registeredphonenumber = @registeredphonenumber AND itemname = @itemname";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					cmd.Parameters.AddWithValue("@itemname", itemname);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetAllItemList oGetAllItemList = new GetAllItemList();
								oGetAllItemList.typeofpay = reader["typeofpay"] == DBNull.Value ? "" : Convert.ToString(reader["typeofpay"]);
								oGetAllItemList.itemhsn = reader["itemhsn"] == DBNull.Value ? "" : Convert.ToString(reader["itemhsn"]);
								oGetAllItemList.baseunit = reader["baseunit"] == DBNull.Value ? "" : Convert.ToString(reader["baseunit"]);
								oGetAllItemList.secondaryunit = reader["secondaryunit"] == DBNull.Value ? "" : Convert.ToString(reader["secondaryunit"]);
								oGetAllItemList.conversionrates = reader["conversionrates"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["conversionrates"]);
								oGetAllItemList.category = reader["category"] == DBNull.Value ? "" : Convert.ToString(reader["category"]);
								oGetAllItemList.itemcode = reader["itemcode"] == DBNull.Value ? "" : Convert.ToString(reader["itemcode"]);
								oGetAllItemList.saleprice = reader["saleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["saleprice"]);
								oGetAllItemList.salewithorwithouttax = reader["salewithorwithouttax"] == DBNull.Value ? "" : Convert.ToString(reader["salewithorwithouttax"]);
								oGetAllItemList.discountonsaleprice = reader["discountonsaleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["discountonsaleprice"]);
								oGetAllItemList.wholesaleprice = reader["wholesaleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["wholesaleprice"]);
								oGetAllItemList.wholesalewithorwithouttax = reader["wholesalewithorwithouttax"] == DBNull.Value ? "" : Convert.ToString(reader["wholesalewithorwithouttax"]);
								oGetAllItemList.minimumwholesalequantity = reader["minimumwholesalequantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["minimumwholesalequantity"]);
								oGetAllItemList.purchaseprice = reader["purchaseprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["purchaseprice"]);
								oGetAllItemList.purchasewithorwithouttax = reader["purchasewithorwithouttax"] == DBNull.Value ? "" : Convert.ToString(reader["purchasewithorwithouttax"]);
								oGetAllItemList.taxrate = reader["taxrate"] == DBNull.Value ? "" : Convert.ToString(reader["taxrate"]);
								oGetAllItemList.openingquantity = reader["openingquantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["openingquantity"]);
								oGetAllItemList.remainingquantity = reader["remainingquantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["remainingquantity"]);
								oGetAllItemList.atprice = reader["atprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["atprice"]);
								oGetAllItemList.asofdate = reader["asofdate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["asofdate"]);
								oGetAllItemList.minimumstocktomaintain = reader["minimumstocktomaintain"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["minimumstocktomaintain"]);
								oGetAllItemList._location = reader["_location"] == DBNull.Value ? "" : Convert.ToString(reader["_location"]);
								oGetAllItemList.percentageoramounttype = reader["percentageoramounttype"] == DBNull.Value ? "" : Convert.ToString(reader["percentageoramounttype"]);
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
					cmd.CommandText = @"SELECT cat.category, COUNT(it.category) AS categorycount 
                FROM category AS cat 
                LEFT JOIN item AS it ON cat.category = it.category AND it.registeredphonenumber = @registeredphonenumber_join
                WHERE cat.registeredphonenumber = @registeredphonenumber_where
                GROUP BY cat.category";
					cmd.Parameters.AddWithValue("@registeredphonenumber_where", registeredphonenumber);
					cmd.Parameters.AddWithValue("@registeredphonenumber_join", registeredphonenumber);
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
					cmd.CommandText = "SELECT itemname, remainingquantity, purchaseprice FROM item WHERE registeredphonenumber = @registeredphonenumber AND category = @category";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					cmd.Parameters.AddWithValue("@category", category);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetItemList oGetItemList = new GetItemList();
								oGetItemList.itemname = reader["itemname"] == DBNull.Value ? "" : Convert.ToString(reader["itemname"]);
								oGetItemList.remainingquantity = reader["remainingquantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["remainingquantity"]);
								oGetItemList.purchaseprice = reader["purchaseprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["purchaseprice"]);
								oGetItemByCategoryRs.getItemList.Add(oGetItemList);
							}
							oGetItemByCategoryRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oGetItemByCategoryRs.status = "FAILED";
						}
					}
				}
				oGetItemByCategoryRs.status = "SUCCESS";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oGetItemByCategoryRs.status = "FAILED";
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
				oAddUpdateCategoryRs.statusmessage = outputResult;
				oAddUpdateCategoryRs.status = "Success";
				if(outputResult == "Record exists")
				{
					oAddUpdateCategoryRs.status = "Failed";
				}
			}
			catch (Exception ex)
			{
				oAddUpdateCategoryRs.statusmessage = outputResult;
				oAddUpdateCategoryRs.status = "Failed";
				Console.WriteLine(ex.Message);
			}
			return oAddUpdateCategoryRs;
		}

		public async Task<long> GetNextSequenceValue()
		{
			long nextValue = 0;

			string sqlQuery = "SELECT nextval('assigncodeseq');";
			try
			{
				using (var connection = new NpgsqlConnection(dbConn))
				{
					connection.Open();

					using (var command = new NpgsqlCommand(sqlQuery, connection))
					{
						nextValue = (long)command.ExecuteScalar();
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return nextValue;
		}
	}
}
