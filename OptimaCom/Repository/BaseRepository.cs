using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OptimaCom.Repository
{
    public abstract class BaseRepository
    {
        private  string _connectionString;
        public BaseRepository(string conn)
        {
            _connectionString = conn;
            if (!IsAvailable)
            {
                IsSqlAvailable();
            }
        }
        public bool CheckConnectionStringAndChange(string conn)
        {
            _connectionString = conn;
            if (!IsAvailable)
            {
                IsSqlAvailable();
            }
            return IsAvailable;
        }
        public abstract int GetItemById(object obj);
        public abstract object GetData(object name);

        public virtual string ExecuteSQL(string sql)
        {

            //string result = XLController.zarzadcaBazy.WykonajZapytanie(sql);
            //return result;
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery().ToString();
                        con.Close();
                        return "ok";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
                return "Błąd podłączenia do bazy";

        }
        public virtual DataTable TSqlDtResult(string sql)
        {
            //DataTable result = XLController.zarzadcaBazy.WykonajZapytanieTSQL(sql);
            //return result;
            DataTable t = new DataTable();
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        SqlCommand command = new SqlCommand(sql)
                        {
                            CommandTimeout = 10000,
                            Connection = con
                        };
                        //con.Open();
                        using (SqlDataAdapter DataAdapter = new SqlDataAdapter(command))
                        {
                            DataAdapter.Fill(t);
                        }
                        //con.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
                throw new Exception("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.");
            return t;
        }
        public virtual string SingleSqlResult(string sql)
        {
            //string s = XLController.zarzadcaBazy.WykonajZapytanieSkalar(sql);
            //return s;
            String wynik = "";
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sql, con);
                        var tmp = cmd.ExecuteScalar();
                        if (tmp != null)
                        {
                            wynik = tmp.ToString();
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return wynik;
                }
            }
            else
            {
                throw new Exception("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.");
            }
            return wynik;
        }

        public bool IsAvailable;


        public List<dynamic> GetData(string query)
        {
            List<dynamic> dynamicList = new List<dynamic>();
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    dynamic dynamicObject = new System.Dynamic.ExpandoObject();

                                    foreach (DataColumn column in dataTable.Columns)
                                    {
                                        ((IDictionary<string, object>)dynamicObject)[column.ColumnName] = row[column];
                                    }

                                    dynamicList.Add(dynamicObject);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("sql: " + ex.Message);
                    throw new Exception(ex.Message);

                }
            }
            return dynamicList;
        }

        public dynamic GetSingleDataRow(string query)
        {
            dynamic dynamicObject = null;
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {

                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                if (dataTable.Rows.Count > 0)
                                {
                                    dynamicObject = new System.Dynamic.ExpandoObject();

                                    foreach (DataColumn column in dataTable.Columns)
                                    {
                                        ((IDictionary<string, object>)dynamicObject)[column.ColumnName] = dataTable.Rows[0][column];
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return dynamicObject;
        }


        public List<T> GetData<T>(string query) where T : class, new()
        {
            List<T> resultList = new List<T>();

            if (IsAvailable)
            {
                try
                {
                    using
                        (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    T obj = new T();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                                        if (propertyInfo != null && reader[i] != DBNull.Value)
                                        {
                                            propertyInfo.SetValue(obj, reader[i]);
                                        }
                                    }
                                    resultList.Add(obj);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return resultList;
        }

        public bool IsSqlAvailable()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    connection.Close();
                    IsAvailable = true;
                    return true;
                }
            }
            catch //(Exception ex)
            {
                IsAvailable = false;
                return false;
            }
        }
    }
}
