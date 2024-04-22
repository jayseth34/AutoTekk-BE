using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using WebApplication1.Models;

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
					cmd.CommandText = "SELECT status, expirydate FROM registeragent WHERE phoneNumber = "+ ologinRq.phoneNumber + " AND _password = '" + ologinRq.password + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						ologinrs.status = "SUCCESS";
						ologinrs.statusMessage = reader["status"].ToString();
						ologinrs.expiryDate = Convert.ToDateTime(reader["expirydate"]);
					}
				}
			}
			catch(Exception ex)
			{
				ologinrs.statusMessage = "Record Not Found";
			}
			return ologinrs;
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
					cmd.CommandText = "SELECT phonenumber FROM registeragent WHERE phoneNumber = " + oregisterRq.phoneNumber ;
					NpgsqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						oregisterrs.status = "Phone Number already exists.";
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
							cmd.CommandText = "INSERT INTO registeragent (phoneNumber, _password, _state, address) VALUES (@phonenumber, @_password, @_state, @address)";
							cmd.Parameters.AddWithValue("@phonenumber", oregisterRq.phoneNumber);
							cmd.Parameters.AddWithValue("@_password", oregisterRq.password);
							cmd.Parameters.AddWithValue("@_state", oregisterRq.state);
							cmd.Parameters.AddWithValue("@address", oregisterRq.address);
							cmd.ExecuteNonQuery();
							oregisterrs.status = "Inserted Successfully";
						}
					}
					catch (Exception ex)
					{
						oregisterrs.status = "Data Could Not Be Inserted";
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
						cmd.CommandText = "SELECT partyname FROM party WHERE registeredphonenumber = " + opartyRq.registeredPhoneNumber + " AND partyname = '" + opartyRq.partyName + "'";
						NpgsqlDataReader reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							opartyRs.status = "Record Already Exists";
							partyexist = true;
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
								" additionalfieldname1, additionalfieldname2, additionalfieldname3, additionalfieldname4, additionalfieldname1value, additionalfieldname2value, additionalfieldname3value, additionalfieldname4value, partybalance) VALUES (@typeofpay, @registeredphonenumber, @partyname, @gst, @phonenumber, @partygroup, @gsttype, @_state, @emailid, @billingaddress, " +
								"@shippingaddress, @openingbalance, @topayorreceive, @asofdate, @creditlimit, @additionalfieldname1, @additionalfieldname2, @additionalfieldname3, @additionalfieldname4, @additionalfieldname1value, @additionalfieldname2value, @additionalfieldname3value, @additionalfieldname4value, @partybalance)";

							cmd.Parameters.AddWithValue("typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("registeredphonenumber", opartyRq.registeredPhoneNumber);
							cmd.Parameters.AddWithValue("partyname", opartyRq.partyName);
							cmd.Parameters.AddWithValue("gst", opartyRq.GST);
							cmd.Parameters.AddWithValue("phonenumber", opartyRq.phoneNumber);
							cmd.Parameters.AddWithValue("partygroup", opartyRq.partyGroup);
							cmd.Parameters.AddWithValue("gsttype", opartyRq.gstType);
							cmd.Parameters.AddWithValue("_state", opartyRq._state);
							cmd.Parameters.AddWithValue("emailid", opartyRq.emailId);
							cmd.Parameters.AddWithValue("billingaddress", opartyRq.billingAddress);
							cmd.Parameters.AddWithValue("shippingaddress", opartyRq.shippingAddress);
							cmd.Parameters.AddWithValue("openingbalance", opartyRq.openingBalance);
							cmd.Parameters.AddWithValue("topayorreceive", opartyRq.toPayOrReceive);
							cmd.Parameters.AddWithValue("asofdate", opartyRq.asOfDate);
							cmd.Parameters.AddWithValue("creditlimit", opartyRq.creditLimit);
							cmd.Parameters.AddWithValue("additionalfieldname1", opartyRq.additionalFieldName1);
							cmd.Parameters.AddWithValue("additionalfieldname2", opartyRq.additionalFieldName2);
							cmd.Parameters.AddWithValue("additionalfieldname3", opartyRq.additionalFieldName3);
							cmd.Parameters.AddWithValue("additionalfieldname4", opartyRq.additionalFieldName4);
							cmd.Parameters.AddWithValue("additionalfieldname1value", opartyRq.additionalFieldName1value);
							cmd.Parameters.AddWithValue("additionalfieldname2value", opartyRq.additionalFieldName2value);
							cmd.Parameters.AddWithValue("additionalfieldname3value", opartyRq.additionalFieldName3value);
							cmd.Parameters.AddWithValue("additionalfieldname4value", opartyRq.additionalFieldName4value);
							cmd.Parameters.AddWithValue("partybalance", opartyRq.partybalance);

							cmd.ExecuteNonQuery();
							opartyRs.status = "Inserted Successfully";
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					
				}
				if (partyexist)
				{
					try
					{
						using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
						{
							conn.Open();
							NpgsqlCommand cmd = new NpgsqlCommand();
							cmd.Connection = conn;
							cmd.CommandType = CommandType.Text;
							cmd.CommandText = " UPDATE party SET typeofpay = @typeofpay, gst = @gst, phonenumber = @phonenumber, partygroup = @partygroup, gsttype = @gsttype, _state = @_state, emailid = @emailid, billingaddress = @billingaddress, " +
											  "shippingaddress = @shippingaddress, openingbalance = @openingbalance, topayorreceive = @topayorreceive, asofdate = @asofdate, creditlimit = @creditlimit, additionalfieldname1 = @additionalfieldname1," +
											  "additionalfieldname2 = @additionalfieldname2, additionalfieldname3 = @additionalfieldname3, additionalfieldname4 = @additionalfieldname4, partybalance = @partybalance, additionalfieldname1value = @additionalfieldname1value, additionalfieldname2value = @additionalfieldname2value, additionalfieldname3value = @additionalfieldname3value, additionalfieldname4value = @additionalfieldname4value WHERE registeredPhoneNumber = " + opartyRq.registeredPhoneNumber + " AND partyname = '" + opartyRq.partyName + "'";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@gst", opartyRq.GST);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phoneNumber);
							cmd.Parameters.AddWithValue("@partygroup", opartyRq.partyGroup);
							cmd.Parameters.AddWithValue("@gsttype", opartyRq.gstType);
							cmd.Parameters.AddWithValue("@_state", opartyRq._state);
							cmd.Parameters.AddWithValue("@emailid", opartyRq.emailId);
							cmd.Parameters.AddWithValue("@billingaddress", opartyRq.billingAddress);
							cmd.Parameters.AddWithValue("@shippingaddress", opartyRq.shippingAddress);
							cmd.Parameters.AddWithValue("@openingbalance", opartyRq.openingBalance);
							cmd.Parameters.AddWithValue("@topayorreceive", opartyRq.toPayOrReceive);
							cmd.Parameters.AddWithValue("@asofdate", opartyRq.asOfDate);
							cmd.Parameters.AddWithValue("@creditlimit", opartyRq.creditLimit);
							cmd.Parameters.AddWithValue("@additionalfieldname1", opartyRq.additionalFieldName1);
							cmd.Parameters.AddWithValue("@additionalfieldname2", opartyRq.additionalFieldName2);
							cmd.Parameters.AddWithValue("@additionalfieldname3", opartyRq.additionalFieldName3);
							cmd.Parameters.AddWithValue("@additionalfieldname4", opartyRq.additionalFieldName4);
							cmd.Parameters.AddWithValue("@partybalance", opartyRq.partybalance);
							cmd.Parameters.AddWithValue("@additionalfieldname1value", opartyRq.additionalFieldName1value);
							cmd.Parameters.AddWithValue("@additionalfieldname2value", opartyRq.additionalFieldName2value);
							cmd.Parameters.AddWithValue("@additionalfieldname3value", opartyRq.additionalFieldName3value);
							cmd.Parameters.AddWithValue("@additionalfieldname4value", opartyRq.additionalFieldName4value);
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
							cmd.CommandText = "INSERT INTO transactions(typeofpay, invoicedate, total, balance, customername, phonenumber, registeredphonenumber," +
								"paymentstatus) VALUES(@typeofpay, @invoicedate, @total, @balance, @customername, @phonenumber, @registeredphonenumber," +
								"@paymentstatus) RETURNING transaction_id";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@invoicedate", opartyRq.asOfDate);
							cmd.Parameters.AddWithValue("@total", opartyRq.openingBalance);
							cmd.Parameters.AddWithValue("@balance", opartyRq.openingBalance);
							cmd.Parameters.AddWithValue("@customername", opartyRq.partyName);
							cmd.Parameters.AddWithValue("@phonenumber", opartyRq.phoneNumber);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredPhoneNumber);
							cmd.Parameters.AddWithValue("@paymentstatus", "UNPAID");
							int transactionId = (int)cmd.ExecuteScalar();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (!string.IsNullOrEmpty(opartyRq.partyGroup))
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
								cmd.CommandText = "SELECT partygroup FROM partygroup WHERE registeredphonenumber = " + opartyRq.registeredPhoneNumber + " AND partygroup = '" + opartyRq.partyGroup + "'";
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
									cmd.Parameters.AddWithValue("@partygroup", opartyRq.partyGroup);
									cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredPhoneNumber);
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

		public GetPartyRs GetPartyDetails(Int64 registeredPhoneNumber, string partyName)
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
						"additionalfieldname1, additionalfieldname2, additionalfieldname3, additionalfieldname4, typeofpay, topayorreceive, partybalance, additionalfieldname1value, additionalfieldname2value, additionalfieldname3value, additionalfieldname4value FROM party WHERE registeredphonenumber = " + registeredPhoneNumber + " AND partyname = '" +
						partyName + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if(reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetAllPartyList ogetallpartylist = new GetAllPartyList();
								ogetallpartylist.GST = Convert.ToString(reader["gst"]);
								ogetallpartylist.phoneNumber = Convert.ToInt64(reader["phonenumber"]);
								ogetallpartylist.partyGroup = Convert.ToString(reader["partygroup"]);
								ogetallpartylist.gstType = Convert.ToString(reader["gsttype"]);
								ogetallpartylist._state = Convert.ToString(reader["_state"]);
								ogetallpartylist.emailId = Convert.ToString(reader["emailId"]);
								ogetallpartylist.billingAddress = Convert.ToString(reader["billingAddress"]);
								ogetallpartylist.shippingAddress = Convert.ToString(reader["shippingAddress"]);
								ogetallpartylist.openingBalance = Convert.ToInt64(reader["openingbalance"]);
								ogetallpartylist.asOfDate = reader["asofdate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["asofdate"]);
								ogetallpartylist.creditLimit = Convert.ToInt64(reader["creditlimit"]);
								ogetallpartylist.additionalFieldName1 = Convert.ToString(reader["additionalfieldname1"]);
								ogetallpartylist.additionalFieldName2 = Convert.ToString(reader["additionalfieldname2"]);
								ogetallpartylist.additionalFieldName3 = Convert.ToString(reader["additionalfieldname3"]);
								ogetallpartylist.additionalFieldName4 = Convert.ToString(reader["additionalfieldname4"]);
								ogetallpartylist.typeofpay = Convert.ToString(reader["typeofpay"]);
								ogetallpartylist.toPayOrReceive = Convert.ToString(reader["topayorreceive"]);
								ogetallpartylist.partybalance = Convert.ToInt64(reader["partybalance"]);
								ogetallpartylist.additionalFieldName1Value = Convert.ToString(reader["additionalfieldname4value"]);
								ogetallpartylist.additionalFieldName2Value = Convert.ToString(reader["additionalfieldname4value"]);
								ogetallpartylist.additionalFieldName3Value = Convert.ToString(reader["additionalfieldname4value"]);
								ogetallpartylist.additionalFieldName4Value = Convert.ToString(reader["additionalfieldname4value"]);
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

		public GetPartyListRs GetPartyList(Int64 registeredPhoneNumber)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT partyname, partybalance from party where registeredphonenumber = " + registeredPhoneNumber ;
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetPartyList oGetPartyList = new GetPartyList();
								oGetPartyList.partyname = Convert.ToString(reader["partyname"]);
								oGetPartyList.partybalance = Convert.ToInt64(reader["partybalance"]);
								oGetPartyListRs.getPartyList.Add(oGetPartyList);
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
			return oGetPartyListRs;
		}
		public GetPartyGroupRs GetPartyGroup(Int64 registeredPhoneNumber)
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
					cmd.CommandText = "SELECT partygroup, COUNT(*) AS partygroup_count FROM party WHERE registeredphonenumber = " + registeredPhoneNumber + " GROUP BY partygroup";
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
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT partyname, partybalance from party where registeredphonenumber = " + registeredphonenumber + " AND partygroup = '" + groupname + "'";
					NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						try
						{
							while (reader.Read())
							{
								GetPartyList oGetPartyList = new GetPartyList();
								oGetPartyList.partyname = Convert.ToString(reader["partyname"]);
								oGetPartyList.partybalance = Convert.ToInt64(reader["partybalance"]);
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
	}
}
