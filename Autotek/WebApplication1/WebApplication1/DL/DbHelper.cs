using Npgsql;
using System.Data;

namespace WebApplication1.DL
{
	/// <summary>
	/// Shared helpers for the small "does this row exist?" / "insert this lookup value
	/// if it's new" patterns that were previously copy-pasted across ItemDL and LoginDL
	/// (item/party existence checks, category/partygroup lookup tables).
	/// `table`/`column` are always literals supplied by our own DL code, never user
	/// input, so they are safe to interpolate into the query text here.
	/// </summary>
	public static class DbHelper
	{
		public static bool RecordExists(string connectionString, string table, string column, string value, long? registeredphonenumber)
		{
			using var conn = new NpgsqlConnection(connectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand(
				$"SELECT 1 FROM {table} WHERE registeredphonenumber = @registeredphonenumber AND {column} = @value LIMIT 1", conn);
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
			cmd.Parameters.AddWithValue("@value", value);
			using var reader = cmd.ExecuteReader();
			return reader.Read();
		}

		/// <summary>
		/// Ensures a lookup-table row (category, partygroup, etc.) exists for this value,
		/// inserting it if it doesn't. Mirrors the "SELECT ... ; if not found INSERT ..."
		/// pattern previously duplicated in ItemDL (category) and LoginDL (partygroup).
		/// </summary>
		public static void EnsureLookupValueExists(string connectionString, string table, string column, string value, long? registeredphonenumber)
		{
			if (string.IsNullOrEmpty(value)) return;

			bool exists = false;
			try
			{
				exists = RecordExists(connectionString, table, column, value, registeredphonenumber);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			if (exists) return;

			try
			{
				using var conn = new NpgsqlConnection(connectionString);
				conn.Open();
				using var cmd = new NpgsqlCommand(
					$"INSERT INTO {table} ({column}, registeredphonenumber) VALUES (@value, @registeredphonenumber)", conn);
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@value", value);
				cmd.Parameters.AddWithValue("@registeredphonenumber", registeredphonenumber);
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
