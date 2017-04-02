using PoloneixApiBot.Entities;
using PoloneixApiBot.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoloneixApiBot.DB
{
    public class DBDirectory
    {
        public static string  ConnectionStringKey = "PoloniexConnection";
        public static int SaveOrUpdateSellBuyOrderQuote(SellBuyOrderQuote item)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"SaveOrUpdateSellBuyOrderQuote";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.StoredProcedure;
            parameterList.Add(DatabaseUtility.GetSqlParameter("Id", item.Id, SqlDbType.Int));
            parameterList.Add(DatabaseUtility.GetSqlParameter("IdOrder", item.IdOrder, SqlDbType.BigInt));
            parameterList.Add(DatabaseUtility.GetSqlParameter("TypeInternal", item.TypeInternal.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Type", item.Type.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("PricePerCoin", item.PricePerCoin, SqlDbType.Float));
            parameterList.Add(DatabaseUtility.GetSqlParameter("AmountQuote", item.AmountQuote, SqlDbType.Float));
            parameterList.Add(DatabaseUtility.GetSqlParameter("AmountBase", item.AmountBase, SqlDbType.Float));
            parameterList.Add(DatabaseUtility.GetSqlParameter("TradeTime", item.TradeTime, SqlDbType.DateTime));
            int id = DatabaseUtility.ExecuteScalar(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray()).ToInt();
            return id;
        }
        public static SellBuyOrderQuote GetLatestTransactionCurrency(string QuoteCurrency, string type)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"GetLatestTransactionCurrency";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.StoredProcedure;
            parameterList.Add(DatabaseUtility.GetSqlParameter("QuoteCurrency", QuoteCurrency, SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("type", type, SqlDbType.NVarChar));
            DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
            if (dataSet.Tables.Count > 0)
            {
                using (DataTable dt = dataSet.Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var e = GetSellBuyOrderQuoteFromDataRow(dr);
                        return e;
                    }
                }
            }
            return null;
        }
        public static SellBuyOrderQuote GetSellBuyOrderQuote(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"SELECT * FROM SellBuyOrderQuotes WHERE id=@id";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.Text;
            parameterList.Add(DatabaseUtility.GetSqlParameter("id", id, SqlDbType.Int));
            DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
            if (dataSet.Tables.Count > 0)
            {
                using (DataTable dt = dataSet.Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var e = GetSellBuyOrderQuoteFromDataRow(dr);
                        return e;
                    }
                }
            }
            return null;
        }

        public static List<SellBuyOrderQuote> GetSellBuyOrderQuotes()
        {
            var list = new List<SellBuyOrderQuote>();
            String commandText = @"SELECT * FROM SellBuyOrderQuotes ORDER BY Id DESC";
            var parameterList = new List<SqlParameter>();
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            var commandType = CommandType.Text;
            DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
            if (dataSet.Tables.Count > 0)
            {
                using (DataTable dt = dataSet.Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var e = GetSellBuyOrderQuoteFromDataRow(dr);
                        list.Add(e);
                    }
                }
            }
            return list;
        }
        public static void DeleteSellBuyOrderQuote(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"DELETE FROM SellBuyOrderQuotes WHERE Id=@Id";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.Text;
            parameterList.Add(DatabaseUtility.GetSqlParameter("Id", id, SqlDbType.Int));
            DatabaseUtility.ExecuteNonQuery(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
        }

        private static SellBuyOrderQuote GetSellBuyOrderQuoteFromDataRow(DataRow dr)
        {
            var item = new SellBuyOrderQuote();

            item.Id = dr["Id"].ToInt();
            item.IdOrder = dr["IdOrder"].ToULong();
            item.TypeInternal = dr["TypeInternal"].ToStr();
            item.Type = dr["Type"].ToStr();
            item.PricePerCoin = dr["PricePerCoin"].ToFloat();
            item.AmountQuote = dr["AmountQuote"].ToFloat();
            item.AmountBase = dr["AmountBase"].ToFloat();
            item.TradeTime = dr["TradeTime"].ToDateTime();
            return item;
        }


    }
}
