using System;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace 의료IT공학과.데이터베이스
{
    public class LocalDB
    {
        OleDbCommand comm = null;
        OleDbConnection conn = null;
        OleDbDataReader reader = null;
        string connectionStr = null;

        public LocalDB(string connStr)
        {
            connectionStr = connStr;
        }

        public void Open()
        {
            if (conn != null) Close();

            comm = new OleDbCommand();
            conn = new OleDbConnection();

            conn.ConnectionString = connectionStr;
            conn.Open();
            comm.Connection = conn;
        }

        public void Close()
        {
            if (conn != null) conn.Close();
            if (reader != null) reader.Close();
            conn = null;
            comm = null;
            reader = null;
        }

        public void Query(string sql)
        {
            string trimmedSQL = sql.Trim();
            string[] words = trimmedSQL.Split(' ');
            if (words[0].ToUpper().Equals("SELECT")) ExecuteReader(trimmedSQL);
            else ExecuteNonQuery(trimmedSQL);

        }

        private void ExecuteNonQuery(string sql)
        {
            comm.CommandText = sql;
            comm.ExecuteNonQuery();
        }

        private void ExecuteReader(string sql)
        {
            if (reader != null) reader.Close();
            comm.CommandText = sql;
            reader = comm.ExecuteReader();
        }

        public bool Read()
        {
            if (reader == null) return false;

            return reader.Read();
        }

        public bool HasRows  //Property로 사용. 함수를 쓰면 줄 때 쓸 때 두개를 가지고 있음
                             //하나로 다 할 수 있음. 컨트롤이 용이
        {
            get
            {
                if (reader == null) return false;
                return reader.HasRows;
            }

        }

        public int FieldCount
        {
            get
            {
                if (reader == null) return 0;
                return reader.FieldCount;
            }
        }

        public string GetName(int index)
        {
            if (reader == null) return "";

            return reader.GetName(index);
        }

        public object GetData(string dataName)
        {
            return reader[dataName];
        }

        public object GetData(int index)
        {
            return reader[index];
        }

    }//class
}