using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using NpgsqlTypes;
using Razorpay.Api;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
using WebApplication1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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
						" shippingaddress, paymentstatus, amountdetails, isbankscustomernameupdate, bankscustomername) VALUES(@typeofpay, @invoicenumber, @invoicedate, @stateofsupply, @paymenttype, @total, @received, @balance, @customername, @phonenumber, @registeredphonenumber, @billingaddress," +
						" @shippingaddress, @paymentstatus, @amountdetails, false, @bankscustomername) RETURNING transaction_id";
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
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(otransactionRq.amountdetailslist));
					cmd.Parameters.AddWithValue("@bankscustomername", otransactionRq.customername);
					var transactionId = cmd.ExecuteScalar();
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
								cmdd.CommandText = "INSERT INTO item_details (transaction_id, item, qty, unit, priceperunit, registeredphonenumber, invoicenumber, customername, invoicedate, typeofpay, paymentstatus, taxrate, taxrateamount, discountpercent, discountamount) " +
									"VALUES (@transaction_id, @item, @qty, @unit, @priceperunit, @registeredphonenumber, @invoicenumber, @customername, @invoicedate, @typeofpay, @paymentstatus, @taxrate, @taxrateamount, @discountpercent, @discountamount)";
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
								cmdd.Parameters.AddWithValue("@taxrate", itemDetail.taxrate);
								cmdd.Parameters.AddWithValue("@taxrateamount", itemDetail.taxrateamount);
								cmdd.Parameters.AddWithValue("@discountpercent", itemDetail.discountpercent);
								cmdd.Parameters.AddWithValue("@discountamount", itemDetail.discountamount);
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
							cmd.Parameters.AddWithValue("v_topayparty", NpgsqlDbType.Numeric).Value = otransactionRq.topayparty;
							cmd.Parameters.AddWithValue("v_toreceivefromparty", NpgsqlDbType.Numeric).Value = otransactionRq.toreceivefromparty;
							cmd.Parameters.AddWithValue("v_item", NpgsqlDbType.Varchar).Value = itemDetails.item;
							cmd.Parameters.AddWithValue("v_qty", NpgsqlDbType.Numeric).Value = itemDetails.qty;
							cmd.Parameters.AddWithValue("v_remainingquantity", NpgsqlDbType.Numeric).Value = itemDetails.remainingquantity;
							cmd.Parameters.AddWithValue("v_invoicenumber", NpgsqlDbType.Numeric).Value = otransactionRq.invoicenumber;
							cmd.Parameters.AddWithValue("v_typeofpay", NpgsqlDbType.Varchar).Value = otransactionRq.typeofpay;
							cmd.Parameters.AddWithValue("v_isupdate", NpgsqlDbType.Boolean).Value = otransactionRq.isupdate;
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

		public GetPartyTransactionsRs GetPartyTransactions(Int64 registeredphonenumber, string customername)
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
					cmd.CommandText = "SELECT DISTINCT ON (tr.invoicenumber, tr.typeofpay)  tr.invoicenumber, tr.typeofpay, tr.invoicedate, tr.total, tr.balance, tr.phonenumber, tr.paymentstatus, pr.emailid, pr.billingaddress, pr.creditlimit, pr.gst, pr.creditlimit FROM transactions tr right join party pr ON tr.registeredphonenumber = pr.registeredphonenumber" +
						" where tr.registeredphonenumber = " + registeredphonenumber + " AND " +
						"tr.customername = '" + customername + "' AND tr.showtransaction = 'SHOW'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetPartyTransactionsRs.gst = reader["gst"] == DBNull.Value ? "" : Convert.ToString(reader["gst"]);
								oGetPartyTransactionsRs.emailid = reader["emailid"] == DBNull.Value ? "" : Convert.ToString(reader["emailid"]);
								oGetPartyTransactionsRs.billingaddress = reader["billingaddress"] == DBNull.Value ? "" : Convert.ToString(reader["billingaddress"]);
								oGetPartyTransactionsRs.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
								oGetPartyTransactionsRs.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["creditlimit"]);
								GetAllPartyTransactionsList oGetAllPartyTransactionsList = new GetAllPartyTransactionsList(); 
								oGetAllPartyTransactionsList.typeofpay = reader["typeofpay"] == DBNull.Value ? "" : Convert.ToString(reader["typeofpay"]);
								oGetAllPartyTransactionsList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetAllPartyTransactionsList.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["creditlimit"]);
								oGetAllPartyTransactionsList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetAllPartyTransactionsList.total = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
								oGetAllPartyTransactionsList.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["balance"]);
								oGetAllPartyTransactionsList.paymentstatus = reader["paymentstatus"] == DBNull.Value ? "" : Convert.ToString(reader["paymentstatus"]);
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
						oGetPartyTransactionsRs.status = "SUCCESS";
						oGetPartyTransactionsRs.statusmessage = "No records found";

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
			string amtdetails = string.Empty;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT tr.typeofpay, tr.invoicedate, tr.stateofsupply, tr.paymenttype, tr.total, tr.received, tr.balance, tr.customername," +
						"tr.phonenumber, tr.billingaddress, tr.shippingaddress, tr.amountdetails, ide.item, ide,qty, ide.unit, ide.priceperunit, ide.transaction_id, ide.taxrate, ide.taxrateamount, ide.discountpercent, ide.discountamount FROM transactions tr join item_details ide" +
						" ON tr.transaction_id = ide.transaction_id WHERE tr.invoicenumber = " + oGetPartyTransactionDetailsRq.invoicenumber + " AND tr.registeredphonenumber = " +
						oGetPartyTransactionDetailsRq.registeredphonenumber + " AND tr.typeofpay = '" + oGetPartyTransactionDetailsRq.typeofpay + "'";
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
									oGetPartyTransactionDetailsRs.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
									oGetPartyTransactionDetailsRs.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
									oGetPartyTransactionDetailsRs.stateofsupply = reader["stateofsupply"] == DBNull.Value ? null : Convert.ToString(reader["stateofsupply"]);
									oGetPartyTransactionDetailsRs.paymenttype = reader["paymenttype"] == DBNull.Value ? null : Convert.ToString(reader["paymenttype"]);
									oGetPartyTransactionDetailsRs.total = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
									oGetPartyTransactionDetailsRs.received = reader["received"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["received"]);
									oGetPartyTransactionDetailsRs.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["balance"]);
									oGetPartyTransactionDetailsRs.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
									oGetPartyTransactionDetailsRs.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
									oGetPartyTransactionDetailsRs.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
									oGetPartyTransactionDetailsRs.shippingaddress = reader["shippingaddress"] == DBNull.Value ? null : Convert.ToString(reader["shippingaddress"]);
									amtdetails = reader["amountdetails"] == DBNull.Value ? null : Convert.ToString(reader["amountdetails"]);
									recordFetched = false;
								}
								ItemDetailsListRs oItemDetailsListRs = new ItemDetailsListRs();
								oItemDetailsListRs.item = reader["item"] == DBNull.Value ? null : Convert.ToString(reader["item"]);
								oItemDetailsListRs.qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["qty"]);
								oItemDetailsListRs.unit = reader["unit"] == DBNull.Value ? null : Convert.ToString(reader["unit"]);
								oItemDetailsListRs.priceperunit = reader["priceperunit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["priceperunit"]);
								oItemDetailsListRs.transactionid = reader["transaction_id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["transaction_id"]);
								oItemDetailsListRs.taxrate = reader["taxrate"] == DBNull.Value ? null : Convert.ToString(reader["taxrate"]);
								oItemDetailsListRs.taxrateamount = reader["taxrateamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["taxrateamount"]);
								oItemDetailsListRs.discountpercent = reader["discountpercent"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["discountpercent"]);
								oItemDetailsListRs.discountamount = reader["discountamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["discountamount"]);
								oGetPartyTransactionDetailsRs.amountdetailslist = JsonConvert.DeserializeObject<List<AmountDetails>>(amtdetails);
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
						oGetPartyTransactionDetailsRs.status = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetPartyTransactionDetailsRs;
		}

		public List<GetAllItemTransactionsList> GetItemTransactions(Int64 registeredphonenumber, string itemname)
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
						"FROM item_details WHERE registeredphonenumber = " + registeredphonenumber + " and item = '" + itemname + "' AND showtransaction = 'SHOW'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetAllItemTransactionsList oGetAllItemTransactionsList = new GetAllItemTransactionsList();
								oGetAllItemTransactionsList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetAllItemTransactionsList.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetAllItemTransactionsList.partyname = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
								oGetAllItemTransactionsList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetAllItemTransactionsList.qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["qty"]);
								oGetAllItemTransactionsList.priceperunit = reader["priceperunit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["priceperunit"]);
								oGetAllItemTransactionsList.paymentstatus = reader["paymentstatus"] == DBNull.Value ? null : Convert.ToString(reader["paymentstatus"]);
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
						oGetItemTransactionsRs.status = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetItemTransactionsRs.itemTransactionsList;
		}

		public GetItemTransactionsRs GetItemHeaderDetails(Int64 registeredphonenumber, string itemname, List<GetAllItemTransactionsList> oGetAllItemTransactionsList)
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
					cmd.CommandText = "SELECT saleprice, wholesaleprice, purchaseprice, remainingquantity from item WHERE registeredphonenumber = " + registeredphonenumber + " and itemname = '" + itemname + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetItemTransactionsRs.saleprice = reader["saleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["saleprice"]);
								oGetItemTransactionsRs.wholesaleprice = reader["wholesaleprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["wholesaleprice"]);
								oGetItemTransactionsRs.purchaseprice = reader["purchaseprice"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["purchaseprice"]);
								oGetItemTransactionsRs.remainingquantity = reader["remainingquantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["remainingquantity"]);
								oGetItemTransactionsRs.status = "SUCCESS";

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
						oGetItemTransactionsRs.status = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetItemTransactionsRs;
		}

		public GetTypeOfPayTransactionsRs GetTypeOfPayTransactions(Int64 registeredphonenumber, string typeofpay)
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
					cmd.CommandText = "SELECT invoicenumber, typeofpay, customername, invoicedate, paymentstatus, paymenttype, total, balance, isconverted FROM transactions where registeredphonenumber = " + registeredphonenumber +
						" AND typeofpay = '" + typeofpay + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{

								GetTypeOfPayTransactionsList oGetTypeOfPayTransactionsList = new GetTypeOfPayTransactionsList();
								oGetTypeOfPayTransactionsList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetTypeOfPayTransactionsList.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetTypeOfPayTransactionsList.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
								oGetTypeOfPayTransactionsList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetTypeOfPayTransactionsList.paymentstatus = reader["paymentstatus"] == DBNull.Value ? null : Convert.ToString(reader["paymentstatus"]);
								oGetTypeOfPayTransactionsList.paymenttype = reader["paymenttype"] == DBNull.Value ? null : Convert.ToString(reader["paymenttype"]);
								oGetTypeOfPayTransactionsList.total = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
								oGetTypeOfPayTransactionsList.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["balance"]);
								oGetTypeOfPayTransactionsList.isconverted = reader["isconverted"] == DBNull.Value ? false : Convert.ToBoolean(reader["isconverted"]);
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
						oGetTypeOfPayTransactionsRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetTypeOfPayTransactionsRs;
		}

		public Int64 GetInvoiceNumberCount(Int64 registeredPhoneNumber, string typeOfPay)
		{
			Int64 nextInvoiceNumber = 1;

			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();

					string query = "SELECT COALESCE(MAX(invoicenumber), 0) FROM transactions WHERE typeofpay = @TypeOfPay AND registeredphonenumber = @RegisteredPhoneNumber";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TypeOfPay", typeOfPay);
						cmd.Parameters.AddWithValue("@RegisteredPhoneNumber", registeredPhoneNumber);

						object result = cmd.ExecuteScalar();

						if (result != DBNull.Value && result != null)
						{
							nextInvoiceNumber = Convert.ToInt64(result) + 1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error occurred: {ex.Message}");
				throw; 
			}

			return nextInvoiceNumber;
		}

		public TransactionRs SaveDeliveryChallan(TransactionRq otransactionRq, Int64 invoicecount, string typeofpay, bool isconverted)
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
						" shippingaddress, paymentstatus, isconverted, amountdetails, isbankscustomernameupdate, bankscustomername) VALUES(@typeofpay, @invoicenumber, @invoicedate, @stateofsupply, @paymenttype, @total, @received, @balance, @customername, @phonenumber, @registeredphonenumber, @billingaddress," +
						" @shippingaddress, @paymentstatus, @isconverted, @amountdetails, false, @bankscustomername) RETURNING transaction_id";
					cmd.Parameters.AddWithValue("@typeofpay", typeofpay);
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
					cmd.Parameters.AddWithValue("@isconverted",isconverted);
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(otransactionRq.amountdetailslist));
					cmd.Parameters.AddWithValue("@bankscustomername", otransactionRq.customername);
					var transactionId = cmd.ExecuteScalar();
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
								cmdd.CommandText = "INSERT INTO item_details (transaction_id, item, qty, unit, priceperunit, registeredphonenumber, invoicenumber, customername, invoicedate, typeofpay, paymentstatus, taxrate, taxrateamount, discountpercent, discountamount) " +
									"VALUES (@transaction_id, @item, @qty, @unit, @priceperunit, @registeredphonenumber, @invoicenumber, @customername, @invoicedate, @typeofpay, @paymentstatus, @taxrate, @taxrateamount, @discountpercent, @discountamount)";
								cmdd.Parameters.AddWithValue("@transaction_id", transactionId);
								cmdd.Parameters.AddWithValue("@item", itemDetail.item);
								cmdd.Parameters.AddWithValue("@qty", itemDetail.qty);
								cmdd.Parameters.AddWithValue("@unit", itemDetail.unit);
								cmdd.Parameters.AddWithValue("@priceperunit", itemDetail.priceperunit);
								cmdd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
								cmdd.Parameters.AddWithValue("@invoicenumber", otransactionRq.invoicenumber);
								cmdd.Parameters.AddWithValue("@customername", otransactionRq.customername);
								cmdd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
								cmdd.Parameters.AddWithValue("@typeofpay", typeofpay);
								cmdd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
								cmdd.Parameters.AddWithValue("@taxrate", itemDetail.taxrate);
								cmdd.Parameters.AddWithValue("@taxrateamount", itemDetail.taxrateamount);
								cmdd.Parameters.AddWithValue("@discountpercent", itemDetail.discountpercent);
								cmdd.Parameters.AddWithValue("@discountamount", itemDetail.discountamount);
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
						invoicecount = (Convert.ToInt64(result) + 1 );
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return invoicecount;
		}

		public void UpdateDlChallan(Int64 invoicenumber,Int64 registeredphonenumber, string typeofpay)
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
						cmd.CommandText = "UPDATE transactions SET showtransaction = 'DONT SHOW' WHERE invoicenumber = '" + invoicenumber + "' AND registeredphonenumber = " + registeredphonenumber + " AND typeofpay = '" + typeofpay + "'";
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
						cmd.CommandText = "UPDATE item_details SET showtransaction = 'DONT SHOW' WHERE invoicenumber = '" + invoicenumber + "' AND registeredphonenumber = " + registeredphonenumber + " AND typeofpay = '" + typeofpay + "'";
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

		public GetPartyTransactionDetailsRs ConvertToSaleSaleOrder(ConvertToSaleSaleOrderRq oConvertToSaleSaleOrderRq)
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
						" ON tr.transaction_id = ide.transaction_id WHERE tr.invoicenumber = " + oConvertToSaleSaleOrderRq.invoicenumber + " AND tr.registeredphonenumber = " +
						oConvertToSaleSaleOrderRq.registeredphonenumber + " AND tr.typeofpay = '" + oConvertToSaleSaleOrderRq.typeofpay + "' AND tr.customername = '" + oConvertToSaleSaleOrderRq.customername + "'";
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
									oGetPartyTransactionDetailsRs.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
									oGetPartyTransactionDetailsRs.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
									oGetPartyTransactionDetailsRs.stateofsupply = reader["stateofsupply"] == DBNull.Value ? null : Convert.ToString(reader["stateofsupply"]);
									oGetPartyTransactionDetailsRs.paymenttype = reader["paymenttype"] == DBNull.Value ? null : Convert.ToString(reader["paymenttype"]);
									oGetPartyTransactionDetailsRs.total = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
									oGetPartyTransactionDetailsRs.received = reader["received"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["received"]);
									oGetPartyTransactionDetailsRs.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["balance"]);
									oGetPartyTransactionDetailsRs.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
									oGetPartyTransactionDetailsRs.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
									oGetPartyTransactionDetailsRs.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
									oGetPartyTransactionDetailsRs.shippingaddress = reader["shippingaddress"] == DBNull.Value ? null : Convert.ToString(reader["shippingaddress"]);
									recordFetched = false;
								}
								ItemDetailsListRs oItemDetailsListRs = new ItemDetailsListRs();
								oItemDetailsListRs.item = reader["item"] == DBNull.Value ? null : Convert.ToString(reader["item"]);
								oItemDetailsListRs.qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["qty"]);
								oItemDetailsListRs.unit = reader["unit"] == DBNull.Value ? null : Convert.ToString(reader["unit"]);
								oItemDetailsListRs.priceperunit = reader["priceperunit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["priceperunit"]);
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
						oGetPartyTransactionDetailsRs.status = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetPartyTransactionDetailsRs;
		}

		public GetLinkedPaymentTransactionRs GetLinkedPaymentTransaction(Int64 registeredphonenumber, string customername, string typeofpay)
		{
			GetLinkedPaymentTransactionRs oGetLinkedPaymentTransactionRs = new GetLinkedPaymentTransactionRs();
			string sqlquery = string.Empty;
			try
			{
				if(typeofpay == "PAYMENT IN")
				{
					sqlquery = "SELECT invoicenumber, typeofpay, total, linkedamount, balance, customername from transactions where balance > 0 and customername = '" + customername + "' AND registeredphonenumber = " + registeredphonenumber +
						" AND typeofpay in ('SALE','RECEIVABLE OPENING BALANCE', 'ADVANCE OUT')";
				} else if(typeofpay == "PAYMENT OUT")
				{
					sqlquery = "SELECT invoicenumber, typeofpay, total, linkedamount, balance, customername from transactions where balance > 0 and customername = '" + customername + "' AND registeredphonenumber = " + registeredphonenumber +
						" AND typeofpay in ('PURCHASE', 'ADVANCE IN', 'PAYABLE OPENING BALANCE')";
				}
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sqlquery;
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetLinkedPaymentTransactionList oGetLinkedPaymentTransactionList = new GetLinkedPaymentTransactionList();
								oGetLinkedPaymentTransactionList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetLinkedPaymentTransactionList.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetLinkedPaymentTransactionList.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
								oGetLinkedPaymentTransactionList.invoicedate = DateTime.Today; ;
								oGetLinkedPaymentTransactionList.total = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
								oGetLinkedPaymentTransactionList.linkedamount = reader["linkedamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["linkedamount"]);
								oGetLinkedPaymentTransactionList.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["balance"]);
								oGetLinkedPaymentTransactionRs.getLinkedPaymentTransactionList.Add(oGetLinkedPaymentTransactionList);
							}
							oGetLinkedPaymentTransactionRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oGetLinkedPaymentTransactionRs.status = "FAILED";
						}
					}
					else
					{
						oGetLinkedPaymentTransactionRs.status = "FAILED";
						oGetLinkedPaymentTransactionRs.statusmessage = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetLinkedPaymentTransactionRs;
		}
		public string UpdateTransactionDetails(TransactionRq otransactionRq)
		{
			string status = string.Empty;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "UPDATE transactions SET phonenumber = @phonenumber, billingaddress = @billingaddress, shippingaddress = @shippingaddress, invoicedate = @invoicedate, stateofsupply = @stateofsupply," +
						" total = @total, received = @received, balance = @balance, paymenttype = @paymenttype, paymentstatus = @paymentstatus, amountdetails = @amountdetails where registeredphonenumber = " + otransactionRq.registeredphonenumber + " AND invoicenumber = " + otransactionRq.invoicenumber + " AND " +
						" customername = '" + otransactionRq.customername + "' AND typeofpay = '" + otransactionRq.typeofpay + "'";
					cmd.Parameters.AddWithValue("@phonenumber", otransactionRq.phonenumber);
					cmd.Parameters.AddWithValue("@billingaddress", otransactionRq.billingaddress);
					cmd.Parameters.AddWithValue("@shippingaddress", otransactionRq.shippingaddress);
					cmd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
					cmd.Parameters.AddWithValue("@stateofsupply", otransactionRq.stateofsupply);
					cmd.Parameters.AddWithValue("@total", otransactionRq.total);
					cmd.Parameters.AddWithValue("@received", otransactionRq.received);
					cmd.Parameters.AddWithValue("@balance", otransactionRq.balance);
					cmd.Parameters.AddWithValue("@paymenttype", otransactionRq.paymenttype);
					cmd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
					cmd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(otransactionRq.amountdetailslist));
					cmd.ExecuteNonQuery();
					status = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
			return status;
		}

		public async Task<string> GetAmtDetails(TransactionRq otransactionRq)
		{
			TransactionRs otransactionRs = new TransactionRs();
			string status = string.Empty;
			string sqlquery = "SELECT amountdetails from transactions where registeredphonenumber = @registeredphonenumber and typeofpay = @typeofpay and customername = @customername and invoicenumber = @invoicenumber";
			using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
			{
				conn.Open();
				NpgsqlCommand cmd = new NpgsqlCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sqlquery;
				cmd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
				cmd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
				cmd.Parameters.AddWithValue("@customername", otransactionRq.customername);
				cmd.Parameters.AddWithValue("@invoicenumber", otransactionRq.invoicenumber);
				NpgsqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows)
				{
					try
					{
						while (reader.Read())
						{
							status = reader["amountdetails"] == DBNull.Value ? null : Convert.ToString(reader["amountdetails"]);
						}
					}
					catch (Exception ex)
					{
						status = "FAILED";
					}
				}
				else
				{
					status = "FAILED";
				}
			}
			
			return status;
		}

		public string UpdateInsertDeleteItemDetails(TransactionRq otransactionRq)
		{
			string status = string.Empty;
			try
			{
				foreach (var itemDetail in otransactionRq.itemdetailslist)
				{
					string query = string.Empty;
					if (itemDetail.queryoperationtype == "INSERT")
					{
						query = "INSERT INTO item_details (transaction_id, item, qty, unit, priceperunit, registeredphonenumber, invoicenumber, customername, invoicedate, typeofpay, paymentstatus, taxrate, taxrateamount, discountpercent, discountamount) " +
							"VALUES (@transaction_id, @item, @qty, @unit, @priceperunit, @registeredphonenumber, @invoicenumber, @customername, @invoicedate, @typeofpay, @paymentstatus, @taxrate, @taxrateamount, @discountpercent, @discountamount)";
					}
					else if (itemDetail.queryoperationtype == "UPDATE")
					{
						query = "UPDATE item_details SET item = @item, qty = @qty, unit = @unit, priceperunit = @priceperunit, invoicedate = @invoicedate, typeofpay = @typeofpay, paymentstatus = @paymentstatus," +
							"taxrate = @taxrate, taxrateamount = @taxrateamount, discountpercent = @discountpercent, discountamount = @discountamount WHERE registeredphonenumber = " + otransactionRq.registeredphonenumber + " AND transaction_id = " + itemDetail.transactionid + " " +
							"AND item = '" + itemDetail.item + "' AND invoicenumber = " + otransactionRq.invoicenumber;
					}
					else if (itemDetail.queryoperationtype == "DELETE")
					{
						query = "DELETE FROM item_details WHERE registeredphonenumber = " + otransactionRq.registeredphonenumber + " AND transaction_id = " + itemDetail.transactionid + " AND item = '" + itemDetail.item + "' AND invoicenumber = " + otransactionRq.invoicenumber;
					}
					if(!string.IsNullOrEmpty(query))
					{
						using (NpgsqlConnection connn = new NpgsqlConnection(this._connectionFactory))
						{
							connn.Open();
							NpgsqlCommand cmdd = new NpgsqlCommand();
							cmdd.Connection = connn;
							cmdd.CommandType = CommandType.Text;
							cmdd.CommandText = query;
							if (itemDetail.queryoperationtype == "INSERT")
							{
								cmdd.Parameters.AddWithValue("@transaction_id", itemDetail.transactionid);
								cmdd.Parameters.AddWithValue("@registeredphonenumber", otransactionRq.registeredphonenumber);
								cmdd.Parameters.AddWithValue("@customername", otransactionRq.customername);
								cmdd.Parameters.AddWithValue("@invoicenumber", otransactionRq.invoicenumber);
							}
							cmdd.Parameters.AddWithValue("@item", itemDetail.item);
							cmdd.Parameters.AddWithValue("@qty", itemDetail.qty);
							cmdd.Parameters.AddWithValue("@unit", itemDetail.unit);
							cmdd.Parameters.AddWithValue("@priceperunit", itemDetail.priceperunit);
							cmdd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
							cmdd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
							cmdd.Parameters.AddWithValue("@paymentstatus", otransactionRq.paymentstatus);
							cmdd.Parameters.AddWithValue("@taxrate", itemDetail.taxrate);
							cmdd.Parameters.AddWithValue("@taxrateamount", itemDetail.taxrateamount);
							cmdd.Parameters.AddWithValue("@discountpercent", itemDetail.discountpercent);
							cmdd.Parameters.AddWithValue("@discountamount", itemDetail.discountamount);
							cmdd.ExecuteNonQuery();
						}
					}
				}
				status = "SUCCESS";
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
			return status;
		}
		public async Task<bool> UpdateLinkedPaymentTransaction(List<GetLinkedPaymentTransactionList> transactions)
		{
			string paymentstatus = string.Empty;
			Int64 registeredphonenumber = 0;
			Decimal topayparty = 0;
			Decimal toreceivefromparty = 0;
			string partyname = string.Empty;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					using (var transaction = conn.BeginTransaction())
					{
						foreach (var item in transactions)
						{
							registeredphonenumber = item.registeredphonenumber;
							topayparty = item.topayparty;
							toreceivefromparty = item.toreceivefromparty;
							partyname = item.customername;
							if (item.balance == 0)
							{
								paymentstatus = "PAID";
							}
							else if (item.balance > 0)
							{
								paymentstatus = "PARTIAL";
							}
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "UPDATE transactions SET linkedamount = linkedamount + @linkedAmount, balance = @balance, linkedaccount = 'LINKED', received = received + @received, " +
								"paymentstatus = @paymentStatus, paymentinoutinvoicedate = @paymentinoutinvoicedate, paymentininvoicenumber = @paymentininvoicenumber WHERE invoicenumber = @invoicenumber and registeredphonenumber = @registeredphonenumber and typeofpay = @typeofpay" +
								" and customername = @customername";
							cmd.Parameters.AddWithValue("@linkedAmount", item.unused);
							cmd.Parameters.AddWithValue("@received", item.unused);
							cmd.Parameters.AddWithValue("@balance", item.balance);
							cmd.Parameters.AddWithValue("@invoicenumber", item.invoicenumber);
							cmd.Parameters.AddWithValue("@paymentStatus", paymentstatus);
							cmd.Parameters.AddWithValue("@registeredphonenumber", item.registeredphonenumber);
							cmd.Parameters.AddWithValue("@typeofpay", item.typeofpay);
							cmd.Parameters.AddWithValue("@paymentinoutinvoicedate", DateTime.UtcNow);
							cmd.Parameters.AddWithValue("@paymentininvoicenumber",item.paymentininvoicenumber);
							cmd.Parameters.AddWithValue("@customername",item.customername);
							await cmd.ExecuteNonQueryAsync();
						}
						await transaction.CommitAsync();
					}
				}
				using (NpgsqlConnection connn = new NpgsqlConnection(this._connectionFactory))
				{
					connn.Open();
					NpgsqlCommand cmdd = new NpgsqlCommand();
					cmdd.Connection = connn;
					cmdd.CommandType = CommandType.Text;
					cmdd.CommandText = "UPDATE party SET topayparty = @topayparty, toreceivefromparty = @toreceivefromparty WHERE partyname = @partyname and registeredphonenumber = " + registeredphonenumber;
					cmdd.Parameters.AddWithValue("@topayparty", topayparty);
					cmdd.Parameters.AddWithValue("@toreceivefromparty", toreceivefromparty);
					cmdd.Parameters.AddWithValue("@partyname", partyname);
					await cmdd.ExecuteNonQueryAsync();
				}

					return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<GetPartyAmounts> GetTopaypartyreceiveparty(Int64 registeredphonenumber, string customername)
		{
			GetPartyAmounts oGetPartyAmounts = new GetPartyAmounts();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT topayparty, toreceivefromparty from party where partyname = '" + customername + "' AND registeredphonenumber = " + registeredphonenumber;
						
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetPartyAmounts.topayparty = reader["topayparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["topayparty"]);
								oGetPartyAmounts.toreceivefromparty = reader["toreceivefromparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["toreceivefromparty"]);
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
			return oGetPartyAmounts;
		}

		public async Task<UpadatePaymentInOutTrnxRs> UpdatePaymentInOutTrnx(UpadatePaymentInOutTrnxRq oUpadatePaymentInOutTrnxRq)
		{
			UpadatePaymentInOutTrnxRs oUpadatePaymentInOutTrnxRs = new UpadatePaymentInOutTrnxRs();
			string sqlQuery = "INSERT INTO transactions (typeofpay, customername, paymenttype, invoicedate, invoicenumber, received, total, balance, paymentstatus, registeredphonenumber, amountdetails, bankscustomername, isbankscustomernameupdate)" +
				"VALUES(@typeofpay, @customername, @paymenttype, @invoicedate, @invoicenumber, @received, @total, @balance, @paymentstatus, @registeredphonenumber, @amountdetails, @bankscustomername, false)";
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sqlQuery;
					cmd.Parameters.AddWithValue("@typeofpay", oUpadatePaymentInOutTrnxRq.typeofpay);
					cmd.Parameters.AddWithValue("@registeredphonenumber", oUpadatePaymentInOutTrnxRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@customername", oUpadatePaymentInOutTrnxRq.customername);
					cmd.Parameters.AddWithValue("@paymenttype", oUpadatePaymentInOutTrnxRq.paymenttype);
					cmd.Parameters.AddWithValue("@invoicedate", oUpadatePaymentInOutTrnxRq.invoicedate);
					cmd.Parameters.AddWithValue("@invoicenumber", oUpadatePaymentInOutTrnxRq.invoicenumber);
					cmd.Parameters.AddWithValue("@received", oUpadatePaymentInOutTrnxRq.received);
					cmd.Parameters.AddWithValue("@total", oUpadatePaymentInOutTrnxRq.received);
					cmd.Parameters.AddWithValue("@balance", 0);
					cmd.Parameters.AddWithValue("@paymentstatus", "USED");
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(oUpadatePaymentInOutTrnxRq.amountdetails));
					cmd.Parameters.AddWithValue("@bankscustomername", oUpadatePaymentInOutTrnxRq.customername);
					cmd.ExecuteNonQuery();
					oUpadatePaymentInOutTrnxRs.status = "SUCCESS";
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				oUpadatePaymentInOutTrnxRs.status = "FALED";
			}
			return oUpadatePaymentInOutTrnxRs;
		}

		public async Task<UpadatePaymentInOutTrnxRs> InsertAdvanceTrnx(InsertAdvanceTrnxRq oInsertAdvanceTrnxRq)
		{
			UpadatePaymentInOutTrnxRs oUpadatePaymentInOutTrnxRs = new UpadatePaymentInOutTrnxRs();
			string sqlQuery = "INSERT INTO transactions (typeofpay, customername, paymenttype, invoicedate, invoicenumber, received, total, balance, paymentstatus, registeredphonenumber, amountdetails, bankscustomername, isbankscustomernameupdate)" +
				"VALUES(@typeofpay, @customername, @paymenttype, @invoicedate, @invoicenumber, @received, @total, @balance, @paymentstatus, @registeredphonenumber, @amountdetails, @bankscustomername, false)";
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sqlQuery;
					cmd.Parameters.AddWithValue("@typeofpay", oInsertAdvanceTrnxRq.typeofpay);
					cmd.Parameters.AddWithValue("@registeredphonenumber", oInsertAdvanceTrnxRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@customername", oInsertAdvanceTrnxRq.customername);
					cmd.Parameters.AddWithValue("@paymenttype", oInsertAdvanceTrnxRq.paymenttype);
					cmd.Parameters.AddWithValue("@invoicedate", oInsertAdvanceTrnxRq.invoicedate);
					cmd.Parameters.AddWithValue("@invoicenumber", oInsertAdvanceTrnxRq.invoicenumber);
					cmd.Parameters.AddWithValue("@received", oInsertAdvanceTrnxRq.received);
					cmd.Parameters.AddWithValue("@total", oInsertAdvanceTrnxRq.received);
					cmd.Parameters.AddWithValue("@balance", oInsertAdvanceTrnxRq.received);
					cmd.Parameters.AddWithValue("@paymentstatus", "UNPAID");
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(oInsertAdvanceTrnxRq.amountdetails));
					cmd.Parameters.AddWithValue("@bankscustomername", oInsertAdvanceTrnxRq.customername);
					cmd.ExecuteNonQuery();
					oUpadatePaymentInOutTrnxRs.status = "SUCCESS";
				}
				return oUpadatePaymentInOutTrnxRs;

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oUpadatePaymentInOutTrnxRs.status = "FAILED";
			}
			return oUpadatePaymentInOutTrnxRs;
		}

		public async Task<bool> UpdatePartyToPayReceive(InsertAdvanceTrnxRq oInsertAdvanceTrnxRq)
		{
			string sqlQuery = "update party set topayparty = @topayparty, toreceivefromparty = @toreceivefromparty where registeredphonenumber = @registeredphonenumber and partyname = @customername";
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sqlQuery;
					cmd.Parameters.AddWithValue("@registeredphonenumber", oInsertAdvanceTrnxRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@customername", oInsertAdvanceTrnxRq.customername);
					cmd.Parameters.AddWithValue("@topayparty", oInsertAdvanceTrnxRq.topayparty);
					cmd.Parameters.AddWithValue("@toreceivefromparty", oInsertAdvanceTrnxRq.toreceivefromparty);
					cmd.ExecuteNonQuery();
				}
				return true;

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
			return true;
		}

		public async Task<GetUpdatedTrnxInOutValRs> GetUpdatedTrnxInOutVal(GetUpdatedTrnxInOutValRq oGetUpdatedTrnxInOutValRq)
		{
			GetUpdatedTrnxInOutValRs oGetUpdatedTrnxInOutValRs = new GetUpdatedTrnxInOutValRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT invoicenumber, typeofpay, invoicedate, customername, received, amountdetails FROM transactions where registeredphonenumber = @registeredphonenumber and invoicenumber = @invoicenumber and typeofpay = @typeofpay";
					cmd.Parameters.AddWithValue("@registeredphonenumber", oGetUpdatedTrnxInOutValRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@invoicenumber", oGetUpdatedTrnxInOutValRq.invoicenumber);
					cmd.Parameters.AddWithValue("@typeofpay", oGetUpdatedTrnxInOutValRq.typeofpay);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetUpdatedTrnxInOutValRs.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetUpdatedTrnxInOutValRs.received = reader["received"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["received"]);
								oGetUpdatedTrnxInOutValRs.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetUpdatedTrnxInOutValRs.partyname = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
								oGetUpdatedTrnxInOutValRs.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								string amountdetails = reader["amountdetails"] == DBNull.Value ? null : Convert.ToString(reader["amountdetails"]);
								oGetUpdatedTrnxInOutValRs.amountdetails = JsonConvert.DeserializeObject<List<AmountDetails>>(amountdetails);
							}
							oGetUpdatedTrnxInOutValRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oGetUpdatedTrnxInOutValRs.status = "FAILED";
						}
					}
					else
					{
						oGetUpdatedTrnxInOutValRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetUpdatedTrnxInOutValRs;
		}

		public async Task<bool> InsertPaymentInOutTrnx(List<GetLinkedPaymentTransactionList> transactions)
		{
			bool val = false;
			string sqlQuery = "INSERT INTO payementinouttransactions (typeofpay, customername, invoicedate, invoicenumber, registeredphonenumber, linkedamount, paymentininvoicenumber)" +
				"VALUES(@typeofpay, @customername, @invoicedate, @invoicenumber, @registeredphonenumber, @linkedamount, @paymentininvoicenumber)";
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sqlQuery;
					foreach(GetLinkedPaymentTransactionList str in transactions)
					{
						cmd.Parameters.AddWithValue("@typeofpay", str.typeofpay);
						cmd.Parameters.AddWithValue("@registeredphonenumber", str.registeredphonenumber);
						cmd.Parameters.AddWithValue("@customername", str.customername);
						cmd.Parameters.AddWithValue("@invoicedate", str.invoicedate);
						cmd.Parameters.AddWithValue("@invoicenumber", str.invoicenumber);
						cmd.Parameters.AddWithValue("@linkedamount", str.unused);
						cmd.Parameters.AddWithValue("@paymentininvoicenumber", str.paymentininvoicenumber);
						cmd.ExecuteNonQuery();
						val = true;
						
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return val;
		}

		public async Task<bool> InsertAdvanceInOutTrnx(InsertAdvanceTrnxRq oInsertAdvanceTrnxRq)
		{
			bool val = false;
			string sqlQuery = "INSERT INTO payementinouttransactions (typeofpay, customername, invoicedate, invoicenumber, registeredphonenumber, linkedamount, paymentininvoicenumber)" +
				"VALUES(@typeofpay, @customername, @invoicedate, @invoicenumber, @registeredphonenumber, @linkedamount, @paymentininvoicenumber)";
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = sqlQuery;
					cmd.Parameters.AddWithValue("@typeofpay", oInsertAdvanceTrnxRq.typeofpay);
					cmd.Parameters.AddWithValue("@registeredphonenumber", oInsertAdvanceTrnxRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@customername", oInsertAdvanceTrnxRq.customername);
					cmd.Parameters.AddWithValue("@invoicedate", oInsertAdvanceTrnxRq.invoicedate);
					cmd.Parameters.AddWithValue("@invoicenumber", oInsertAdvanceTrnxRq.invoicenumber);
					cmd.Parameters.AddWithValue("@linkedamount", oInsertAdvanceTrnxRq.received);
					cmd.Parameters.AddWithValue("@paymentininvoicenumber", oInsertAdvanceTrnxRq.invoicenumber);
					cmd.ExecuteNonQuery();
					val = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return val;
		}
		public PaymentInOutTrnxRs GetPaymentInOutTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			PaymentInOutTrnxRs oPaymentInOutTrnxRs = new PaymentInOutTrnxRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT invoicenumber, typeofpay, invoicedate, linkedamount  FROM payementinouttransactions where registeredphonenumber = @registeredphonenumber AND paymentininvoicenumber = @paymentininvoicenumber";
					cmd.Parameters.AddWithValue("@typeofpay", oGetPartyTransactionDetailsRq.typeofpay);
					cmd.Parameters.AddWithValue("@registeredphonenumber", oGetPartyTransactionDetailsRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@paymentininvoicenumber", oGetPartyTransactionDetailsRq.invoicenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								ListPaymentInOutTrnxRs oListPaymentInOutTrnxRs = new ListPaymentInOutTrnxRs();
								oListPaymentInOutTrnxRs.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oListPaymentInOutTrnxRs.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oListPaymentInOutTrnxRs.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oListPaymentInOutTrnxRs.linkedamount = reader["linkedamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["linkedamount"]);
								oPaymentInOutTrnxRs.inouttrnxlist.Add(oListPaymentInOutTrnxRs);
							}
							oPaymentInOutTrnxRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							oPaymentInOutTrnxRs.status = "FAILED";
						}
					}
					else
					{
						oPaymentInOutTrnxRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oPaymentInOutTrnxRs;
		}

		public async Task<BankFormRs> SaveBankDetails(BankFormRq oBankFormRq)
		{
			BankFormRs oBankFormRs = new BankFormRs();
			string sqlQuery = "INSERT INTO BankForm (accountdisplayname, openingbalance, asofdate, registeredphonenumber, amount) " +
							  "VALUES (@accountdisplayname, @openingbalance, @asofdate, @registeredphonenumber, @amount)";

			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					using (NpgsqlCommand cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = sqlQuery;
						cmd.Parameters.AddWithValue("@accountdisplayname", oBankFormRq.newaccountdisplayname);
						cmd.Parameters.AddWithValue("@openingbalance", oBankFormRq.newopeningbalance);
						cmd.Parameters.AddWithValue("@asofdate", oBankFormRq.asofdate);
						cmd.Parameters.AddWithValue("@registeredphonenumber", oBankFormRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@amount", oBankFormRq.newopeningbalance);

						cmd.ExecuteNonQuery();
						oBankFormRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				oBankFormRs.status = "FAILED";
				Console.WriteLine("Error occurred: " + ex.Message);
			}
			return oBankFormRs;
		}

		public async Task<bool> IfBankExists(BankFormRq oBankFormRq)
		{
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (NpgsqlCommand cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "SELECT accountdisplayname FROM BankForm WHERE registeredphonenumber = @registeredphonenumber AND accountdisplayname = @accountdisplayname";
						cmd.Parameters.AddWithValue("@registeredphonenumber", oBankFormRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@accountdisplayname", oBankFormRq.newaccountdisplayname);

						var exists = await cmd.ExecuteScalarAsync();

						return exists != null;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return true;
		}

		public async Task<BankFormRs> UpdateBankDetails(BankFormRq oBankFormRq)
		{
			BankFormRs oBankFormRs = new BankFormRs();
			Decimal amount = 0;
			string sqlQuery = "UPDATE BankForm SET accountdisplayname = @accountdisplayname, openingbalance = @openingbalance, asofdate = @asofdate, amount = @amount " +
				  "WHERE registeredphonenumber = @registeredphonenumber and accountdisplayname = @olddisplayname";
			if(oBankFormRq.newopeningbalance > oBankFormRq.oldopeningbalance)
			{
				amount = oBankFormRq.newopeningbalance - oBankFormRq.oldopeningbalance;
				oBankFormRq.amount = oBankFormRq.amount + amount;
			}
			else if (oBankFormRq.newopeningbalance < oBankFormRq.oldopeningbalance)
			{
				amount = oBankFormRq.oldopeningbalance - oBankFormRq.newopeningbalance;
				oBankFormRq.amount = oBankFormRq.amount - amount;
			}
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					using (NpgsqlCommand cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = sqlQuery;

						cmd.Parameters.AddWithValue("@accountdisplayname", oBankFormRq.newaccountdisplayname);
						cmd.Parameters.AddWithValue("@openingbalance", oBankFormRq.newopeningbalance);
						cmd.Parameters.AddWithValue("@asofdate", oBankFormRq.asofdate);
						cmd.Parameters.AddWithValue("@registeredphonenumber", oBankFormRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@amount", oBankFormRq.amount);
						cmd.Parameters.AddWithValue("@olddisplayname", oBankFormRq.oldaccountdisplayname);
						cmd.ExecuteNonQuery();
						oBankFormRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				oBankFormRs.status = "FAILED";
				Console.WriteLine(ex.Message);
			}
			return oBankFormRs;
		}

		public async Task<GetBankDetailsRs> GetBankDetailsAsync(GetBankDetailsRq oGetBankDetailsRq)
		{
			GetBankDetailsRs oGetBankDetailsRs = new GetBankDetailsRs();
			List<AmountDetails> amount = new List<AmountDetails>();
			string result = "SUCCESS";

			string sqlQuery = @"SELECT typeofpay, bankscustomername, invoicedate, amountdetails, paymenttype, transaction_id
                        FROM transactions
                        WHERE registeredphonenumber = @registeredphonenumber
                        AND (paymenttype LIKE '%' || @customername || '%' 
                        AND (paymenttype = @customername 
                        OR paymenttype LIKE @customername || ',%' 
                        OR paymenttype LIKE '%,' || @customername 
                        OR paymenttype LIKE '%,' || @customername || ',%'))";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlQuery, conn))
					{
						cmd.CommandType = CommandType.Text;

						// Define and add parameters
						cmd.Parameters.AddWithValue("@registeredphonenumber", oGetBankDetailsRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@customername", oGetBankDetailsRq.accountdisplayname);

						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (await reader.ReadAsync())
								{
									BankTrnxDetails oBankTrnxDetails = new BankTrnxDetails();
									oBankTrnxDetails.customername = reader["bankscustomername"] == DBNull.Value ? null : Convert.ToString(reader["bankscustomername"]);
									oBankTrnxDetails.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
									oBankTrnxDetails.invoicedate = reader["invoicedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["invoicedate"]);
									oBankTrnxDetails.transactionid = reader["transaction_id"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["transaction_id"]);
									amount = reader["amountdetails"] == DBNull.Value ? null : JsonConvert.DeserializeObject<List<AmountDetails>>(Convert.ToString(reader["amountdetails"]));
									if (amount != null)
									{
										foreach (var amountDetail in amount)
										{
											if (amountDetail.type == oGetBankDetailsRq.accountdisplayname)
											{
												oBankTrnxDetails.amount = amountDetail.amount;
												Console.WriteLine($"Match found for account display name: {amountDetail.type}");
												Console.WriteLine($"Amount: {amountDetail.amount}");
											}
										}
									}
									oGetBankDetailsRs.bankTrnxDetails.Add(oBankTrnxDetails);
								}
								oGetBankDetailsRs.status = "SUCCESS";
							}
							else
							{
								// Log the parameters and query for debugging
								Console.WriteLine("Query executed but no rows were returned.");
								Console.WriteLine($"Parameters: registeredphonenumber={oGetBankDetailsRq.registeredphonenumber}, customername={oGetBankDetailsRq.accountdisplayname}");
								oGetBankDetailsRs.status = "SUCCESS";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetBankDetailsRs;
		}

		public async Task<List<BankTrnxDetailsVal>> GetBankDetailsAsyncVal(GetBankDetailsRq oGetBankDetailsRq)
		{
			List<BankTrnxDetailsVal> olistamtval = new List<BankTrnxDetailsVal>();
			string result = "SUCCESS";

			string sqlQuery = @"SELECT typeofpay, bankscustomername, invoicedate, amountdetails, paymenttype, transaction_id, isbankscustomernameupdate
                        FROM transactions
                        WHERE registeredphonenumber = @registeredphonenumber
                        AND (paymenttype LIKE '%' || @customername || '%' 
                        AND (paymenttype = @customername 
                        OR paymenttype LIKE @customername || ',%' 
                        OR paymenttype LIKE '%,' || @customername 
                        OR paymenttype LIKE '%,' || @customername || ',%'))";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlQuery, conn))
					{
						cmd.CommandType = CommandType.Text;

						// Define and add parameters
						cmd.Parameters.AddWithValue("@registeredphonenumber", oGetBankDetailsRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@customername", oGetBankDetailsRq.accountdisplayname);

						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (await reader.ReadAsync())
								{
									BankTrnxDetailsVal oBankTrnxDetailsVal = new BankTrnxDetailsVal();
									oBankTrnxDetailsVal.customername = reader["bankscustomername"] == DBNull.Value ? null : Convert.ToString(reader["bankscustomername"]);
									oBankTrnxDetailsVal.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
									oBankTrnxDetailsVal.paymenttype = reader["paymenttype"] == DBNull.Value ? null : Convert.ToString(reader["paymenttype"]);
									oBankTrnxDetailsVal.invoicedate = reader["invoicedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["invoicedate"]);
									oBankTrnxDetailsVal.transactionid = reader["transaction_id"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["transaction_id"]);
									oBankTrnxDetailsVal.amtdetails = reader["amountdetails"] == DBNull.Value ? null : Convert.ToString(reader["amountdetails"]);
									oBankTrnxDetailsVal.isbankscustomernameupdate = reader["isbankscustomernameupdate"] == DBNull.Value ? false : Convert.ToBoolean(reader["isbankscustomernameupdate"]);
									olistamtval.Add(oBankTrnxDetailsVal);
								}
							}
							else
							{
								// Log the parameters and query for debugging
								Console.WriteLine("Query executed but no rows were returned.");
								Console.WriteLine($"Parameters: registeredphonenumber={oGetBankDetailsRq.registeredphonenumber}, customername={oGetBankDetailsRq.accountdisplayname}");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return olistamtval;
		}

		public async Task<List<BankTrnxDetailsVal>> GetRecordsToUpdate(GetBankDetailsRq oGetBankDetailsRq)
		{
			List<BankTrnxDetailsVal> records = new List<BankTrnxDetailsVal>();
			string sqlQuery = @"
        SELECT typeofpay, bankscustomername, invoicedate, amountdetails, paymenttype, transaction_id
        FROM transactions
        WHERE registeredphonenumber = @registeredphonenumber
        AND (
            bankscustomername LIKE '%FROM: ' || @customername || '%'
            OR bankscustomername LIKE '%TO: ' || @customername || '%'
        )
    ";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlQuery, conn))
					{
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@registeredphonenumber", oGetBankDetailsRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@customername", oGetBankDetailsRq.accountdisplayname);

						using (var reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var record = new BankTrnxDetailsVal
								{
									typeofpay = reader.GetString(0),
									customername = reader.GetString(1),
									invoicedate = reader.GetDateTime(2),
									paymenttype = reader.GetString(4),
									transactionid = reader.GetInt64(5),
								};
								records.Add(record);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Handle exceptions as needed
				Console.WriteLine($"Error: {ex.Message}");
			}

			return records;
		}

		public List<BankTrnxDetailsVal> ProcessRecords(List<BankTrnxDetailsVal> records, string newCustomerName)
		{
			foreach (var record in records)
			{
				if (record.customername.Contains("FROM: "))
				{
					record.customername = record.customername.Replace(record.customername.Substring(record.customername.IndexOf("FROM: ") + 6), newCustomerName);
				}
				else if (record.customername.Contains("TO: "))
				{
					record.customername = record.customername.Replace(record.customername.Substring(record.customername.IndexOf("TO: ") + 4), newCustomerName);
				}
			}

			return records;
		}

		public async Task UpdateRecords(List<BankTrnxDetailsVal> records)
		{
			string sqlQuery = @"
        UPDATE transactions
        SET bankscustomername = @customername
        WHERE transaction_id = @transaction_id and isbankscustomernameupdate = true
    ";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var transaction = conn.BeginTransaction())
					{
						foreach (var record in records)
						{
							using (var cmd = new NpgsqlCommand(sqlQuery, conn))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@customername", record.customername);
								cmd.Parameters.AddWithValue("@transaction_id", record.transactionid);

								await cmd.ExecuteNonQueryAsync();
							}
						}

						transaction.Commit();
					}
				}
			}
			catch (Exception ex)
			{
				// Handle exceptions as needed
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

		public async Task UpdateCustomerNames(GetBankDetailsRq oGetBankDetailsRq, string newCustomerName)
		{
			var records = await GetRecordsToUpdate(oGetBankDetailsRq);

			var updatedRecords = ProcessRecords(records, newCustomerName);

			await UpdateRecords(updatedRecords);
		}


		public async Task<string> UpdateBankAmount(List<AmountDetails> oAmountDetails, Int64 registeredphonenumber, string typeofpay)
		{
			string result = string.Empty;
			string sqlQueryAdd = "UPDATE BankForm " +
								"SET amount = amount + @amount " +
								"WHERE registeredphonenumber = @registeredphonenumber and accountdisplayname = @type";
			string sqlQuerySubtract = "UPDATE BankForm " +
								"SET amount = amount - @amount " +
								"WHERE registeredphonenumber = @registeredphonenumber and accountdisplayname = @type";
			string query = string.Empty;
			if(typeofpay == "SALE")
			{
				query = sqlQueryAdd;
			}
			else if (typeofpay == "PURCHASE")
			{
				query = sqlQuerySubtract;
			}


			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();

					foreach (var details in oAmountDetails)
					{
						using (var cmd = new NpgsqlCommand(query, conn))
						{
							cmd.CommandType = CommandType.Text;

							cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
							cmd.Parameters.AddWithValue("@amount", details.amount);
							cmd.Parameters.AddWithValue("@type", details.type);
							await cmd.ExecuteNonQueryAsync();
						}
					}
					result = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				result = "FAILED";
				Console.WriteLine(ex.Message);
			}
			return result;
		}

		public async Task<string> UpdateBankAmountDetails(List<AmountDetails> oAmountDetails, Int64 registeredphonenumber, List<AmountDetails> amt, string typeofpay)
		{
			string result = string.Empty;
			string query = string.Empty;

			// SQL queries to update the amount in the BankForm table
			string sqlQueryAdd = "UPDATE BankForm " +
								 "SET amount = amount + @amount " +  // Add amount back
								 "WHERE registeredphonenumber = @registeredphonenumber and accountdisplayname = @type";

			string sqlQuerySubtract = "UPDATE BankForm " +
									  "SET amount = amount - @amount " +  // Subtract amount from new account
									  "WHERE registeredphonenumber = @registeredphonenumber and accountdisplayname = @type";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();

					// Iterate through amt to handle removed items
					foreach (var oldAmt in amt)
					{
						var matchingNewAmt = oAmountDetails.FirstOrDefault(a => a.type == oldAmt.type);
						if (matchingNewAmt == null)
						{
							// If the type in amt is not found in oAmountDetails, revert the amount
							if (typeofpay == "SALE")
							{
								query = sqlQuerySubtract;
							}
							else if (typeofpay == "PURCHASE")
							{
								query = sqlQueryAdd;
							}

							using (var cmd = new NpgsqlCommand(query, conn))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
								cmd.Parameters.AddWithValue("@amount", oldAmt.amount);
								cmd.Parameters.AddWithValue("@type", oldAmt.type);
								await cmd.ExecuteNonQueryAsync();
							}
						}
					}

					// Iterate through oAmountDetails to handle updates and additions
					foreach (var details in oAmountDetails)
					{
						var matchingAmt = amt.FirstOrDefault(a => a.type == details.type);

						if (matchingAmt != null)
						{
							// If the type is found in amt, handle based on type
							decimal amountDifference = details.amount - matchingAmt.amount;

							if (typeofpay == "SALE")
							{
								query = sqlQueryAdd;
							}
							else if (typeofpay == "PURCHASE")
							{
								query = sqlQuerySubtract;
							}

							using (var cmd = new NpgsqlCommand(query, conn))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
								cmd.Parameters.AddWithValue("@amount", amountDifference);
								cmd.Parameters.AddWithValue("@type", details.type);
								await cmd.ExecuteNonQueryAsync();
							}
						}
						else
						{
							// If the type is not found in amt, handle as a new addition
							if (typeofpay == "SALE")
							{
								query = sqlQueryAdd;
							}
							else if (typeofpay == "PURCHASE")
							{
								query = sqlQuerySubtract;
							}

							using (var cmd = new NpgsqlCommand(query, conn))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
								cmd.Parameters.AddWithValue("@amount", details.amount);
								cmd.Parameters.AddWithValue("@type", details.type);
								await cmd.ExecuteNonQueryAsync();
							}
						}
					}

					result = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				result = "FAILED";
				Console.WriteLine(ex.Message);
			}
			return result;
		}



		public async Task<GetBanksRs> GetBanks(GetBanksRq oGetBanksRq)
		{
			GetBanksRs oGetBanksRs = new GetBanksRs();
			string sqlQuery = @"SELECT accountdisplayname, amount from BankForm where registeredphonenumber = @registeredphonenumber";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlQuery, conn))
					{
						cmd.CommandType = CommandType.Text;

						// Define and add parameters
						cmd.Parameters.AddWithValue("@registeredphonenumber", oGetBanksRq.registeredphonenumber);

						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (await reader.ReadAsync())
								{
									BanksList oBanksList = new BanksList();
									oBanksList.accountdisplayname = reader["accountdisplayname"] == DBNull.Value ? null : Convert.ToString(reader["accountdisplayname"]);
									oBanksList.amount = reader["amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["amount"]);
									oGetBanksRs.bankslist.Add(oBanksList);
								}
								oGetBanksRs.status = "SUCCESS";
							}
							else
							{
								oGetBanksRs.status = "SUCCESS";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetBanksRs;
		}

		public async Task<bool> InsertTrnx(BankFormRq oBankFormRq)
		{
			bool val = false;
			string sqlQuery = "INSERT INTO transactions (typeofpay, bankscustomername, invoicedate, total, balance, registeredphonenumber, showtransaction, amountdetails, paymenttype) " +
							  "VALUES (@typeofpay, @customername, @invoicedate, @total, @balance, @registeredphonenumber, 'DONT SHOW', @amountdetails, @paymenttype)";

			try
			{
				List<AmountDetails> amounts = new List<AmountDetails>();
				AmountDetails amountDetails = new AmountDetails();
				amountDetails.type = oBankFormRq.newaccountdisplayname;
				amountDetails.amount = oBankFormRq.newopeningbalance;
				amounts.Add(amountDetails);
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					using (NpgsqlCommand cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = sqlQuery;
						cmd.Parameters.AddWithValue("@typeofpay", oBankFormRq.typeofpay);
						cmd.Parameters.AddWithValue("@customername", oBankFormRq.newaccountdisplayname);
						cmd.Parameters.AddWithValue("@invoicedate", oBankFormRq.asofdate);
						cmd.Parameters.AddWithValue("@total", oBankFormRq.newopeningbalance);
						cmd.Parameters.AddWithValue("@balance", oBankFormRq.newopeningbalance);
						cmd.Parameters.AddWithValue("@registeredphonenumber", oBankFormRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(amounts));
						cmd.Parameters.AddWithValue("@paymenttype", oBankFormRq.newaccountdisplayname);

						cmd.ExecuteNonQuery();
						val = true;
					}
				}
			}
			catch (Exception ex)
			{
				val = false;
				Console.WriteLine("Error occurred: " + ex.Message);
			}
			return val;
		}

		public async Task<bool> UpdatetTrnx(BankFormRq oBankFormRq)
		{
			bool val = false;
			string sqlQuery = "update transactions set bankscustomername = @customername, invoicedate = @invoicedate, total = @total, balance = @balance" +
							  " where registeredphonenumber = @registeredphonenumber and customername = @oldcustomername";

			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					using (NpgsqlCommand cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = sqlQuery;
						cmd.Parameters.AddWithValue("@typeofpay", oBankFormRq.typeofpay);
						cmd.Parameters.AddWithValue("@customername", oBankFormRq.newaccountdisplayname);
						cmd.Parameters.AddWithValue("@invoicedate", oBankFormRq.asofdate);
						cmd.Parameters.AddWithValue("@total", oBankFormRq.newopeningbalance);
						cmd.Parameters.AddWithValue("@balance", oBankFormRq.newopeningbalance);
						cmd.Parameters.AddWithValue("@registeredphonenumber", oBankFormRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@oldcustomername", oBankFormRq.oldaccountdisplayname);

						cmd.ExecuteNonQuery();
						val = true;
					}
				}
			}
			catch (Exception ex)
			{
				val = false;
				Console.WriteLine("Error occurred: " + ex.Message);
			}
			return val;
		}
		

		public async Task<bool> UpdateInternalValuesOfpayment(List<BankTrnxDetailsVal> olistamtval, BankFormRq oBankFormRq)
		{
			bool updateSuccess = true;
			string searchString = oBankFormRq.oldaccountdisplayname;
			string replaceString = oBankFormRq.newaccountdisplayname;

			foreach (var bankDetail in olistamtval)
			{
				bool isUpdated = false;

				if (!string.IsNullOrEmpty(bankDetail.paymenttype) && bankDetail.paymenttype.Contains(searchString))
				{
					bankDetail.paymenttype = bankDetail.paymenttype.Replace(searchString, replaceString);
					isUpdated = true;
				}

				if (!string.IsNullOrEmpty(bankDetail.customername) && bankDetail.customername.Contains(searchString) && bankDetail.isbankscustomernameupdate)
				{
					bankDetail.customername = bankDetail.customername.Replace(searchString, replaceString);
					isUpdated = true;
				}

				var amountDetails = JsonConvert.DeserializeObject<List<AmountDetails>>(bankDetail.amtdetails);

				foreach (var detail in amountDetails)
				{
					if (!string.IsNullOrEmpty(detail.type) && detail.type.Contains(searchString))
					{
						detail.type = detail.type.Replace(searchString, replaceString);
						isUpdated = true;
					}
				}

				if (isUpdated)
				{
					bankDetail.amtdetails = JsonConvert.SerializeObject(amountDetails);

					bool dbUpdateSuccess = await UpdateBankTransactionAsync(bankDetail);
					if (!dbUpdateSuccess)
					{
						updateSuccess = false;
					}
				}
			}

			return updateSuccess;
		}

		public async Task<bool> UpdateBankTransactionAsync(BankTrnxDetailsVal bankDetail)
		{
			try
			{
				string updateQuery = @"UPDATE transactions
                               SET paymenttype = @paymenttype, amountdetails = @amountdetails, bankscustomername = @customername
                               WHERE transaction_id = @transaction_id";

				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(updateQuery, conn))
					{
						cmd.CommandType = CommandType.Text;

						cmd.Parameters.AddWithValue("@paymenttype", bankDetail.paymenttype ?? (object)DBNull.Value);
						cmd.Parameters.AddWithValue("@amountdetails", bankDetail.amtdetails ?? (object)DBNull.Value);
						cmd.Parameters.AddWithValue("@transaction_id", bankDetail.transactionid);
						cmd.Parameters.AddWithValue("@customername", bankDetail.customername);

						await cmd.ExecuteNonQueryAsync();
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to update transaction ID {bankDetail.transactionid}: {ex.Message}");
				return false;
			}
		}

		public async Task<GetBanksDetailsValuesRs> GetBanksDetailsValues(GetBanksDetailsValuesRq oGetBanksDetailsValuesRq)
		{
			GetBanksDetailsValuesRs oGetBanksDetailsValuesRs = new GetBanksDetailsValuesRs();
			string sqlQuery = @"SELECT accountdisplayname, openingbalance, asofdate, amount from BankForm where registeredphonenumber = @registeredphonenumber and accountdisplayname = @accountdisplayname";

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlQuery, conn))
					{
						cmd.CommandType = CommandType.Text;

						// Define and add parameters
						cmd.Parameters.AddWithValue("@registeredphonenumber", oGetBanksDetailsValuesRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@accountdisplayname", oGetBanksDetailsValuesRq.newaccountdisplayname);

						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (await reader.ReadAsync())
								{
									oGetBanksDetailsValuesRs.newaccountdisplayname = reader["accountdisplayname"] == DBNull.Value ? null : Convert.ToString(reader["accountdisplayname"]);
									oGetBanksDetailsValuesRs.newopeningbalance = reader["openingbalance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["openingbalance"]);
									oGetBanksDetailsValuesRs.amount = reader["amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["amount"]);
									oGetBanksDetailsValuesRs.asofDate = reader["asofdate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["asofdate"]);
								}
								oGetBanksDetailsValuesRs.status = "SUCCESS";
							}
							else
							{
								oGetBanksDetailsValuesRs.status = "FAILED";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetBanksDetailsValuesRs;
		}

		public async Task<GetTransferDetailsValuesRs> GetTransferDetailsValues(GetTransferDetailsValuesRq oGetTransferDetailsValuesRq)
		{
			GetTransferDetailsValuesRs oGetTransferDetailsValuesRs = new GetTransferDetailsValuesRs();
			string sqlQuery = @"SELECT typeofpay, invoicedate, total, paymenttype, bankscustomername from transactions where registeredphonenumber = @registeredphonenumber and transaction_id = @transaction_id and typeofpay = @typeofpay";
			string from = string.Empty;
			string to = string.Empty;
			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlQuery, conn))
					{
						cmd.CommandType = CommandType.Text;

						// Define and add parameters
						cmd.Parameters.AddWithValue("@registeredphonenumber", oGetTransferDetailsValuesRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@transaction_id", oGetTransferDetailsValuesRq.transactionid);
						cmd.Parameters.AddWithValue("@typeofpay", oGetTransferDetailsValuesRq.typeofpay);

						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (await reader.ReadAsync())
								{
									oGetTransferDetailsValuesRs.customername = reader["paymenttype"] == DBNull.Value ? null : Convert.ToString(reader["paymenttype"]);
									oGetTransferDetailsValuesRs.banktobank = reader["bankscustomername"] == DBNull.Value ? null : Convert.ToString(reader["bankscustomername"]);
									oGetTransferDetailsValuesRs.amount = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
									oGetTransferDetailsValuesRs.adjustmentDate = reader["invoicedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["invoicedate"]);
									if (!string.IsNullOrEmpty(oGetTransferDetailsValuesRs.banktobank))
									{
										if (oGetTransferDetailsValuesRs.banktobank.Contains("FROM:"))
										{
											from = oGetTransferDetailsValuesRs.customername;
										}
										string result = oGetTransferDetailsValuesRs.banktobank.Replace("FROM:", "").Replace("TO:", "");
										oGetTransferDetailsValuesRs.banktobank = result.Trim();
										to = oGetTransferDetailsValuesRs.banktobank;
										if (!string.IsNullOrEmpty(from))
										{
											oGetTransferDetailsValuesRs.customername = to;
											oGetTransferDetailsValuesRs.banktobank = from;
										}
										
									}
								}
								oGetTransferDetailsValuesRs.status = "SUCCESS";
							}
							else
							{
								oGetTransferDetailsValuesRs.status = "FAILED";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetTransferDetailsValuesRs;
		}

		public async Task<TransfersRs> Transfers(TransfersRq oTransfersRq, string sqlquery, int num)
		{
			TransfersRs oTransfersRs = new TransfersRs();

			try
			{
				using (var conn = new NpgsqlConnection(this._connectionFactory))
				{
					await conn.OpenAsync();
					using (var cmd = new NpgsqlCommand(sqlquery, conn))
					{
						cmd.CommandType = CommandType.Text;

						if(oTransfersRq.type == "bankToCash")
						{
							cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@accountdisplayname", oTransfersRq.fromAccount);
							cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
						}
						else if (oTransfersRq.type == "cashToBank")
						{
							cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@accountdisplayname", oTransfersRq.toAccount);
							cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
						}
						else if (oTransfersRq.type == "bankToBank")
						{
							if(num == 1)
							{
								cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@accountdisplayname", oTransfersRq.fromAccount);
								cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
							}
							else if (num == 2)
							{
								cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@accountdisplayname", oTransfersRq.toAccount);
								cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
							}
							
						}
						else if (oTransfersRq.type == "adjustBalance")
						{
							cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@accountdisplayname", oTransfersRq.accountName);
							cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
						}
						cmd.ExecuteNonQuery();
						oTransfersRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oTransfersRs.status = "FAILED";
			}
			return oTransfersRs;
		}

		public async Task<TransfersRs> InsertTransfers(TransfersRq oTransfersRq, string sqlquery, List<AmountDetails> amtDetails, int num)
		{
			TransfersRs oTransfersRs = new TransfersRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					using (NpgsqlCommand cmd = new NpgsqlCommand())
					{
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = sqlquery;
						if (oTransfersRq.type == "bankToCash")
						{
							cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@invoicedate", oTransfersRq.adjustmentDate);
							cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
							cmd.Parameters.AddWithValue("@paymenttype", oTransfersRq.fromAccount);
							cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(amtDetails));
						}
						else if (oTransfersRq.type == "cashToBank")
						{
							cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@invoicedate", oTransfersRq.adjustmentDate);
							cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
							cmd.Parameters.AddWithValue("@paymenttype", oTransfersRq.toAccount);
							cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(amtDetails));
						}
						else if (oTransfersRq.type == "bankToBank")
						{
							if (num == 1)
							{
								cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@invoicedate", oTransfersRq.adjustmentDate);
								cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
								cmd.Parameters.AddWithValue("@paymenttype", oTransfersRq.fromAccount);
								cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(amtDetails));
								cmd.Parameters.AddWithValue("@customername", "TO: " + oTransfersRq.toAccount);
							}
							else if (num == 2)
							{
								cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@invoicedate", oTransfersRq.adjustmentDate);
								cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
								cmd.Parameters.AddWithValue("@paymenttype", oTransfersRq.toAccount);
								cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(amtDetails));
								cmd.Parameters.AddWithValue("@customername", "FROM: " + oTransfersRq.fromAccount);
							}
						}
						else if (oTransfersRq.type == "adjustBalance")
						{
							cmd.Parameters.AddWithValue("@registeredphonenumber", oTransfersRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@invoicedate", oTransfersRq.adjustmentDate);
							cmd.Parameters.AddWithValue("@amount", oTransfersRq.amount);
							cmd.Parameters.AddWithValue("@paymenttype", oTransfersRq.accountName);
							cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(amtDetails));
						}

						cmd.ExecuteNonQuery();
						oTransfersRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occurred: " + ex.Message);
				oTransfersRs.status = "FAILED";
			}
			return oTransfersRs;
		}
	}
}