using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
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
					cmd.CommandText = "SELECT status, expirydate, plantype FROM registeragent WHERE phonenumber = " + ologinRq.phonenumber + " AND _password = '" + ologinRq.password + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						ologinrs.status = "SUCCESS";
						ologinrs.statusMessage = reader["status"].ToString();
						ologinrs.expiryDate = Convert.ToDateTime(reader["expirydate"]);
						ologinrs.plantype =  reader["plantype"] == DBNull.Value ? null : Convert.ToString(reader["plantype"]);
					}
				}
			}
			catch(Exception ex)
			{
				ologinrs.statusMessage = "Record Not Found";
			}
			return ologinrs;
		}

		public bool GetOtpLoginDetails(string phonenumber)
		{
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT status, expirydate FROM registeragent WHERE phonenumber = " + Convert.ToInt64(phonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				return false;
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
					cmd.CommandText = "SELECT status, expirydate, plantype FROM registeragent WHERE phonenumber = " + Convert.ToInt64(request.registeredphonenumber);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						otpRs.status = "SUCCESS";
						otpRs.statusmessage = reader["status"].ToString();
						otpRs.expiryDate = Convert.ToDateTime(reader["expirydate"]);
						otpRs.plantype = reader["plantype"] == DBNull.Value ? null : Convert.ToString(reader["plantype"]);
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
					cmd.CommandText = "SELECT phonenumber FROM registeragent WHERE phonenumber = " + oregisterRq.phonenumber ;
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
						cmd.CommandText = "SELECT partyname FROM party WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND partyname = '" + opartyRq.partyname + "'";
						NpgsqlDataReader reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							opartyRs.status = "Party Name Already Exists";
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
							opartyRs.status = "Inserted Successfully";
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					
				}
				/*if (partyexist)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "UPDATE party SET typeofpay = @typeofpay, gst = @gst, phonenumber = @phonenumber, partygroup = @partygroup, gsttype = @gsttype, _state = @_state, emailid = @emailid, billingaddress = @billingaddress, " +
											  "shippingaddress = @shippingaddress, openingbalance = @openingbalance, topayorreceive = @topayorreceive, asofdate = @asofdate, creditlimit = @creditlimit, additionalfieldname1 = @additionalfieldname1," +
											  "additionalfieldname2 = @additionalfieldname2, additionalfieldname3 = @additionalfieldname3, additionalfieldname4 = @additionalfieldname4, partybalance = @partybalance, additionalfieldname1value = @additionalfieldname1value, additionalfieldname2value = @additionalfieldname2value, additionalfieldname3value = @additionalfieldname3value, additionalfieldname4value = @additionalfieldname4value WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND partyname = '" + opartyRq.partyname + "'";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
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
							cmd.Parameters.AddWithValue("@partybalance", opartyRq.partybalance);
							cmd.Parameters.AddWithValue("@additionalfieldname1value", opartyRq.additionalfieldname1value);
							cmd.Parameters.AddWithValue("@additionalfieldname2value", opartyRq.additionalfieldname2value);
							cmd.Parameters.AddWithValue("@additionalfieldname3value", opartyRq.additionalfieldname3value);
							cmd.Parameters.AddWithValue("@additionalfieldname4value", opartyRq.additionalfieldname4value);
							cmd.ExecuteNonQuery();
							opartyRs.status = "Party Updated Successfully";
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				} */
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
							cmd.CommandText = "INSERT INTO transactions(typeofpay, invoicedate, total, balance, customername, phonenumber, registeredphonenumber," +
								"paymentstatus) VALUES(@typeofpay, @invoicedate, @total, @balance, @customername, @phonenumber, @registeredphonenumber," +
								"@paymentstatus) RETURNING transaction_id";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@invoicedate", opartyRq.asofdate);
							cmd.Parameters.AddWithValue("@total", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@balance", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phonenumber);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@paymentstatus", "UNPAID");
							Int64 transactionId = (Int64)cmd.ExecuteScalar();
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
								cmd.CommandText = "SELECT partygroup FROM partygroup WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND partygroup = '" + opartyRq.partygroup + "'";
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
							cmd.CommandText = "SELECT partyname FROM party WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND partyname = '" + opartyRq.partyname + "'";
							NpgsqlDataReader reader = cmd.ExecuteReader();
							while (reader.Read())
							{
								opartyRs.status = "Party Name Already Exists";
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
											  "additionalfieldname4value = @additionalfieldname4value, topayparty = @topayparty, toreceivefromparty = @toreceivefromparty WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND partyname = '" + opartyRq.oldpartyname + "'";
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
							cmd.Parameters.AddWithValue("@topayparty", opartyRq.topayparty);
							cmd.Parameters.AddWithValue("@toreceivefromparty", opartyRq.toreceivefromparty);
							cmd.ExecuteNonQuery();
							opartyRs.status = "Party Updated Successfully";
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


							cmd.CommandText = "UPDATE transactions SET typeofpay = @typeofpay, invoicedate = @invoicedate, total = @total, balance = @balance, customername = @customername, phonenumber = @phonenumber" +
											  " WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " customername = '" + opartyRq.oldpartyname + "'";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@invoicedate", opartyRq.asofdate);
							cmd.Parameters.AddWithValue("@total", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@balance", opartyRq.openingbalance);
							cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phonenumber);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredphonenumber);
							cmd.Parameters.AddWithValue("@paymentstatus", "UNPAID");
							cmd.ExecuteNonQuery();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				else
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = "DELETE FROM transactions WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND customername = '" + opartyRq.oldpartyname + "'" +
								"AND (typeofpay = 'RECEIVABLE OPENING BALANCE' OR typeofpay = 'PAYABLE OPENING BALANCE')";
							cmd.ExecuteNonQuery();
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
								cmd.CommandText = "SELECT partygroup FROM partygroup WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND partygroup = '" + opartyRq.partygroup + "'";
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
						cmd.CommandText = "UPDATE transactions set customername = @customername WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND customername = '" + opartyRq.oldpartyname + "'";
						cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
						cmd.ExecuteNonQuery();
					}

					using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
					{
						conn.Open();
						NpgsqlCommand cmd = new NpgsqlCommand();
						cmd.Connection = conn;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "UPDATE item_details set customername = @customername WHERE registeredphonenumber = " + opartyRq.registeredphonenumber + " AND customername = '" + opartyRq.oldpartyname + "'";
						cmd.Parameters.AddWithValue("@customername", opartyRq.partyname);
						cmd.ExecuteNonQuery();
					}
				}
				catch(Exception ex)
				{
					opartyRs.status = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return opartyRs;
		}

		public GetPartyRs GetPartyDetails(Int64 registeredphonenumber, string partyname)
		{
			GetPartyRs ogetPartyRs = new GetPartyRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT gst, phonenumber, partygroup, gsttype, _state, emailid, billingaddress, shippingaddress, openingbalance, asofdate, creditlimit," +
						"additionalfieldname1, additionalfieldname2, additionalfieldname3, additionalfieldname4, typeofpay, topayorreceive, topayparty, toreceivefromparty, additionalfieldname1value, additionalfieldname2value, additionalfieldname3value, additionalfieldname4value FROM party WHERE registeredphonenumber = " + registeredphonenumber + " AND partyname = '" +
						partyname + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if(reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetAllPartyList ogetallpartylist = new GetAllPartyList();
								ogetallpartylist.GST = reader["gst"] == DBNull.Value ? null : Convert.ToString(reader["gst"]);
								ogetallpartylist.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
								ogetallpartylist.partygroup = reader["partygroup"] == DBNull.Value ? null : Convert.ToString(reader["partygroup"]);
								ogetallpartylist.gsttype = reader["gsttype"] == DBNull.Value ? null : Convert.ToString(reader["gsttype"]);
								ogetallpartylist._state = reader["_state"] == DBNull.Value ? null : Convert.ToString(reader["_state"]);
								ogetallpartylist.emailid = reader["emailid"] == DBNull.Value ? null : Convert.ToString(reader["emailid"]);
								ogetallpartylist.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
								ogetallpartylist.shippingaddress = reader["shippingaddress"] == DBNull.Value ? null : Convert.ToString(reader["shippingaddress"]);
								ogetallpartylist.openingbalance = reader["openingbalance"] == DBNull.Value ? 0 : Convert.ToInt64(reader["openingbalance"]);
								ogetallpartylist.asofdate = reader["asofdate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["asofdate"]);
								ogetallpartylist.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["creditlimit"]);
								ogetallpartylist.additionalfieldname1 = reader["additionalfieldname1"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname1"]);
								ogetallpartylist.additionalfieldname2 = reader["additionalfieldname2"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname2"]);
								ogetallpartylist.additionalfieldname3 = reader["additionalfieldname3"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname3"]);
								ogetallpartylist.additionalfieldname4 = reader["additionalfieldname4"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname4"]);
								ogetallpartylist.typeofpay = reader["typeofpay"] == DBNull.Value ? null : Convert.ToString(reader["typeofpay"]);
								ogetallpartylist.topayorreceive = reader["topayorreceive"] == DBNull.Value ? null : Convert.ToString(reader["topayorreceive"]);
								ogetallpartylist.topayparty = reader["topayparty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["topayparty"]);
								ogetallpartylist.toreceivefromparty = reader["toreceivefromparty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["toreceivefromparty"]);
								ogetallpartylist.additionalfieldname1value = reader["additionalfieldname1value"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname1value"]);
								ogetallpartylist.additionalfieldname2value = reader["additionalfieldname2value"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname2value"]);
								ogetallpartylist.additionalfieldname3value = reader["additionalfieldname3value"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname3value"]);
								ogetallpartylist.additionalfieldname4value = reader["additionalfieldname4value"] == DBNull.Value ? null : Convert.ToString(reader["additionalfieldname4value"]);

								ogetPartyRs.partyList.Add(ogetallpartylist);
								ogetPartyRs.status = "SUCCESS";
							}
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
					else
					{
						ogetPartyRs.status = "FAILED";
						ogetPartyRs.statusMessage = "No Records Found";
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return ogetPartyRs;
		}

		public GetPartyListRs GetPartyList(Int64 registeredphonenumber)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			Int64 topayparty = 0;
			Int64 toreceivefromparty = 0;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT partyname, topayparty, toreceivefromparty, shippingaddress, billingaddress, phonenumber, creditlimit from party where registeredphonenumber = " + registeredphonenumber ;
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetPartyList oGetPartyList = new GetPartyList();
								oGetPartyList.partyname = reader["partyname"] == DBNull.Value ? null : Convert.ToString(reader["partyname"]);
								oGetPartyList.topayparty = reader["topayparty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["topayparty"]);
								oGetPartyList.toreceivefromparty = reader["toreceivefromparty"] == DBNull.Value ? 0 : Convert.ToInt64(reader["toreceivefromparty"]);
								oGetPartyList.creditlimit = reader["creditlimit"] == DBNull.Value ? 0 : Convert.ToInt64(reader["creditlimit"]);
								oGetPartyList.shippingaddress = reader["shippingaddress"] == DBNull.Value ? null : Convert.ToString(reader["shippingaddress"]);
								oGetPartyList.billingaddress = reader["billingaddress"] == DBNull.Value ? null : Convert.ToString(reader["billingaddress"]);
								oGetPartyList.phonenumber = reader["phonenumber"] == DBNull.Value ? 0 : Convert.ToInt64(reader["phonenumber"]);
								oGetPartyListRs.getPartyList.Add(oGetPartyList);

							}
							oGetPartyListRs.status = "SUCCESS";
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
					cmd.CommandText = "SELECT partygroup, COUNT(*) AS partygroup_count FROM party WHERE registeredphonenumber = " + registeredphonenumber + " GROUP BY partygroup";
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
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oGetPartyGroupRs;
		}

		public GetPartyByGroupRs GetPartyByGroup(Int64 registeredphonenumber, string groupname)
		{
			GetPartyByGroupRs oGetPartyByGroupRs = new GetPartyByGroupRs();
			Int64 topayparty = 0;
			Int64 toreceivefromparty = 0;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT partyname, topayparty, toreceivefromparty from party where registeredphonenumber = " + registeredphonenumber + " AND partygroup = '" + groupname + "'";
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
				oAddUpdatePartyGropRs.status = outputResult;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return oAddUpdatePartyGropRs;
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
					cmd.CommandText = "UPDATE registeragent set expirydate = @expirydate, plantype = @plantype WHERE phonenumber = " + registeredphonenumber ;
					cmd.Parameters.AddWithValue("@expirydate", date);
					cmd.Parameters.AddWithValue("@plantype", planType);
					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}
	}
}
