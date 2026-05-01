using Npgsql;
using NpgsqlTypes;
using Org.BouncyCastle.Tls.Crypto;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using WebApplication1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication1.DL
{
	public class LoginDL
	{
		private readonly string _connectionFactory; 
		private static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		private static readonly string dbConn = root.GetValue<string>("ConnectionStrings");

		public LoginDL(IConfiguration configuration)
		{
			this._connectionFactory = configuration.GetValue<string>("ConnectionStrings");
		}

		public LoginRs GetLoginDetails(LoginRq ologinRq)
		{
			LoginRs ologinrs = new LoginRs();
			try
			{
				using(NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = @"SELECT status, expirydate, plantype
                FROM registeragent
                WHERE phonenumber = @phonenumber
                  AND _password = @password";
					cmd.Parameters.AddWithValue("@phonenumber", ologinRq.phonenumber);
					cmd.Parameters.AddWithValue("@password", ologinRq.password);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						ologinrs.status = "SUCCESS";
						ologinrs.statusMessage = reader["status"].ToString();
						ologinrs.expiryDate = Convert.ToDateTime(reader["expirydate"]);
						ologinrs.plantype =  reader["plantype"] == DBNull.Value ? "" : Convert.ToString(reader["plantype"]);
						return ologinrs;
					}
					ologinrs.status = "FAILED";
					ologinrs.statusMessage = "Record Not Found";
				}
			}
			catch(Exception ex)
			{
				ologinrs.status = "FAILED";
				ologinrs.statusMessage = "Record Not Found";
			}
			return ologinrs;
		}

		public bool GetOtpLoginDetails(string phonenumber)
		{
			try
			{
				Console.WriteLine(this._connectionFactory);
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT status, expirydate FROM registeragent WHERE phonenumber = @phonenumber";
					cmd.Parameters.AddWithValue("@phonenumber", Convert.ToInt64(phonenumber));
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return false;
		}

		public OtpRs VerifyOtpser(VerifyOtpRequest request)
		{
			OtpRs otpRs = new OtpRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT status, expirydate, plantype FROM registeragent WHERE phonenumber = @phonenumber";
					cmd.Parameters.AddWithValue("@phonenumber", Convert.ToInt64(request.registeredphonenumber));
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						otpRs.status = "SUCCESS";
						otpRs.statusmessage = reader["status"].ToString();
						otpRs.expiryDate = Convert.ToDateTime(reader["expirydate"]);
						otpRs.plantype = reader["plantype"] == DBNull.Value ? "" : Convert.ToString(reader["plantype"]);
					}
				}
			}
			catch (Exception ex)
			{
				otpRs.status = "FAILED";
				return otpRs;
			}
			return otpRs;
		}

		public RegisterRs RegisterUser(RegisterRq oregisterRq)
		{
			RegisterRs oregisterrs = new RegisterRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT phonenumber FROM registeragent WHERE phonenumber = @phonenumber";
					cmd.Parameters.AddWithValue("@phonenumber", oregisterRq.phonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						oregisterrs.status = "Phone Number already exists.";
						oregisterrs.stat = "Failed";
						oregisterrs.exist = true;
					}
				}
				if(!oregisterrs.exist)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "INSERT INTO registeragent (phonenumber, _password, _state, address) VALUES (@phonenumber, @_password, @_state, @address)";
							cmd.Parameters.AddWithValue("@phonenumber", oregisterRq.phonenumber);
							cmd.Parameters.AddWithValue("@_password", oregisterRq.password);
							cmd.Parameters.AddWithValue("@_state", oregisterRq.state);
							cmd.Parameters.AddWithValue("@address", oregisterRq.address);
							cmd.ExecuteNonQuery();
							oregisterrs.status = "Inserted Successfully";
							oregisterrs.stat = "Success";

						}
					}
					catch (Exception ex)
					{
						oregisterrs.status = "Data Could Not Be Inserted";
						oregisterrs.stat = "Failed";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
			return oregisterrs;
		}

		public PartyRs Party(PartyRq opartyRq)
		{
			PartyRs opartyRs = new PartyRs();
			Boolean partyexist = false;
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
						cmd.CommandText = "SELECT partyname FROM party WHERE registeredphonenumber = @registeredphonenumber AND partyname = @partyname";
						cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@partyname", opartyRq.partyname);
						NpgsqlDataReader reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							opartyRs.status = "Failed";
							opartyRs.statusmessage = "Party Name Already Exists";
							return opartyRs;
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				if (!partyexist)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "INSERT INTO party (typeofpay, registeredphonenumber, partyname, gst, phonenumber, partygroup, gsttype, _state, emailid, billingaddress, shippingaddress, openingbalance, topayorreceive, asofdate, creditlimit," +
								" additionalfieldname1, additionalfieldname2, additionalfieldname3, additionalfieldname4, additionalfieldname1value, additionalfieldname2value, additionalfieldname3value, additionalfieldname4value, topayparty, toreceivefromparty) VALUES (@typeofpay, @registeredphonenumber, @partyname, @gst, @phonenumber, @partygroup, @gsttype, @_state, @emailid, @billingaddress, " +
								"@shippingaddress, @openingbalance, @topayorreceive, @asofdate, @creditlimit, @additionalfieldname1, @additionalfieldname2, @additionalfieldname3, @additionalfieldname4, @additionalfieldname1value, @additionalfieldname2value, @additionalfieldname3value, @additionalfieldname4value, @topayparty, @toreceiveparty)";

							cmd.Parameters.AddWithValue("typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("partyname", opartyRq.partyname);
							cmd.Parameters.AddWithValue("gst", opartyRq.GST);
							cmd.Parameters.AddWithValue("phonenumber", opartyRq.phonenumber);
							cmd.Parameters.AddWithValue("partygroup", opartyRq.partygroup);
							cmd.Parameters.AddWithValue("gsttype", opartyRq.gsttype);
							cmd.Parameters.AddWithValue("_state", opartyRq._state);
							cmd.Parameters.AddWithValue("emailid", opartyRq.emailid);
							cmd.Parameters.AddWithValue("billingaddress", opartyRq.billingaddress);
							cmd.Parameters.AddWithValue("shippingaddress", opartyRq.shippingaddress);
							cmd.Parameters.AddWithValue("openingbalance", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("topayorreceive", opartyRq.topayorreceive);
							cmd.Parameters.AddWithValue("asofdate", opartyRq.asofdate);
							cmd.Parameters.AddWithValue("creditlimit", opartyRq.creditlimit);
							cmd.Parameters.AddWithValue("additionalfieldname1", opartyRq.additionalfieldname1);
							cmd.Parameters.AddWithValue("additionalfieldname2", opartyRq.additionalfieldname2);
							cmd.Parameters.AddWithValue("additionalfieldname3", opartyRq.additionalfieldname3);
							cmd.Parameters.AddWithValue("additionalfieldname4", opartyRq.additionalfieldname4);
							cmd.Parameters.AddWithValue("additionalfieldname1value", opartyRq.additionalfieldname1value);
							cmd.Parameters.AddWithValue("additionalfieldname2value", opartyRq.additionalfieldname2value);
							cmd.Parameters.AddWithValue("additionalfieldname3value", opartyRq.additionalfieldname3value);
							cmd.Parameters.AddWithValue("additionalfieldname4value", opartyRq.additionalfieldname4value);
							cmd.Parameters.AddWithValue("topayparty", opartyRq.topayparty);
							cmd.Parameters.AddWithValue("toreceiveparty", opartyRq.toreceivefromparty);
							cmd.ExecuteNonQuery();
							opartyRs.status = "Success";
							opartyRs.statusmessage = "Inserted Successfully";
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					
				}
				if (opartyRq.typeofpay == "RECEIVABLE OPENING BALANCE" || opartyRq.typeofpay == "PAYABLE OPENING BALANCE")
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "INSERT INTO transactions(typeofpay, invoicedate, total, received, balance, customername, phonenumber, registeredphonenumber," +
								"paymentstatus, invoicenumber) VALUES(@typeofpay, @invoicedate, @total, 0, @balance, @customername, @phonenumber, @registeredphonenumber," +
								"@paymentstatus, 1) RETURNING transaction_id";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@invoicedate", DateTime.UtcNow);
							cmd.Parameters.AddWithValue("@total", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@balance", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phonenumber);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@paymentstatus", "UNPAID");
							var transactionId = cmd.ExecuteScalar();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (!string.IsNullOrEmpty(opartyRq.partygroup))
				{
					bool partygroupexist = false;
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
								cmd.CommandText = "SELECT partygroup FROM partygroup WHERE registeredphonenumber = @registeredphonenumber AND partygroup = @partygroup";
								cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@partygroup", opartyRq.partygroup);
								NpgsqlDataReader reader = cmd.ExecuteReader();
								while (reader.Read())
								{
									partygroupexist = true;
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{
							if (!partygroupexist)
							{
								using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
								{
									conn.Open();
									NpgsqlCommand cmd = new NpgsqlCommand();
									cmd.Connection = conn;
									cmd.CommandType = CommandType.Text;
									cmd.CommandText = "INSERT INTO partygroup(partygroup, registeredphonenumber) VALUES(@partygroup, @registeredphonenumber)";
									cmd.Parameters.AddWithValue("@partygroup", opartyRq.partygroup);
									cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
									cmd.ExecuteNonQuery();
									opartyRs.status = "Success";
									opartyRs.statusmessage = "Inserted Successfully";
								}
							}
						}
						catch(Exception ex)
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
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return opartyRs;
		}

		public PartyRs UpdateParty(PartyRq opartyRq)
		{
			PartyRs opartyRs = new PartyRs();
			Boolean partyexist = false;
			try
			{
				if(opartyRq.partyname != opartyRq.oldpartyname)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "SELECT partyname FROM party WHERE registeredphonenumber = @registeredphonenumber AND partyname = @partyname";
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@partyname", opartyRq.partyname);
							NpgsqlDataReader reader = cmd.ExecuteReader();
							while (reader.Read())
							{
								opartyRs.status = "Failed";
								opartyRs.statusmessage = "Party Name Already Exists";
								return opartyRs;
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (!partyexist)
				{
					// Read current running totals so we adjust rather than overwrite accumulated transaction history.
					decimal oldOpeningBalance = 0;
					string oldTopayOrReceive = "";
					decimal currentTopay = 0;
					decimal currentToreceive = 0;
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "SELECT openingbalance, topayorreceive, topayparty, toreceivefromparty FROM party WHERE registeredphonenumber = @registeredphonenumber AND partyname = @oldpartyname";
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@oldpartyname", opartyRq.oldpartyname);
							NpgsqlDataReader reader = cmd.ExecuteReader();
							if (reader.HasRows && reader.Read())
							{
								oldOpeningBalance = reader["openingbalance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["openingbalance"]);
								oldTopayOrReceive = reader["topayorreceive"] as string ?? "";
								currentTopay = reader["topayparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["topayparty"]);
								currentToreceive = reader["toreceivefromparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["toreceivefromparty"]);
							}
						}
					}
					catch (Exception ex) { Console.WriteLine(ex.Message); }

					// Remove old opening balance contribution from running totals.
					if (oldTopayOrReceive.ToUpper() == "RECEIVE")
						currentToreceive -= oldOpeningBalance;
					else if (oldTopayOrReceive.ToUpper() == "PAY")
						currentTopay -= oldOpeningBalance;

					// Add new opening balance contribution to running totals.
					string newTopayOrReceive = opartyRq.topayorreceive?.ToUpper() ?? "";
					if (newTopayOrReceive == "RECEIVE")
						currentToreceive += opartyRq.openingbalance;
					else if (newTopayOrReceive == "PAY")
						currentTopay += opartyRq.openingbalance;

					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "UPDATE party SET partyname = @partyname, typeofpay = @typeofpay, gst = @gst, phonenumber = @phonenumber, partygroup = @partygroup, gsttype = @gsttype, _state = @_state, emailid = @emailid, " +
											  "billingaddress = @billingaddress, " + "shippingaddress = @shippingaddress, openingbalance = @openingbalance, topayorreceive = @topayorreceive, asofdate = @asofdate, " +
											  "creditlimit = @creditlimit, additionalfieldname1 = @additionalfieldname1," + "additionalfieldname2 = @additionalfieldname2, additionalfieldname3 = @additionalfieldname3, " +
											  "additionalfieldname4 = @additionalfieldname4," + " additionalfieldname1value = @additionalfieldname1value, " +
											  "additionalfieldname2value = @additionalfieldname2value, additionalfieldname3value = @additionalfieldname3value, " +
											  "additionalfieldname4value = @additionalfieldname4value, topayparty = @topayparty, toreceivefromparty = @toreceivefromparty WHERE registeredphonenumber = @registeredphonenumber AND partyname = @oldpartyname";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@partyname", opartyRq.partyname);
							cmd.Parameters.AddWithValue("@gst", opartyRq.GST);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phonenumber);
							cmd.Parameters.AddWithValue("@partygroup", opartyRq.partygroup);
							cmd.Parameters.AddWithValue("@gsttype", opartyRq.gsttype);
							cmd.Parameters.AddWithValue("@_state", opartyRq._state);
							cmd.Parameters.AddWithValue("@emailid", opartyRq.emailid);
							cmd.Parameters.AddWithValue("@billingaddress", opartyRq.billingaddress);
							cmd.Parameters.AddWithValue("@shippingaddress", opartyRq.shippingaddress);
							cmd.Parameters.AddWithValue("@openingbalance", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@topayorreceive", opartyRq.topayorreceive);
							cmd.Parameters.AddWithValue("@asofdate", opartyRq.asofdate);
							cmd.Parameters.AddWithValue("@creditlimit", opartyRq.creditlimit);
							cmd.Parameters.AddWithValue("@additionalfieldname1", opartyRq.additionalfieldname1);
							cmd.Parameters.AddWithValue("@additionalfieldname2", opartyRq.additionalfieldname2);
							cmd.Parameters.AddWithValue("@additionalfieldname3", opartyRq.additionalfieldname3);
							cmd.Parameters.AddWithValue("@additionalfieldname4", opartyRq.additionalfieldname4);
							cmd.Parameters.AddWithValue("@additionalfieldname1value", opartyRq.additionalfieldname1value);
							cmd.Parameters.AddWithValue("@additionalfieldname2value", opartyRq.additionalfieldname2value);
							cmd.Parameters.AddWithValue("@additionalfieldname3value", opartyRq.additionalfieldname3value);
							cmd.Parameters.AddWithValue("@additionalfieldname4value", opartyRq.additionalfieldname4value);
							cmd.Parameters.AddWithValue("@topayparty", currentTopay);
							cmd.Parameters.AddWithValue("@toreceivefromparty", currentToreceive);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@oldpartyname", opartyRq.oldpartyname);
							cmd.ExecuteNonQuery();
							opartyRs.status = "Success";
							opartyRs.statusmessage = "Party Updated Successfully";
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				// Always delete existing opening balance transactions first (handles amount change,
				// type switch RECEIVE<->PAY, and opening balance removal cleanly).
				try
				{
					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = @"DELETE FROM transactions
                WHERE registeredphonenumber = @registeredphonenumber
                  AND customername = @oldpartyname
                  AND (typeofpay = 'RECEIVABLE OPENING BALANCE' OR typeofpay = 'PAYABLE OPENING BALANCE')
                  AND invoicenumber = 1";
						cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@oldpartyname", opartyRq.oldpartyname);
						cmd.ExecuteNonQuery();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				// Insert fresh opening balance transaction with updated amount/type.
				if (opartyRq.typeofpay == "RECEIVABLE OPENING BALANCE" || opartyRq.typeofpay == "PAYABLE OPENING BALANCE")
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "INSERT INTO transactions (typeofpay, invoicedate, total, received, balance, customername, phonenumber, registeredphonenumber, paymentstatus, invoicenumber)" +
											  " VALUES (@typeofpay, @invoicedate, @total, 0, @balance, @customername, @phonenumber, @registeredphonenumber, @paymentstatus, 1)";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@invoicedate", DateTime.UtcNow);
							cmd.Parameters.AddWithValue("@total", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@balance", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phonenumber);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@paymentstatus", "UNPAID");
							cmd.ExecuteNonQuery();
							opartyRs.status = "Success";
							opartyRs.statusmessage = "Party Updated Successfully";
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (!string.IsNullOrEmpty(opartyRq.partygroup))
				{
					bool partygroupexist = false;
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
								cmd.CommandText = "SELECT partygroup FROM partygroup WHERE registeredphonenumber = @registeredphonenumber AND partygroup = @partygroup";
								cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
								cmd.Parameters.AddWithValue("@partygroup", opartyRq.partygroup);
								NpgsqlDataReader reader = cmd.ExecuteReader();
								while (reader.Read())
								{
									partygroupexist = true;
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{
							if (!partygroupexist)
							{
								using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
								{
									conn.Open();
									NpgsqlCommand cmd = new NpgsqlCommand();
									cmd.Connection = conn;
									cmd.CommandType = CommandType.Text;
									cmd.CommandText = "INSERT INTO partygroup(partygroup, registeredphonenumber) VALUES(@partygroup, @registeredphonenumber)";
									cmd.Parameters.AddWithValue("@partygroup", opartyRq.partygroup);
									cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
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
				try
				{
					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = @"UPDATE transactions
                SET customername = @customername
                WHERE registeredphonenumber = @registeredphonenumber
                  AND customername = @oldpartyname";
						cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@oldpartyname", opartyRq.oldpartyname);
						cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
						cmd.ExecuteNonQuery();
					}

					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = @"UPDATE transactions
                SET bankscustomername = @customername
                WHERE registeredphonenumber = @registeredphonenumber
                  AND bankscustomername = @oldpartyname
                  AND isbankscustomernameupdate = false";
						cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
						cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@oldpartyname", opartyRq.oldpartyname);
						cmd.ExecuteNonQuery();
					}

					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = @"UPDATE item_details
                SET customername = @customername
                WHERE registeredphonenumber = @registeredphonenumber
                  AND customername = @oldpartyname";
						cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
						cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
						cmd.Parameters.AddWithValue("@oldpartyname", opartyRq.oldpartyname);
						cmd.ExecuteNonQuery();
					}
				}
				catch(Exception ex)
				{
					opartyRs.status = "Failed";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return opartyRs;
		}

		public GetPartyRs GetPartyDetails(Int64 registeredPhoneNumber, string partyName)
		{
			GetPartyRs ogetPartyRs = new GetPartyRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					string query = @"
                SELECT gst, phonenumber, partygroup, gsttype, _state, emailid, billingaddress, shippingaddress, 
                       openingbalance, asofdate, creditlimit, additionalfieldname1, 
                       additionalfieldname2, additionalfieldname3, additionalfieldname4, typeofpay, topayorreceive, 
                       topayparty, toreceivefromparty, additionalfieldname1value, additionalfieldname2value, 
                       additionalfieldname3value, additionalfieldname4value 
                FROM party 
                WHERE registeredphonenumber = @registeredPhoneNumber AND partyname = @partyName";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@registeredPhoneNumber", registeredPhoneNumber);
						cmd.Parameters.AddWithValue("@partyName", partyName);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.HasRows)
							{
								while (reader.Read())
								{
									GetAllPartyList ogetallpartylist = new GetAllPartyList
									{
										GST = reader["gst"] as string,
										phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]),
										partygroup = reader["partygroup"] as string,
										gsttype = reader["gsttype"] as string,
										_state = reader["_state"] as string,
										emailid = reader["emailid"] as string,
										billingaddress = reader["billingaddress"] as string,
										shippingaddress = reader["shippingaddress"] as string,
										openingbalance = reader["openingbalance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["openingbalance"]),
										asofdate = reader["asofdate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["asofdate"]),
										creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["creditlimit"]),
										additionalfieldname1 = reader["additionalfieldname1"] as string,
										additionalfieldname2 = reader["additionalfieldname2"] as string,
										additionalfieldname3 = reader["additionalfieldname3"] as string,
										additionalfieldname4 = reader["additionalfieldname4"] as string,
										typeofpay = reader["typeofpay"] as string,
										topayorreceive = reader["topayorreceive"] as string,
										topayparty = reader["topayparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["topayparty"]),
										toreceivefromparty = reader["toreceivefromparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["toreceivefromparty"]),
										additionalfieldname1value = reader["additionalfieldname1value"] as string,
										additionalfieldname2value = reader["additionalfieldname2value"] as string,
										additionalfieldname3value = reader["additionalfieldname3value"] as string,
										additionalfieldname4value = reader["additionalfieldname4value"] as string
									};

									ogetPartyRs.partyList.Add(ogetallpartylist);
								}

								ogetPartyRs.status = "SUCCESS";
							}
							else
							{
								ogetPartyRs.status = "FAILED";
								ogetPartyRs.statusMessage = "No Records Found";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ogetPartyRs.status = "ERROR";
				ogetPartyRs.statusMessage = ex.Message;
			}
			return ogetPartyRs;
		}

		public GetPartyListRs GetPartyList(Int64 registeredphonenumber)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			Decimal topayparty = 0;
			Decimal toreceivefromparty = 0;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = @"SELECT partyname, topayparty, toreceivefromparty, shippingaddress, billingaddress, phonenumber, creditlimit
                FROM party
                WHERE registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetPartyList oGetPartyList = new GetPartyList();
								oGetPartyList.partyname = reader["partyname"] == DBNull.Value ? "" : Convert.ToString(reader["partyname"]);
								oGetPartyList.topayparty = reader["topayparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["topayparty"]);
								oGetPartyList.toreceivefromparty = reader["toreceivefromparty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["toreceivefromparty"]);
								oGetPartyList.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["creditlimit"]);
								oGetPartyList.shippingaddress = reader["shippingaddress"] == DBNull.Value ? "" : Convert.ToString(reader["shippingaddress"]);
								oGetPartyList.billingaddress = reader["billingaddress"] == DBNull.Value ? "" : Convert.ToString(reader["billingaddress"]);
								oGetPartyList.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
								oGetPartyListRs.getPartyList.Add(oGetPartyList);

							}
							oGetPartyListRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oGetPartyListRs.status = "FAILED";
						}
					}
					oGetPartyListRs.status = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oGetPartyListRs.status = "FAILED";
			}
			return oGetPartyListRs;
		}
		public GetPartyGroupRs GetPartyGroup(Int64 registeredphonenumber)
		{
			GetPartyGroupRs oGetPartyGroupRs = new GetPartyGroupRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = @"SELECT pg.partygroup, COUNT(par.partygroup) AS partygroup_count
                FROM partygroup AS pg
                LEFT JOIN party AS par ON pg.partygroup = par.partygroup AND par.registeredphonenumber = @registeredphonenumber
                WHERE pg.registeredphonenumber = @registeredphonenumber
                GROUP BY pg.partygroup";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);

					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetPartyGroupListtRs oGetPartyGroupListtRs = new GetPartyGroupListtRs();
								oGetPartyGroupListtRs.partygroup = Convert.ToString(reader["partygroup"]);
								oGetPartyGroupListtRs.partygroupcount = Convert.ToInt64(reader["partygroup_count"]);
								oGetPartyGroupRs.getPartyGroupList.Add(oGetPartyGroupListtRs);
							}
							oGetPartyGroupRs.status = "SUCCESS";
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oGetPartyGroupRs.status = "FAILED";
						}
					}
					oGetPartyGroupRs.status = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oGetPartyGroupRs.status = "FAILED";
			}
			return oGetPartyGroupRs;
		}

		public GetPartyByGroupRs GetPartyByGroup(Int64 registeredphonenumber, string groupname)
		{
			GetPartyByGroupRs oGetPartyByGroupRs = new GetPartyByGroupRs();
			Decimal topayparty = 0;
			Decimal toreceivefromparty = 0;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = @"SELECT partyname, topayparty, toreceivefromparty
                FROM party
                WHERE registeredphonenumber = @registeredphonenumber
                  AND partygroup = @groupname";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					cmd.Parameters.AddWithValue("@groupname", groupname);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetPartyList oGetPartyList = new GetPartyList();
								oGetPartyList.partyname = Convert.ToString(reader["partyname"]);
								oGetPartyList.topayparty = Convert.ToInt64(reader["topayparty"]);
								oGetPartyList.toreceivefromparty = Convert.ToInt64(reader["toreceivefromparty"]);
								oGetPartyByGroupRs.getPartyList.Add(oGetPartyList);
								oGetPartyByGroupRs.status = "SUCCESS";
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oGetPartyByGroupRs.status = "FAILED";
						}
					}
					else
					{
						oGetPartyByGroupRs.status = "SUCCESS";
						oGetPartyByGroupRs.statusmessage = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oGetPartyByGroupRs.status = "FAILED";
			}
			return oGetPartyByGroupRs;
		}

		public AddUpdatePartyGropRs AddUpdatePartyGroup(AddUpdatePartyGropRq oAddUpdatePartyGropRq)
		{
			AddUpdatePartyGropRs oAddUpdatePartyGropRs = new AddUpdatePartyGropRs();
			string outputResult = string.Empty;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(dbConn))
				{
					conn.Open(); // Open the connection outside the loop

						using (NpgsqlCommand cmd = new NpgsqlCommand("sp_addupdatepartygroup", conn))
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.AddWithValue("v_oldgroupname", NpgsqlDbType.Varchar).Value = oAddUpdatePartyGropRq.oldgroupname;
							cmd.Parameters.AddWithValue("v_newgroupname", NpgsqlDbType.Varchar).Value = oAddUpdatePartyGropRq.newgroupname;
							cmd.Parameters.AddWithValue("v_registeredphonenumber", NpgsqlDbType.Numeric).Value = oAddUpdatePartyGropRq.registeredphonenumber;
							var outputParameter = new NpgsqlParameter("output_result", NpgsqlDbType.Varchar);
							outputParameter.Direction = ParameterDirection.Output;
							cmd.Parameters.Add(outputParameter);
							cmd.ExecuteNonQuery();
							outputResult = cmd.Parameters["output_result"].Value.ToString();
						}
				}
				oAddUpdatePartyGropRs.statusmessage = outputResult;
				oAddUpdatePartyGropRs.status= "SUCCESS";
				if (outputResult == "Record exists")
				{
					oAddUpdatePartyGropRs.status = "FAILED";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oAddUpdatePartyGropRs.statusmessage = outputResult;
				oAddUpdatePartyGropRs.status = "FAILED";
			}
			return oAddUpdatePartyGropRs;
		}

        public GetBusinessInfoRs GetBusinessInfo(Int64 registeredphonenumber)
        {
            GetBusinessInfoRs oGetBusinessInfoRs = new GetBusinessInfoRs();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM AddBusinessInformation WHERE registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                BusinessInfo businessInfo = new BusinessInfo
                                {
                                    businessName = Convert.ToString(reader["businessName"]),
                                    gstin = Convert.ToString(reader["gstin"]),
                                    phoneNumber = Convert.ToInt64(reader["phoneNumber"]),
                                    emailId = Convert.ToString(reader["emailId"]),
                                    businessAddress = Convert.ToString(reader["businessAddress"]),
                                    businessType = Convert.ToString(reader["businessType"]),
                                    businessCategory = Convert.ToString(reader["businessCategory"]),
                                    pincode = Convert.ToDecimal(reader["pincode"]),
                                    state = Convert.ToString(reader["state"]),
                                    businessDescription = Convert.ToString(reader["businessDescription"])
                                };

                                oGetBusinessInfoRs.businessInfo = businessInfo;
                            }
                            oGetBusinessInfoRs.status = "SUCCESS";
							oGetBusinessInfoRs.statusmsg = "Record fetched successfully";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
							oGetBusinessInfoRs.status = "FAILED";
						}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return oGetBusinessInfoRs;
        }

        public AddBusinessInformationRs AddUpdateBusinessInformation(AddBusinessInformationRq OAddBusinessInformationRq)
        {
            AddBusinessInformationRs OAddBusinessInformationRs = new AddBusinessInformationRs();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT registeredphonenumber FROM AddBusinessInformation WHERE registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", OAddBusinessInformationRq.registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        OAddBusinessInformationRs.statusmsg = "Phone Number already registered.";
                        OAddBusinessInformationRs.exist = true;
                    }
                }
                if (!OAddBusinessInformationRs.exist)
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
                        {
                            conn.Open();
                            NpgsqlCommand cmd = new NpgsqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO AddBusinessInformation(registeredphonenumber, businessName, gstin, phoneNumber, emailId, businessAddress, businessType, " +
								"businessCategory, pincode, state, businessDescription) VALUES (@registeredphonenumber, @businessName, @gstin, @phoneNumber, @emailId, @businessAddress," +
								" @businessType, @businessCategory, @pincode, @state, @businessDescription)";
                            cmd.Parameters.AddWithValue("@registeredphonenumber", OAddBusinessInformationRq.registeredphonenumber);
                            cmd.Parameters.AddWithValue("@businessName", OAddBusinessInformationRq.businessName);
                            cmd.Parameters.AddWithValue("@gstin", OAddBusinessInformationRq.gstin);
                            cmd.Parameters.AddWithValue("@phoneNumber", OAddBusinessInformationRq.phoneNumber);
                            cmd.Parameters.AddWithValue("@emailId", OAddBusinessInformationRq.emailId);
                            cmd.Parameters.AddWithValue("@businessAddress", OAddBusinessInformationRq.businessAddress);
                            cmd.Parameters.AddWithValue("@businessType", OAddBusinessInformationRq.businessType);
                            cmd.Parameters.AddWithValue("@businessCategory", OAddBusinessInformationRq.businessCategory);
                            cmd.Parameters.AddWithValue("@pincode", OAddBusinessInformationRq.pincode);
                            cmd.Parameters.AddWithValue("@state", OAddBusinessInformationRq.state);
                            cmd.Parameters.AddWithValue("@businessDescription", OAddBusinessInformationRq.businessDescription);
                            cmd.ExecuteNonQuery();
                            OAddBusinessInformationRs.statusmsg = "Business Details registered Successfully";
                            OAddBusinessInformationRs.status = "Success";
                            OAddBusinessInformationRs.exist = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        OAddBusinessInformationRs.statusmsg = "Data could not be inserted";
                        OAddBusinessInformationRs.status = "Failed";
                    }
                }
                if (OAddBusinessInformationRs.exist)
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
                        {
                            conn.Open();
                            NpgsqlCommand cmd = new NpgsqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "UPDATE AddBusinessInformation SET businessName = @businessName, gstin = @gstin, phoneNumber = @phoneNumber, emailId = @emailId, businessAddress = @businessAddress, " +
                                              "businessType = @businessType, businessCategory = @businessCategory, pincode = @pincode, state = @state, businessDescription = @businessDescription " +
                                              "WHERE registeredphonenumber = @registeredphonenumber";
                            cmd.Parameters.AddWithValue("@registeredphonenumber", OAddBusinessInformationRq.registeredphonenumber);
                            cmd.Parameters.AddWithValue("@businessName", OAddBusinessInformationRq.businessName);
                            cmd.Parameters.AddWithValue("@gstin", OAddBusinessInformationRq.gstin);
                            cmd.Parameters.AddWithValue("@phoneNumber", OAddBusinessInformationRq.phoneNumber);
                            cmd.Parameters.AddWithValue("@emailId", OAddBusinessInformationRq.emailId);
                            cmd.Parameters.AddWithValue("@businessAddress", OAddBusinessInformationRq.businessAddress);
                            cmd.Parameters.AddWithValue("@businessType", OAddBusinessInformationRq.businessType);
                            cmd.Parameters.AddWithValue("@businessCategory", OAddBusinessInformationRq.businessCategory);
                            cmd.Parameters.AddWithValue("@pincode", OAddBusinessInformationRq.pincode);
                            cmd.Parameters.AddWithValue("@state", OAddBusinessInformationRq.state);
                            cmd.Parameters.AddWithValue("@businessDescription", OAddBusinessInformationRq.businessDescription);
                            cmd.ExecuteNonQuery();
                            OAddBusinessInformationRs.statusmsg = "Business Details Updated Successfully";
                            OAddBusinessInformationRs.status = "Success";
                            OAddBusinessInformationRs.exist = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        OAddBusinessInformationRs.statusmsg = "Data could not be inserted";
                        OAddBusinessInformationRs.status = "Failed";
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return OAddBusinessInformationRs;
        }

        public bool UpdateExpiryDate(DateTime date, Int64 registeredphonenumber, string planType)
		{
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "UPDATE registeragent SET expirydate = @expirydate, plantype = @plantype WHERE phonenumber = @phonenumber";
					cmd.Parameters.AddWithValue("@expirydate", date);
					cmd.Parameters.AddWithValue("@plantype", planType);
					cmd.Parameters.AddWithValue("@phonenumber", registeredphonenumber);

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		public DashboardDetailsRs DashBoardDetails(Int64 registeredphonenumber)
		{
			DashboardDetailsRs oDashboardDetailsRs = new DashboardDetailsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "select partyname, topayparty, toreceivefromparty from party where registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								Youllpayreceive oYoullpayreceive = new Youllpayreceive();
								oYoullpayreceive.partyname = Convert.ToString(reader["partyname"]);
								oYoullpayreceive.partyreceive = Convert.ToDouble(reader["toreceivefromparty"]);
								oYoullpayreceive.partypay = Convert.ToDouble(reader["topayparty"]);
								oDashboardDetailsRs.youllpayreceiveparty.Add(oYoullpayreceive);
								oDashboardDetailsRs.youllreceive += oYoullpayreceive.partyreceive;
								oDashboardDetailsRs.youllpay += oYoullpayreceive.partypay;
							}
						}
						catch (Exception ex)
						{
                            oDashboardDetailsRs.statusmessage = "Something went wrong!";
                            oDashboardDetailsRs.status = "Failed";
                            Console.WriteLine(ex.Message);
						}
					}
                    oDashboardDetailsRs.status = "Success";
                }
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
                oDashboardDetailsRs.statusmessage = "Something went wrong!";
                oDashboardDetailsRs.status = "Failed";
            }

			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "select itemname, remainingquantity from item where minimumstocktomaintain > remainingquantity and registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								Lowstocks oLowstocks = new Lowstocks();
								oLowstocks.item = Convert.ToString(reader["itemname"]);
								oLowstocks.qty = Convert.ToInt64(reader["remainingquantity"]);
								oDashboardDetailsRs.lowstocks.Add(oLowstocks);
							}
						}
						catch (Exception ex)
						{
                            oDashboardDetailsRs.statusmessage = "Something went wrong!";
                            oDashboardDetailsRs.status = "Failed";
                            Console.WriteLine(ex.Message);
						}
					}
                    oDashboardDetailsRs.status = "Success";
                }
            }
			catch(Exception ex)
			{
                oDashboardDetailsRs.statusmessage = "Something went wrong!";
                oDashboardDetailsRs.status = "Failed";
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
					cmd.CommandText = "select remainingquantity * purchaseprice as stockvalue from item where registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								oDashboardDetailsRs.stockvalue += Convert.ToDouble(reader["stockvalue"]);
							}
						}
						catch (Exception ex)
						{
                            oDashboardDetailsRs.statusmessage = "Something went wrong!";
                            oDashboardDetailsRs.status = "Failed";
                            Console.WriteLine(ex.Message);
						}
					}
                    oDashboardDetailsRs.status = "Success";
                }
            }
			catch (Exception ex)
			{
                oDashboardDetailsRs.statusmessage = "Something went wrong!";
                oDashboardDetailsRs.status = "Failed";
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
					cmd.CommandText = "select ide.typeofpay, ide.item, tr.total from item_details as ide left join transactions as tr on ide.transaction_id = tr.transaction_id where (ide.typeofpay = 'SALE' or ide.typeofpay = 'PURCHASE') and ide.registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								PurchaseDash oPurchaseDash = new PurchaseDash();
								oPurchaseDash.typeofpay = Convert.ToString(reader["typeofpay"]);
								oPurchaseDash.item = Convert.ToString(reader["item"]);
								oPurchaseDash.total = Convert.ToDouble(reader["total"]);
								if(oPurchaseDash.typeofpay == "PURCHASE")
								{
									oDashboardDetailsRs.totalpurchase += oPurchaseDash.total;
								}
								oDashboardDetailsRs.purchasedash.Add(oPurchaseDash);
							}
						}
						catch (Exception ex)
						{
                            oDashboardDetailsRs.statusmessage = "Something went wrong!";
                            oDashboardDetailsRs.status = "Failed";
                            Console.WriteLine(ex.Message);
						}
					}
                    oDashboardDetailsRs.status = "Success";
                }
            }
			catch (Exception ex)
			{
                oDashboardDetailsRs.statusmessage = "Something went wrong!";
                oDashboardDetailsRs.status = "Failed";
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
					cmd.CommandText = "select accountdisplayname, amount from BankForm where registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								Bankaccounts oBankaccounts = new Bankaccounts();
								oBankaccounts.bankname = Convert.ToString(reader["accountdisplayname"]);
								oBankaccounts.bankamount = Convert.ToDouble(reader["amount"]);
								oDashboardDetailsRs.bankaccounts.Add(oBankaccounts);
								oDashboardDetailsRs.bankamount += oBankaccounts.bankamount;
							}
						}
						catch (Exception ex)
						{
                            oDashboardDetailsRs.statusmessage = "Something went wrong!";
                            oDashboardDetailsRs.status = "Failed";
                            Console.WriteLine(ex.Message);
						}
					}
                    oDashboardDetailsRs.status = "Success";
                }
            }
			catch (Exception ex)
			{
                oDashboardDetailsRs.statusmessage = "Something went wrong!";
                oDashboardDetailsRs.status = "Failed";
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
					cmd.CommandText = @"
						SELECT COALESCE(SUM((elem->>'amount')::FLOAT), 0) as cashinhand
						FROM transactions
						CROSS JOIN LATERAL jsonb_array_elements(
							CASE WHEN amountdetails IS NOT NULL AND amountdetails != '' AND amountdetails != '[]'
								 THEN amountdetails::jsonb
								 ELSE '[]'::jsonb
							END
						) AS elem
						WHERE registeredphonenumber = @registeredphonenumber
						AND showtransaction = 'SHOW'
						AND elem->>'type' = 'CASH'";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows && reader.Read())
					{
						oDashboardDetailsRs.cashinhand = Convert.ToDouble(reader["cashinhand"]);
					}
					oDashboardDetailsRs.status = "Success";
				}
			}
			catch (Exception ex)
			{
				oDashboardDetailsRs.statusmessage = "Something went wrong!";
				oDashboardDetailsRs.status = "Failed";
				Console.WriteLine(ex.Message);
			}

			return oDashboardDetailsRs;
		}

		public async Task<DashboardSaleDetailsRs> DashboardSaleDetails(DashboardSaleDetailsRq oDashboardSaleDetailsRq, string daterange)
		{
			DashboardSaleDetailsRs oDashboardSaleDetailsRs = new DashboardSaleDetailsRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT invoicedate, total from transactions where registeredphonenumber = @registeredphonenumber and typeofpay = 'SALE' " + daterange;
					cmd.Parameters.AddWithValue("@registeredphonenumber", oDashboardSaleDetailsRq.registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								SaleDetails oSaleDetails = new SaleDetails
								{
									invoicedate = Convert.ToDateTime(reader["invoicedate"]),
									total = Convert.ToDecimal(reader["total"]),
								};
								oDashboardSaleDetailsRs.saledets.Add(oSaleDetails);
							}
							oDashboardSaleDetailsRs.status = "SUCCESS";
							oDashboardSaleDetailsRs.statusmessage = "Record fetched successfully";
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							oDashboardSaleDetailsRs.status = "FAILED";
						}
					}
					else
					{
						oDashboardSaleDetailsRs.status = "SUCCESS";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				oDashboardSaleDetailsRs.status = "FAILED";
			}
			return oDashboardSaleDetailsRs;
		}
	}
}
