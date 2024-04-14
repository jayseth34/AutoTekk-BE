using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Security.Cryptography.X509Certificates;
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
								" additionalfieldname1, additionalfieldname2, additionalfieldname3, additionalfieldname4, partybalance) VALUES (@typeofpay, @registeredphonenumber, @partyname, @gst, @phonenumber, @partygroup, @gsttype, @_state, @emailid, @billingaddress, " +
								"@shippingaddress, @openingbalance, @topayorreceive, @asofdate, @creditlimit, @additionalfieldname1, @additionalfieldname2, @additionalfieldname3, @additionalfieldname4, @partybalance)";
							cmd.Parameters.AddWithValue("@typeofpay", opartyRq.typeofpay);
							cmd.Parameters.AddWithValue("@registeredphonenumber", opartyRq.registeredPhoneNumber);
							cmd.Parameters.AddWithValue("@partyname", opartyRq.partyName);
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
							cmd.ExecuteNonQuery();
							opartyRs.status = "Inserted Successfully";
						}
					}
					catch(Exception ex)
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
											  "additionalfieldname2 = @additionalfieldname2, additionalfieldname3 = @additionalfieldname3, additionalfieldname4 = @additionalfieldname4, partybalance = @partybalance WHERE registeredPhoneNumber = " + opartyRq.registeredPhoneNumber + " AND partyname = '" + opartyRq.partyName + "'";
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
							cmd.ExecuteNonQuery();
							opartyRs.status = "Party Updated Successfully";
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

		public GetPartyRs GetPartyDetails(GetPartyRq ogetPartyRq)
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
						"additionalfieldname1, additionalfieldname2, additionalfieldname3, additionalfieldname4, typeofpay, topayorreceive, partybalance FROM party WHERE registeredphonenumber = " + ogetPartyRq.registeredPhoneNumber + " AND partyname = '" +
						ogetPartyRq.partyName + "'";
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

		public GetPartyListRs GetPartyList(GetPartyListRq oGetPartyListRq)
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
					cmd.CommandText = "SELECT partyname, partybalance from party where registeredphonenumber = " + oGetPartyListRq.registeredPhoneNumber ;
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
	}
}
