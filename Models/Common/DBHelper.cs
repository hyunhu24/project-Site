using System.Data.SqlClient;
using System.Data;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class DBHelper
    {
		private readonly string _connectionString;

        public DBHelper()
        {
			IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
			_connectionString = config.GetValue<string>("ConnectionStrings:Value")!;
		}

		public SqlConnection Open()
		{
			var conn = new SqlConnection(_connectionString);
			conn.Open();
			return conn;
		}

		private SqlCommand BuildQueryCommand(string cmdText, IDataParameter[]? parameters, CommandType commandType)
		{
			SqlCommand command = new(cmdText)
			{
				CommandType = commandType
			};

			if (parameters != null)
			{
				command.Parameters.AddRange(parameters);
			}

			return command;
		}

		public SqlDataReader RunCmd(SqlConnection connection, string cmdText, IDataParameter[]? parameters = null)
		{
			SqlCommand command = BuildQueryCommand(cmdText, parameters, CommandType.Text);
			command.Connection = connection;
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader(CommandBehavior.CloseConnection); // Ensure connection is closed when reader is closed
			}
			catch
			{
				connection.Close(); // If there's an error, make sure to close the connection
				throw;
			}
			return reader;
		}

		public string RunCmdScalar(string cmdText, IDataParameter[]? parameters = null)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				conn.Open();

				using SqlCommand command = BuildQueryCommand(cmdText, parameters, CommandType.Text);
				command.Connection = conn;
				object result = command.ExecuteScalar();
				return Convert.ToString(result)!;
			}
			catch
			{
				throw;
			}
		}

		public DataSet RunCmdToDataSet(string cmdText, IDataParameter[]? parameters, string tblName)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				using SqlCommand command = BuildQueryCommand(cmdText, parameters, CommandType.Text);
				command.Connection = conn;

				SqlDataAdapter sqlDA = new(command);
				DataSet ds = new();
				sqlDA.Fill(ds, tblName);
				return ds;
			}
			catch
			{
				// 예외 로깅 및 처리
				throw;
			}
		}

		public int RunIntCmd(string cmdText, IDataParameter[]? parameters = null)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				conn.Open();

				using SqlCommand command = BuildQueryCommand(cmdText, parameters, CommandType.Text);
				command.Connection = conn;
				return command.ExecuteNonQuery();
			}
			catch
			{
				// 예외 로깅 및 처리
				throw;
			}
		}

		public void RunNonQueryCmd(string cmdText, IDataParameter[]? parameters = null)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				conn.Open();

				using SqlCommand command = BuildQueryCommand(cmdText, parameters, CommandType.Text);
				command.Connection = conn;
				command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
		}
	}
}