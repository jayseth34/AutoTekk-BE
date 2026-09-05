using Newtonsoft.Json;
using Npgsql;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.DL
{
	public class ExpenseDL
	{
		private readonly string _connectionFactory;

		public ExpenseDL(IConfiguration configuration)
		{
			this._connectionFactory = configuration.GetValue<string>("ConnectionStrings");
		}

		public ExpenseRs SaveExpense(ExpenseRq oExpenseRq)
		{
			ExpenseRs oExpenseRs = new ExpenseRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "INSERT INTO expense (registeredphonenumber, expensedate, category, partyname, description, total, paymenttype, amountdetails, showtransaction) " +
						"VALUES(@registeredphonenumber, @expensedate, @category, @partyname, @description, @total, @paymenttype, @amountdetails, 'SHOW') RETURNING expense_id";
					cmd.Parameters.AddWithValue("@registeredphonenumber", oExpenseRq.registeredphonenumber);
					cmd.Parameters.AddWithValue("@expensedate", oExpenseRq.expensedate);
					cmd.Parameters.AddWithValue("@category", oExpenseRq.category);
					cmd.Parameters.AddWithValue("@partyname", oExpenseRq.partyname ?? "");
					cmd.Parameters.AddWithValue("@description", oExpenseRq.description ?? "");
					cmd.Parameters.AddWithValue("@total", oExpenseRq.total);
					cmd.Parameters.AddWithValue("@paymenttype", oExpenseRq.paymenttype ?? "");
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(oExpenseRq.amountdetailslist));
					var result = cmd.ExecuteScalar();
					oExpenseRs.expense_id = Convert.ToInt64(result);
					oExpenseRs.status = "SUCCESS";
					oExpenseRs.statusmessage = "Expense Added Successfully";
				}
			}
			catch (Exception ex)
			{
				oExpenseRs.status = "FAILED";
				oExpenseRs.statusmessage = "Data Could Not Be Inserted";
				Console.WriteLine(ex.Message);
			}
			return oExpenseRs;
		}

		public ExpenseRs UpdateExpense(ExpenseRq oExpenseRq)
		{
			ExpenseRs oExpenseRs = new ExpenseRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = @"UPDATE expense SET expensedate = @expensedate, category = @category, partyname = @partyname,
						description = @description, total = @total, paymenttype = @paymenttype, amountdetails = @amountdetails
						WHERE expense_id = @expense_id AND registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@expensedate", oExpenseRq.expensedate);
					cmd.Parameters.AddWithValue("@category", oExpenseRq.category);
					cmd.Parameters.AddWithValue("@partyname", oExpenseRq.partyname ?? "");
					cmd.Parameters.AddWithValue("@description", oExpenseRq.description ?? "");
					cmd.Parameters.AddWithValue("@total", oExpenseRq.total);
					cmd.Parameters.AddWithValue("@paymenttype", oExpenseRq.paymenttype ?? "");
					cmd.Parameters.AddWithValue("@amountdetails", JsonConvert.SerializeObject(oExpenseRq.amountdetailslist));
					cmd.Parameters.AddWithValue("@expense_id", oExpenseRq.expense_id);
					cmd.Parameters.AddWithValue("@registeredphonenumber", oExpenseRq.registeredphonenumber);
					cmd.ExecuteNonQuery();
					oExpenseRs.expense_id = oExpenseRq.expense_id;
					oExpenseRs.status = "SUCCESS";
					oExpenseRs.statusmessage = "Expense Updated Successfully";
				}
			}
			catch (Exception ex)
			{
				oExpenseRs.status = "FAILED";
				oExpenseRs.statusmessage = "Data Could Not Be Updated";
				Console.WriteLine(ex.Message);
			}
			return oExpenseRs;
		}

		public List<AmountDetails> GetExpenseAmountDetails(Int64 expense_id, Int64 registeredphonenumber)
		{
			List<AmountDetails> amountdetailslist = new List<AmountDetails>();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT amountdetails FROM expense WHERE expense_id = @expense_id AND registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@expense_id", expense_id);
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					using NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.Read())
					{
						string amtdetails = reader["amountdetails"] == DBNull.Value ? "" : Convert.ToString(reader["amountdetails"]);
						if (!string.IsNullOrEmpty(amtdetails))
						{
							amountdetailslist = JsonConvert.DeserializeObject<List<AmountDetails>>(amtdetails) ?? new List<AmountDetails>();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return amountdetailslist;
		}

		public GetExpenseListRs GetExpenseList(Int64 registeredphonenumber)
		{
			GetExpenseListRs oGetExpenseListRs = new GetExpenseListRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT expense_id, expensedate, category, partyname, description, total, paymenttype, amountdetails FROM expense " +
						"WHERE registeredphonenumber = @registeredphonenumber AND showtransaction = 'SHOW' ORDER BY expensedate DESC, expense_id DESC";
					cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
					using NpgsqlDataReader reader = cmd.ExecuteReader();
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							ExpenseListItem oExpenseListItem = new ExpenseListItem();
							oExpenseListItem.expense_id = Convert.ToInt64(reader["expense_id"]);
							oExpenseListItem.expensedate = reader["expensedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["expensedate"]);
							oExpenseListItem.category = reader["category"] == DBNull.Value ? "" : Convert.ToString(reader["category"]);
							oExpenseListItem.partyname = reader["partyname"] == DBNull.Value ? "" : Convert.ToString(reader["partyname"]);
							oExpenseListItem.description = reader["description"] == DBNull.Value ? "" : Convert.ToString(reader["description"]);
							oExpenseListItem.total = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"]);
							oExpenseListItem.paymenttype = reader["paymenttype"] == DBNull.Value ? "" : Convert.ToString(reader["paymenttype"]);
							string amtdetails = reader["amountdetails"] == DBNull.Value ? "" : Convert.ToString(reader["amountdetails"]);
							oExpenseListItem.amountdetailslist = string.IsNullOrEmpty(amtdetails) ? new List<AmountDetails>() : (JsonConvert.DeserializeObject<List<AmountDetails>>(amtdetails) ?? new List<AmountDetails>());
							oGetExpenseListRs.expenselist.Add(oExpenseListItem);
						}
					}
					oGetExpenseListRs.status = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				oGetExpenseListRs.status = "FAILED";
				oGetExpenseListRs.statusmessage = "Something went wrong!";
				Console.WriteLine(ex.Message);
			}
			return oGetExpenseListRs;
		}

		public DeleteExpenseRs HideExpense(DeleteExpenseRq oDeleteExpenseRq)
		{
			DeleteExpenseRs oDeleteExpenseRs = new DeleteExpenseRs();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(this._connectionFactory))
				{
					conn.Open();
					NpgsqlCommand cmd = new NpgsqlCommand();
					cmd.Connection = conn;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "UPDATE expense SET showtransaction = 'HIDE' WHERE expense_id = @expense_id AND registeredphonenumber = @registeredphonenumber";
					cmd.Parameters.AddWithValue("@expense_id", oDeleteExpenseRq.expense_id);
					cmd.Parameters.AddWithValue("@registeredphonenumber", oDeleteExpenseRq.registeredphonenumber);
					cmd.ExecuteNonQuery();
					oDeleteExpenseRs.status = "SUCCESS";
					oDeleteExpenseRs.statusmessage = "Expense Deleted Successfully";
				}
			}
			catch (Exception ex)
			{
				oDeleteExpenseRs.status = "FAILED";
				oDeleteExpenseRs.statusmessage = "Data Could Not Be Deleted";
				Console.WriteLine(ex.Message);
			}
			return oDeleteExpenseRs;
		}
	}
}
