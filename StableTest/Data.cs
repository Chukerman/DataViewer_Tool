using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace StableTest
{
	public class Data
	{
		private int _Low;
		public int Low
		{
			get { return _Low; }
			set { _Low = value; }
		}

		private int _NodeID;
		public int NodeID
		{
			get { return _NodeID; }
			set { _NodeID = value; }
		}

		private string _RawData;
		public string RawData
		{
			get { return _RawData; }
			set { _RawData = value; }
		}

		private DateTime _LoggingOn;
		public DateTime LoggingOn
		{
			get { return _LoggingOn; }
			set { _LoggingOn = value; }
		}

		private static List<Data> toList(DataTable dt)
		{
			List<Data> result = new List<Data>();
			foreach (DataRow row in dt.Rows)
			{
				Data data = new Data();
				data.Low = Int32.Parse(row["low"].ToString());
				data.NodeID = Int32.Parse(row["nodeid"].ToString());
				data.RawData = row["rawdata"].ToString();
				data.LoggingOn = DateTime.Parse(row["loggingon"].ToString());
				result.Add(data);
			}
			return result;
		}

		public static List<Data> Get_All()
		{
			return toList(DBHelper.SelectCommand("Data_all", CommandType.StoredProcedure));
		}

		public void Save()
		{
			DBHelper.UpdateDeleteCommand("Data_Insert", CommandType.StoredProcedure,
				new SqlParameter("@low", Low),
				new SqlParameter("@nodeid", NodeID),
				new SqlParameter("@rawdata", RawData),
				new SqlParameter("@loggingon", LoggingOn));
		}

		public override string ToString()
		{
			return string.Format("[{3}] => {0} : {1} @ {2}", NodeID, Low, LoggingOn, RawData);
		}
	}
}
