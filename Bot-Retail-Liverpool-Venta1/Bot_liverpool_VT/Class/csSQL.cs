using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Bot_liverpool_VT.Class
{
    class csSQL
    {

        //private string strConnection1 = "User ID=Consulta; Password=s4k8rd]@; Data Source=CORPSFEVEXTSQLP.corp.televisa.com.mx,2020;";
        private string strConnection1 = System.Configuration.ConfigurationManager.ConnectionStrings["Global"].ToString();
        private SqlCommand strCommand = new SqlCommand();
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        public DataTable connect(string StoredProcedure, string[] SP_Parameters)
        {
            try
            {
                if (StoredProcedure != "" || StoredProcedure != null)
                {
                    dt.Clear();

                    strCommand.Connection = new SqlConnection(strConnection1);
                    strCommand.CommandTimeout = 0;
                    strCommand.Connection.Open();
                    strCommand.CommandType = CommandType.StoredProcedure;
                    strCommand.CommandText = StoredProcedure;
                    strCommand.Parameters.Clear();

                    for (int i = 0; i <= SP_Parameters.Count() - 1; i++)
                    {
                        String[] substrings = SP_Parameters[i].Split(':');
                        strCommand.Parameters.AddWithValue(substrings[0], substrings[1]);
                    }

                    dataAdapter.SelectCommand = strCommand;
                    dataAdapter.Fill(dt);

                    strCommand.Parameters.Clear();
                    strCommand.Connection.Close();

                    return dt;

                }

                return dt;

            }
            catch (Exception)
            {
                return dt;
            }
        }


        public Boolean BulkCopy(DataTable DTable, string SQLTable = "")
        {
            Boolean retorno = false;
            using (SqlConnection connection = new SqlConnection(strConnection1))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn c in DTable.Columns)
                        bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                    bulkCopy.DestinationTableName = SQLTable;
                    try
                    {
                        bulkCopy.WriteToServer(DTable);
                        retorno = true;
                    }
                    catch (Exception)
                    {
                        retorno = false;
                        //Console.WriteLine(ex.Message);
                    }
                }
            }
            return retorno;
        }

    }
}
