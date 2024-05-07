using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
					Int64 transactionId = (Int64)cmd.ExecuteScalar();
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
					cmd.CommandText = "SELECT tr.invoicenumber, tr.typeofpay, tr.invoicedate, tr.total, tr.balance, tr.phonenumber, pr.emailid, pr.billingaddress, pr.creditlimit, pr.gst, pr.creditlimit FROM transactions tr join party pr ON tr.customername = pr.partyname" +
						" where tr.registeredphonenumber = " + oGetPartyTransactionsRq.registeredphonenumber + " AND " +
						"tr.customername = '" + oGetPartyTransactionsRq.customername + "' AND tr.showtransaction = 'SHOW'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oGetPartyTransactionsRs.gst = reader["gst"] == DBNull.Value ? null : Convert.ToString(reader["gst"]);
								oGetPartyTransactionsRs.emailid = reader["emailid"] == DBNull.Value ? null : Convert.ToString(reader["emailid"]);
								oGetPartyTransactionsRs.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
								oGetPartyTransactionsRs.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
								oGetPartyTransactionsRs.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["creditlimit"]);
								GetAllPartyTransactionsList oGetAllPartyTransactionsList = new GetAllPartyTransactionsList(); 
								oGetAllPartyTransactionsList.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetAllPartyTransactionsList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetAllPartyTransactionsList.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["creditlimit"]);
								oGetAllPartyTransactionsList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetAllPartyTransactionsList.total = reader["total"] == DBNull.Value ? 0 : Convert.ToInt64(reader["total"]);
								oGetAllPartyTransactionsList.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["balance"]);
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
						"tr.phonenumber, tr.billingaddress, tr.shippingaddress, ide.item, ide,qty, ide.unit, ide.priceperunit, ide.transaction_id, ide.taxrate, ide.taxrateamount, ide.discountpercent, ide.discountamount FROM transactions tr join item_details ide" +
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
									oGetPartyTransactionDetailsRs.total = reader["total"] == DBNull.Value ? 0 : Convert.ToInt64(reader["total"]);
									oGetPartyTransactionDetailsRs.received = reader["received"] == DBNull.Value ? 0 : Convert.ToInt64(reader["received"]);
									oGetPartyTransactionDetailsRs.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["balance"]);
									oGetPartyTransactionDetailsRs.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
									oGetPartyTransactionDetailsRs.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
									oGetPartyTransactionDetailsRs.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
									oGetPartyTransactionDetailsRs.shippingaddress = reader["shippingaddress"] == DBNull.Value ? null : Convert.ToString(reader["shippingaddress"]);
									recordFetched = false;
								}
								ItemDetailsListRs oItemDetailsListRs = new ItemDetailsListRs();
								oItemDetailsListRs.item = reader["item"] == DBNull.Value ? null : Convert.ToString(reader["item"]);
								oItemDetailsListRs.qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["qty"]);
								oItemDetailsListRs.unit = reader["unit"] == DBNull.Value ? null : Convert.ToString(reader["unit"]);
								oItemDetailsListRs.priceperunit = reader["priceperunit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["priceperunit"]);
								oItemDetailsListRs.transactionid = reader["transaction_id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["transaction_id"]);
								oItemDetailsListRs.taxrate = reader["taxrate"] == DBNull.Value ? null : Convert.ToString(reader["taxrate"]);
								oItemDetailsListRs.taxrateamount = reader["taxrateamount"] == DBNull.Value ? 0 : Convert.ToInt64(reader["taxrateamount"]);
								oItemDetailsListRs.discountpercent = reader["discountpercent"] == DBNull.Value ? 0 : Convert.ToInt64(reader["discountpercent"]);
								oItemDetailsListRs.discountamount = reader["discountamount"] == DBNull.Value ? 0 : Convert.ToInt64(reader["discountamount"]);
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
								oGetAllItemTransactionsList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetAllItemTransactionsList.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetAllItemTransactionsList.partyname = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
								oGetAllItemTransactionsList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetAllItemTransactionsList.qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["qty"]);
								oGetAllItemTransactionsList.priceperunit = reader["priceperunit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["priceperunit"]);
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
								oGetItemTransactionsRs.saleprice = reader["saleprice"] == DBNull.Value ? 0 : Convert.ToInt64(reader["saleprice"]);
								oGetItemTransactionsRs.wholesaleprice = reader["wholesaleprice"] == DBNull.Value ? 0 : Convert.ToInt64(reader["wholesaleprice"]);
								oGetItemTransactionsRs.purchaseprice = reader["purchaseprice"] == DBNull.Value ? 0 : Convert.ToInt64(reader["purchaseprice"]);
								oGetItemTransactionsRs.remainingquantity = reader["remainingquantity"] == DBNull.Value ? 0 : Convert.ToInt64(reader["remainingquantity"]);
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
								oGetTypeOfPayTransactionsList.invoicenumber = reader["invoicenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["invoicenumber"]);
								oGetTypeOfPayTransactionsList.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								oGetTypeOfPayTransactionsList.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
								oGetTypeOfPayTransactionsList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetTypeOfPayTransactionsList.paymentstatus = reader["paymentstatus"] == DBNull.Value ? null : Convert.ToString(reader["paymentstatus"]);
								oGetTypeOfPayTransactionsList.paymenttype = reader["paymenttype"] == DBNull.Value ? null : Convert.ToString(reader["paymenttype"]);
								oGetTypeOfPayTransactionsList.total = reader["total"] == DBNull.Value ? 0 : Convert.ToInt64(reader["total"]);
								oGetTypeOfPayTransactionsList.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["balance"]);
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
					cmd.Parameters.AddWithValue("@typeofpay", otransactionRq.typeofpay);
					cmd.Parameters.AddWithValue("@invoicenumber", invoicecount);
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
									oGetPartyTransactionDetailsRs.total = reader["total"] == DBNull.Value ? 0 : Convert.ToInt64(reader["total"]);
									oGetPartyTransactionDetailsRs.received = reader["received"] == DBNull.Value ? 0 : Convert.ToInt64(reader["received"]);
									oGetPartyTransactionDetailsRs.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["balance"]);
									oGetPartyTransactionDetailsRs.customername = reader["customername"] == DBNull.Value ? null : Convert.ToString(reader["customername"]);
									oGetPartyTransactionDetailsRs.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
									oGetPartyTransactionDetailsRs.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
									oGetPartyTransactionDetailsRs.shippingaddress = reader["shippingaddress"] == DBNull.Value ? null : Convert.ToString(reader["shippingaddress"]);
									recordFetched = false;
								}
								ItemDetailsListRs oItemDetailsListRs = new ItemDetailsListRs();
								oItemDetailsListRs.item = reader["item"] == DBNull.Value ? null : Convert.ToString(reader["item"]);
								oItemDetailsListRs.qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["qty"]);
								oItemDetailsListRs.unit = reader["unit"] == DBNull.Value ? null : Convert.ToString(reader["unit"]);
								oItemDetailsListRs.priceperunit = reader["priceperunit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["priceperunit"]);
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

		public GetLinkedPaymentTransactionRs GetLinkedPaymentTransaction(Int64 registeredphonenumber, string customername)
		{
			GetLinkedPaymentTransactionRs oGetLinkedPaymentTransactionRs = new GetLinkedPaymentTransactionRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT invoicenumber, typeofpay, invoicedate, total, linkedamount, balance from transactions where(balance > 0 OR linkedaccount = 'LINKED') and customername = '" + customername + "' AND registeredphonenumber = " + registeredphonenumber +
						" AND typeofpay in ('SALE', 'DELIVERY CHALLAN','RECEIVABLE OPENING BALANCE');";
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
								oGetLinkedPaymentTransactionList.invoicedate = reader["invoicedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["invoicedate"]);
								oGetLinkedPaymentTransactionList.total = reader["total"] == DBNull.Value ? 0 : Convert.ToInt64(reader["total"]);
								oGetLinkedPaymentTransactionList.linkedamount = reader["linkedamount"] == DBNull.Value ? 0 : Convert.ToInt64(reader["linkedamount"]);
								oGetLinkedPaymentTransactionList.balance = reader["balance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["balance"]);
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
						oGetLinkedPaymentTransactionRs.status = "No Recods Found";
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
						" total = @total, received = @received, balance = @balance, paymenttype = @paymenttype where registeredphonenumber = " + otransactionRq.registeredphonenumber + " AND invoicenumber = " + otransactionRq.invoicenumber + " AND " +
						" customername = '" + otransactionRq.customername + "'" ;
					cmd.Parameters.AddWithValue("@phonenumber", otransactionRq.phonenumber);
					cmd.Parameters.AddWithValue("@billingaddress", otransactionRq.billingaddress);
					cmd.Parameters.AddWithValue("@shippingaddress", otransactionRq.shippingaddress);
					cmd.Parameters.AddWithValue("@invoicedate", otransactionRq.invoicedate);
					cmd.Parameters.AddWithValue("@stateofsupply", otransactionRq.stateofsupply);
					cmd.Parameters.AddWithValue("@total", otransactionRq.total);
					cmd.Parameters.AddWithValue("@received", otransactionRq.received);
					cmd.Parameters.AddWithValue("@balance", otransactionRq.balance);
					cmd.Parameters.AddWithValue("@paymenttype", otransactionRq.paymenttype);
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
				status = "SUCCESS";
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
			return status;
		}

	}
}
