using Dapper;
using Microsoft.AspNetCore.Http;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
using WebApplication1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApplication1.DL
{
	public class SaleDL
	{
		private readonly string _connectionFactory;
		private static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		private static readonly string dbConn = root.GetValue<string>("ConnectionStrings");

		public SaleDL(IConfiguration configuration)
		{
			this._connectionFactory = configuration.GetValue<string>("ConnectionStrings");
		}

		public TransactionRs SaveTransaction(TransactionRq otransactionRq)
		{
			TransactionRs otransactionrs = new TransactionRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "INSERT INTO transactions(typeofpay, invoicenumber, invoicedate, stateofsupply, paymenttype, total, received, balance, customername, phonenumber, registeredphonenumber, billingaddress," +
						" shippingaddress, paymentstatus) VALUES(@typeofpay, @invoicenumber, @invoicedate, @stateofsupply, @paymenttype, @total, @received, @balance, @customername, @phonenumber, @registeredphonenumber, @billingaddress," +
						" @shippingaddress, @paymentstatus) RETURNING transaction_id";
					cmd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
					cmd.Parameters.AddWithValue("@invoicenumber", otransactionRq.invoicenumber);
					cmd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
					cmd.Parameters.AddWithValue("@stateofsupply", otransactionRq.stateofsupply);
					cmd.Parameters.AddWithValue("@paymenttype", otransactionRq.paymenttype);
					cmd.Parameters.AddWithValue("@total", otransactionRq.total);
					cmd.Parameters.AddWithValue("@received", otransactionRq.received);
					cmd.Parameters.AddWithValue("@balance", otransactionRq.balance);
					cmd.Parameters.AddWithValue("@customername", otransactionRq.customername);
					cmd.Parameters.AddWithValue("@phonenumber", otransactionRq.phonenumber);
					cmd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@billingaddress", otransactionRq.billingaddress);
					cmd.Parameters.AddWithValue("@shippingaddress", otransactionRq.shippingaddress);
					cmd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
					int transactionId = (int)cmd.ExecuteScalar();
					if (otransactionRq.itemdetailslist.Count > 0)
					{
						foreach (var itemDetail in otransactionRq.itemdetailslist)
						{
							using (NpgsqlConnection connn = new NpgsqlConnection(this._connectionFactory))
							{
								connn.Open();
								NpgsqlCommand cmdd = new NpgsqlCommand();
								cmdd.Connection = connn;
								cmdd.CommandType = CommandType.Text;
								cmdd.CommandText = "INSERT INTO item_details (transaction_id, item, qty, unit, priceperunit, registeredphonenumber, invoicenumber, customername, invoicedate, typeofpay, paymentstatus) VALUES (@transaction_id, @item, @qty, @unit, @priceperunit, @registeredphonenumber, @invoicenumber, @customername, @invoicedate, @typeofpay, @paymentstatus)";
								cmdd.Parameters.AddWithValue("@transaction_id", transactionId);
								cmdd.Parameters.AddWithValue("@item", itemDetail.item);
								cmdd.Parameters.AddWithValue("@qty", itemDetail.qty);
								cmdd.Parameters.AddWithValue("@unit", itemDetail.unit);
								cmdd.Parameters.AddWithValue("@priceperunit", itemDetail.priceperunit);
								cmdd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
								cmdd.Parameters.AddWithValue("@invoicenumber", otransactionRq.invoicenumber);
								cmdd.Parameters.AddWithValue("@customername", otransactionRq.customername);
								cmdd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
								cmdd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
								cmdd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
								cmdd.ExecuteNonQuery();
								otransactionrs.status = "SUCCESS";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return otransactionrs;
		}

		public string FindOrInsertItem(TransactionRq otransactionRq)
		{
			TransactionRs otransactionrs = new TransactionRs();
			string outputResult = string.Empty;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(dbConn))
				{
					conn.Open(); // Open the connection outside the loop

					foreach (var itemDetails in otransactionRq.itemdetailslist)
					{
						using (NpgsqlCommand cmd = new NpgsqlCommand("sp_findorinsertitems", conn))
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.AddWithValue("v_customername", NpgsqlDbType.Varchar).Value = otransactionRq.customername;
							cmd.Parameters.AddWithValue("v_phonenumber", NpgsqlDbType.Numeric).Value = otransactionRq.phonenumber;
							cmd.Parameters.AddWithValue("v_registeredphonenumber", NpgsqlDbType.Numeric).Value = otransactionRq.registeredphonenumber;
							cmd.Parameters.AddWithValue("v_billingaddress", NpgsqlDbType.Varchar).Value = otransactionRq.billingaddress;
							cmd.Parameters.AddWithValue("v_shippingaddress", NpgsqlDbType.Varchar).Value = otransactionRq.shippingaddress;
							cmd.Parameters.AddWithValue("v_partybalance", NpgsqlDbType.Numeric).Value = otransactionRq.partybalance;
							cmd.Parameters.AddWithValue("v_item", NpgsqlDbType.Varchar).Value = itemDetails.item;
							cmd.Parameters.AddWithValue("v_qty", NpgsqlDbType.Numeric).Value = itemDetails.qty;
							cmd.Parameters.AddWithValue("v_remainingquantity", NpgsqlDbType.Numeric).Value = itemDetails.remainingquantity;
							var outputParameter = new NpgsqlParameter("output_result", NpgsqlDbType.Varchar);
							outputParameter.Direction = ParameterDirection.Output;
							cmd.Parameters.Add(outputParameter);
							cmd.ExecuteNonQuery();
							outputResult = cmd.Parameters["output_result"].Value.ToString();
						}
					}
				}
				
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			otransactionrs.statusmessage = outputResult;
			return outputResult;
		}

		public GetPartyTransactionsRs GetPartyTransactions(GetPartyTransactionsRq oGetPartyTransactionsRq)
		{
			GetPartyTransactionsRs oGetPartyTransactionsRs = new GetPartyTransactionsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT tr.invoicenumber, tr.typeofpay, tr.invoicedate, tr.total, tr.balance, tr.phonenumber, pr.emailid, pr.billingaddress, pr.creditlimit, pr.gst FROM transactions tr join party pr ON tr.customername = pr.partyname" +
						" where tr.registeredphonenumber = " + oGetPartyTransactionsRq.registeredphonenumber + " AND " +
						"tr.customername = '" + oGetPartyTransactionsRq.customername + "' AND tr.showtransaction = 'SHOW'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetPartyTransactionsRs.gst = Convert.ToString(reader["gst"]);
								oGetPartyTransactionsRs.emailid = Convert.ToString(reader["emailid"]);
								oGetPartyTransactionsRs.billingaddress = Convert.ToString(reader["billingaddress"]);
								oGetPartyTransactionsRs.phonenumber = Convert.ToInt64(reader["phonenumber"]);
								GetAllPartyTransactionsList oGetAllPartyTransactionsList = new GetAllPartyTransactionsList();
								oGetAllPartyTransactionsList.typeofpay = Convert.ToString(reader["typeofpay"]);
								oGetAllPartyTransactionsList.invoicenumber = Convert.ToInt64(reader["invoicenumber"]);
								oGetAllPartyTransactionsList.creditlimit = Convert.ToInt64(reader["creditlimit"]);
								oGetAllPartyTransactionsList.invoicedate = Convert.ToDateTime(reader["invoicedate"]);
								oGetAllPartyTransactionsList.total = Convert.ToInt64(reader["total"]);
								oGetAllPartyTransactionsList.balance = Convert.ToInt64(reader["balance"]);
								oGetPartyTransactionsRs.partyTransactionsList.Add(oGetAllPartyTransactionsList);
							}
							oGetPartyTransactionsRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oGetPartyTransactionsRs.status = "FAILED";
						}

					}
					else
					{
						oGetPartyTransactionsRs.status = "No Recods Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetPartyTransactionsRs;
		}

		public GetPartyTransactionDetailsRs GetPartyTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT tr.typeofpay, tr.invoicedate, tr.stateofsupply, tr.paymenttype, tr.total, tr.received, tr.balance, tr.customername," +
						"tr.phonenumber, tr.billingaddress, tr.shippingaddress, ide.item, ide,qty, ide.unit, ide.priceperunit FROM transactions tr join item_details ide" +
						" ON tr.transaction_id = ide.transaction_id WHERE tr.invoicenumber = " + oGetPartyTransactionDetailsRq.invoicenumber + " AND tr.registeredphonenumber = " +
						oGetPartyTransactionDetailsRq.registeredphonenumber + "";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							bool recordFetched = true;
							while (reader.Read())
							{
								if (recordFetched)
								{
									oGetPartyTransactionDetailsRs.typeofpay = Convert.ToString(reader["typeofpay"]);
									oGetPartyTransactionDetailsRs.invoicedate = Convert.ToDateTime(reader["invoicedate"]);
									oGetPartyTransactionDetailsRs.stateofsupply = Convert.ToString(reader["stateofsupply"]);
									oGetPartyTransactionDetailsRs.paymenttype = Convert.ToString(reader["paymenttype"]);
									oGetPartyTransactionDetailsRs.total = Convert.ToInt64(reader["total"]);
									oGetPartyTransactionDetailsRs.received = Convert.ToInt64(reader["received"]);
									oGetPartyTransactionDetailsRs.balance = Convert.ToInt64(reader["balance"]);
									oGetPartyTransactionDetailsRs.customername = Convert.ToString(reader["customername"]);
									oGetPartyTransactionDetailsRs.phonenumber = Convert.ToInt64(reader["phonenumber"]);
									oGetPartyTransactionDetailsRs.billingaddress = Convert.ToString(reader["billingaddress"]);
									oGetPartyTransactionDetailsRs.shippingaddress = Convert.ToString(reader["shippingaddress"]);
									recordFetched = false;
								}
								ItemDetailsListRs oItemDetailsListRs = new ItemDetailsListRs();
								oItemDetailsListRs.item = Convert.ToString(reader["item"]);
								oItemDetailsListRs.qty = Convert.ToInt64(reader["qty"]);
								oItemDetailsListRs.unit = Convert.ToString(reader["unit"]);
								oItemDetailsListRs.priceperunit = Convert.ToInt64(reader["priceperunit"]);
								oGetPartyTransactionDetailsRs.itemdetailslist.Add(oItemDetailsListRs);
							}
							oGetPartyTransactionDetailsRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oGetPartyTransactionDetailsRs.status = "FAILED";
						}

					}
					else
					{
						oGetPartyTransactionDetailsRs.status = "No Recods Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetPartyTransactionDetailsRs;
		}

		public List<GetAllItemTransactionsList> GetItemTransactions(GetItemTransactionsRq oGetItemTransactionsRq)
		{
			GetItemTransactionsRs oGetItemTransactionsRs = new GetItemTransactionsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT invoicenumber, typeofpay, customername, invoicedate, qty, priceperunit, paymentstatus " +
						"FROM item_details WHERE registeredphonenumber = " + oGetItemTransactionsRq.registeredphonenumber + " and item = '" + oGetItemTransactionsRq.itemname + "' AND showtransaction = 'SHOW'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetAllItemTransactionsList oGetAllItemTransactionsList = new GetAllItemTransactionsList();
								oGetAllItemTransactionsList.invoicenumber = Convert.ToInt64(reader["invoicenumber"]);
								oGetAllItemTransactionsList.typeofpay = Convert.ToString(reader["typeofpay"]);
								oGetAllItemTransactionsList.partyname = Convert.ToString(reader["customername"]);
								oGetAllItemTransactionsList.invoicedate = Convert.ToDateTime(reader["invoicedate"]);
								oGetAllItemTransactionsList.qty = Convert.ToInt64(reader["qty"]);
								oGetAllItemTransactionsList.priceperunit = Convert.ToInt64(reader["priceperunit"]);
								oGetAllItemTransactionsList.paymentstatus = Convert.ToString(reader["paymentstatus"]);
								oGetItemTransactionsRs.itemTransactionsList.Add(oGetAllItemTransactionsList);
							}
							oGetItemTransactionsRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oGetItemTransactionsRs.status = "FAILED";
						}
					}
					else
					{
						oGetItemTransactionsRs.status = "No Recods Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetItemTransactionsRs.itemTransactionsList;
		}

		public GetItemTransactionsRs GetItemHeaderDetails(GetItemTransactionsRq oGetItemTransactionsRq, List<GetAllItemTransactionsList> oGetAllItemTransactionsList)
		{
			GetItemTransactionsRs oGetItemTransactionsRs = new GetItemTransactionsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT saleprice, wholesaleprice, purchaseprice, remainingquantity from item WHERE registeredphonenumber = " + oGetItemTransactionsRq.registeredphonenumber + " and itemname = '" + oGetItemTransactionsRq.itemname + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetItemTransactionsRs.saleprice = Convert.ToInt64(reader["saleprice"]);
								oGetItemTransactionsRs.wholesaleprice = Convert.ToInt64(reader["wholesaleprice"]);
								oGetItemTransactionsRs.purchaseprice = Convert.ToInt64(reader["purchaseprice"]);
								oGetItemTransactionsRs.remainingquantity = Convert.ToInt64(reader["remainingquantity"]);
							}
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						oGetItemTransactionsRs.itemTransactionsList = oGetAllItemTransactionsList;
					}
					else
					{
						oGetItemTransactionsRs.status = "No Recods Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetItemTransactionsRs;
		}

		public GetTypeOfPayTransactionsRs GetTypeOfPayTransactions(GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT invoicenumber, typeofpay, customername, invoicedate, paymentstatus, paymenttype, total, balance FROM transactions where registeredphonenumber = " + oGetTypeOfPayTransactionsRq.registeredphonenumber +
						" AND typeofpay = '" + oGetTypeOfPayTransactionsRq.typeofpay + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{

								GetTypeOfPayTransactionsList oGetTypeOfPayTransactionsList = new GetTypeOfPayTransactionsList();
								oGetTypeOfPayTransactionsList.invoicenumber = Convert.ToInt64(reader["invoicenumber"]);
								oGetTypeOfPayTransactionsList.typeofpay = Convert.ToString(reader["typeofpay"]);
								oGetTypeOfPayTransactionsList.customername = Convert.ToString(reader["customername"]);
								oGetTypeOfPayTransactionsList.invoicedate = Convert.ToDateTime(reader["invoicedate"]);
								oGetTypeOfPayTransactionsList.paymentstatus = Convert.ToString(reader["paymentstatus"]);
								oGetTypeOfPayTransactionsList.paymenttype = Convert.ToString(reader["paymenttype"]);
								oGetTypeOfPayTransactionsList.total = Convert.ToInt64(reader["total"]);
								oGetTypeOfPayTransactionsList.balance = Convert.ToInt64(reader["balance"]);
								oGetTypeOfPayTransactionsRs.typeofpaytransactionlist.Add(oGetTypeOfPayTransactionsList);
							}
							oGetTypeOfPayTransactionsRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oGetTypeOfPayTransactionsRs.status = "FAILED";
						}
					}
					else
					{
						oGetTypeOfPayTransactionsRs.status = "No Recods Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetTypeOfPayTransactionsRs;
		}

		public Int64 GetInvoiceNumberCount(GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT MAX(invoicenumber) FROM transactions WHERE typeofpay = '" + oGetTypeOfPayTransactionsRq.typeofpay + "' AND registeredphonenumber = " + oGetTypeOfPayTransactionsRq .registeredphonenumber + "" ;
					object result = cmd.ExecuteScalar();
					if (result != DBNull.Value)
					{
						oGetTypeOfPayTransactionsRs.invoicenumbercount = Convert.ToInt64(result);
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
			return oGetTypeOfPayTransactionsRs.invoicenumbercount;
		}

		public TransactionRs SaveDeliveryChallan(TransactionRq otransactionRq, Int64 invoicecount)
		{
			TransactionRs otransactionrs = new TransactionRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "INSERT INTO transactions(typeofpay, invoicenumber, invoicedate, stateofsupply, paymenttype, total, received, balance, customername, phonenumber, registeredphonenumber, billingaddress," +
						" shippingaddress, paymentstatus) VALUES(@typeofpay, @invoicenumber, @invoicedate, @stateofsupply, @paymenttype, @total, @received, @balance, @customername, @phonenumber, @registeredphonenumber, @billingaddress," +
						" @shippingaddress, @paymentstatus) RETURNING transaction_id";
					cmd.Parameters.AddWithValue("@typeofpay", "SALE");
					cmd.Parameters.AddWithValue("@invoicenumber", (invoicecount+1));
					cmd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
					cmd.Parameters.AddWithValue("@stateofsupply", otransactionRq.stateofsupply);
					cmd.Parameters.AddWithValue("@paymenttype", otransactionRq.paymenttype);
					cmd.Parameters.AddWithValue("@total", otransactionRq.total);
					cmd.Parameters.AddWithValue("@received", otransactionRq.received);
					cmd.Parameters.AddWithValue("@balance", otransactionRq.balance);
					cmd.Parameters.AddWithValue("@customername", otransactionRq.customername);
					cmd.Parameters.AddWithValue("@phonenumber", otransactionRq.phonenumber);
					cmd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@billingaddress", otransactionRq.billingaddress);
					cmd.Parameters.AddWithValue("@shippingaddress", otransactionRq.shippingaddress);
					cmd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
					int transactionId = (int)cmd.ExecuteScalar();
					if (otransactionRq.itemdetailslist.Count > 0)
					{
						foreach (var itemDetail in otransactionRq.itemdetailslist)
						{
							using (NpgsqlConnection connn = new NpgsqlConnection(this._connectionFactory))
							{
								connn.Open();
								NpgsqlCommand cmdd = new NpgsqlCommand();
								cmdd.Connection = connn;
								cmdd.CommandType = CommandType.Text;
								cmdd.CommandText = "INSERT INTO item_details (transaction_id, item, qty, unit, priceperunit, registeredphonenumber, invoicenumber, customername, invoicedate, typeofpay, paymentstatus) VALUES (@transaction_id, @item, @qty, @unit, @priceperunit, @registeredphonenumber, @invoicenumber, @customername, @invoicedate, @typeofpay, @paymentstatus)";
								cmdd.Parameters.AddWithValue("@transaction_id", transactionId);
								cmdd.Parameters.AddWithValue("@item", itemDetail.item);
								cmdd.Parameters.AddWithValue("@qty", itemDetail.qty);
								cmdd.Parameters.AddWithValue("@unit", itemDetail.unit);
								cmdd.Parameters.AddWithValue("@priceperunit", itemDetail.priceperunit);
								cmdd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
								cmdd.Parameters.AddWithValue("@invoicenumber", otransactionRq.invoicenumber);
								cmdd.Parameters.AddWithValue("@customername", otransactionRq.customername);
								cmdd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
								cmdd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
								cmdd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
								cmdd.ExecuteNonQuery();
								otransactionrs.status = "SUCCESS";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return otransactionrs;
		}

		public Int64 GetInvoiceNumberCountDLChallan(Int64 registeredphonenumber, string typeofpay)
		{
			Int64 invoicecount = 0;
			try
			{
				
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT MAX(invoicenumber) FROM transactions WHERE typeofpay = '" + typeofpay + "' AND registeredphonenumber = " + registeredphonenumber + "";
					object result = cmd.ExecuteScalar();
					if (result != DBNull.Value)
					{
						invoicecount = Convert.ToInt64(result);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return invoicecount;
		}

		public void UpdateDlChallan(Int64 invoicenumber,Int64 registeredphonenumber)
		{
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
						cmd.CommandText = "UPDATE transactions SET showtransaction = 'DONT SHOW' WHERE invoicenumber = '" + invoicenumber + "' AND registeredphonenumber = " + registeredphonenumber + "";
						cmd.ExecuteNonQuery();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				try
				{
					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "UPDATE item_details SET showtransaction = 'DONT SHOW' WHERE invoicenumber = '" + invoicenumber + "' AND registeredphonenumber = " + registeredphonenumber + "";
						cmd.ExecuteNonQuery();
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
}
