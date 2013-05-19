using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace StableTest
{
	public class DBHelper
	{
		private static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["develop"].ToString());

		public static int InsertCommand(string commandString, CommandType type, params SqlParameter[] parameters)
		{
			SqlDataAdapter da = new SqlDataAdapter(commandString, connection);
			da.SelectCommand.CommandType = type;
			foreach (SqlParameter parameter in parameters)
				da.SelectCommand.Parameters.Add(parameter);
			DataTable dt = new DataTable();
			da.Fill(dt);
			return Int32.Parse(dt.Rows[0][0].ToString());
		}

		public static bool UpdateDeleteCommand(string commandString, CommandType type, params SqlParameter[] parameters)
		{
			connection.Open();
			SqlCommand command = new SqlCommand(commandString, connection);
			command.CommandType = type;
			foreach (SqlParameter parameter in parameters)
				command.Parameters.Add(parameter);
			int result = command.ExecuteNonQuery();
			connection.Close();
			return !(result < 0);
		}

		public static DataTable SelectCommand(string commandString, CommandType type, params SqlParameter[] parameters)
		{
			SqlDataAdapter da = new SqlDataAdapter(commandString, connection);
			da.SelectCommand.CommandType = type;
			foreach (SqlParameter parameter in parameters)
				da.SelectCommand.Parameters.Add(parameter);
			DataTable dt = new DataTable();
			da.Fill(dt);
			return dt;
		}
	}
}
