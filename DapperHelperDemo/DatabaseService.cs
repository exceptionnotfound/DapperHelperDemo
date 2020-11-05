using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DapperHelperDemo
{
    public class DatabaseService
    {
        protected string ConnectionString { get; set; }

        public DatabaseService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<T> Query<T>(string sql, object parameters)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                return conn.Query<T>(sql, parameters).ToList();
            }
            catch
            {
                return default;
            }
        }

        public T QueryFirst<T>(string sql)
        {
            return QueryFirst<T>(sql, null);
        }

        public T QueryFirst<T>(string sql, object parameters)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                return conn.QueryFirst<T>(sql, parameters);
            }
            catch(Exception ex)
            {
                //Log the exception
                throw;
            }
        }

        public T QueryFirstOrDefault<T>(string sql)
        {
            return QueryFirstOrDefault<T>(sql, null);
        }

        public T QueryFirstOrDefault<T>(string sql, object parameters)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                return conn.QueryFirstOrDefault<T>(sql, parameters);
            }
            catch
            {
                //Log the exception
                return default;
            }
        }

        public T QuerySingle<T>(string sql)
        {
            return QuerySingle<T>(sql, null);
        }

        public T QuerySingle<T>(string sql, object parameters)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                return conn.QuerySingle<T>(sql, parameters);
            }
            catch(Exception ex)
            {
                //Log the exception
                throw;
            }
        }

        public T QuerySingleOrDefault<T>(string sql)
        {
            return QuerySingleOrDefault<T>(sql, null);
        }

        public T QuerySingleOrDefault<T>(string sql, object parameters)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                return conn.QuerySingleOrDefault<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public void Execute(string sql)
        {
            Execute(sql, null);
        }

        public void Execute(string sql, object parameters)
        {
            using SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Execute(sql, parameters);
        }

        public T ExecuteScalar<T>(string sql, object parameters)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                T result = conn.ExecuteScalar<T>(sql, parameters);
                return result;
            }
            catch (Exception ex)
            {
                return default;
            }
        }


        public T ExecuteInsert<T>(DapperHelper helper)
        {
            return ExecuteScalar<T>(helper.InsertSql, helper.Parameters);
        }

        public void ExecuteUpdate(DapperHelper helper)
        {
            Execute(helper.UpdateSql, helper.Parameters);
        }
    }
}
