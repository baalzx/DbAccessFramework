using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace com.tjigs.DbAccessFramework
{
    /// <summary>
    /// 本类为数据库操作的通用类，用来封装对数据库的操作
    /// 作者：张昕，原作者：周公
    /// 创建日期：2016年12月16日
    /// 修改日期：2016年12月16日，增加Access数据库访问，并测试通过
    /// </summary>
    public sealed class DbUtility
    {
        public string ConnectionString { get; set; }
        private DbProviderFactory providerFactory; 
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="connectionString">数据库连接字符串</param> 
        /// <param name="providerType">数据库类型枚举，参见<paramref name="providerType"/></param> 
        public DbUtility(string connectionString, DbProviderType providerType) 
        { 
            ConnectionString = connectionString; 
            providerFactory = ProviderFactory.GetDbProviderFactory(providerType); 
            if (providerFactory == null) 
            { 
                throw new ArgumentException("Can't load DbProviderFactory for given value of providerType"); 
            } 
        } 
        /// <summary>    
        /// 对数据库执行增删改操作，返回受影响的行数。    
        /// </summary>    
        /// <param name="sql">要执行的增删改的SQL语句</param>    
        /// <param name="parameters">执行增删改语句所需要的参数</param> 
        /// <returns></returns>   
        public int ExecuteNonQuery(string sql, IList<DbParameter> parameters) 
        { 
            return ExecuteNonQuery(sql, parameters, CommandType.Text); 
        } 
        /// <summary>    
        /// 对数据库执行增删改操作，返回受影响的行数。    
        /// </summary>    
        /// <param name="sql">要执行的增删改的SQL语句</param>    
        /// <param name="parameters">执行增删改语句所需要的参数</param> 
        /// <param name="commandType">执行的SQL语句的类型</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string sql, IList<DbParameter> parameters, CommandType commandType) 
        { 
            using (DbCommand command = CreateDbCommand(sql, parameters, commandType)) 
            { 
                command.Connection.Open(); 
                int affectedRows = command.ExecuteNonQuery(); 
                command.Connection.Close(); 
                return affectedRows; 
            } 
        } 
 
        /// <summary>    
        /// 执行一个查询语句，返回一个关联的DataReader实例    
        /// </summary>    
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param> 
        /// <returns></returns>  
        public DbDataReader ExecuteReader(string sql, IList<DbParameter> parameters) 
        { 
            return ExecuteReader(sql, parameters, CommandType.Text); 
        } 
 
        /// <summary>    
        /// 执行一个查询语句，返回一个关联的DataReader实例    
        /// </summary>    
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param> 
        /// <param name="commandType">执行的SQL语句的类型</param> 
        /// <returns></returns>  
        public DbDataReader ExecuteReader(string sql, IList<DbParameter> parameters, CommandType commandType) 
        { 
            DbCommand command = CreateDbCommand(sql, parameters, commandType); 
            command.Connection.Open(); 
            return command.ExecuteReader(CommandBehavior.CloseConnection); 
        } 
 
        /// <summary>    
        /// 执行一个查询语句，返回一个包含查询结果的DataTable    
        /// </summary>    
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param> 
        /// <returns></returns> 
        public DataTable ExecuteDataTable(string sql, IList<DbParameter> parameters) 
        { 
            return ExecuteDataTable(sql, parameters, CommandType.Text); 
        } 
        /// <summary>    
        /// 执行一个查询语句，返回一个包含查询结果的DataTable    
        /// </summary>    
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param> 
        /// <param name="commandType">执行的SQL语句的类型</param> 
        /// <returns></returns> 
        public DataTable ExecuteDataTable(string sql, IList<DbParameter> parameters, CommandType commandType) 
        { 
            using (DbCommand command = CreateDbCommand(sql, parameters, commandType)) 
            { 
                using (DbDataAdapter adapter = providerFactory.CreateDataAdapter()) 
                { 
                    adapter.SelectCommand = command; 
                    DataTable data = new DataTable(); 
                    adapter.Fill(data); 
                    return data; 
                } 
            } 
        } 
 
        /// <summary>    
        /// 执行一个查询语句，返回查询结果的第一行第一列    
        /// </summary>    
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>    
        /// <returns></returns>    
        public Object ExecuteScalar(string sql, IList<DbParameter> parameters) 
        { 
            return ExecuteScalar(sql, parameters, CommandType.Text); 
        } 
 
        /// <summary>    
        /// 执行一个查询语句，返回查询结果的第一行第一列    
        /// </summary>    
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>    
        /// <param name="commandType">执行的SQL语句的类型</param> 
        /// <returns></returns>    
        public Object ExecuteScalar(string sql, IList<DbParameter> parameters, CommandType commandType) 
        { 
            using (DbCommand command = CreateDbCommand(sql, parameters, commandType)) 
            { 
                command.Connection.Open(); 
                object result = command.ExecuteScalar(); 
                command.Connection.Close(); 
                return result; 
            } 
        }
        #region 2016年12月16日，注释掉实体查询功能，因为没有EntityReader
        /// <summary> 
        /// 查询多个实体集合 
        /// </summary> 
        /// <typeparam name="T">返回的实体集合类型</typeparam> 
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param> 
        /// <returns></returns> 
        
        //public List<T> QueryForList<T>(string sql, IList<DbParameter> parameters) where T : new() 
        //{ 
        //    return QueryForList<T>(sql, parameters, CommandType.Text); 
        //} 
 
        ///// <summary> 
        /////  查询多个实体集合 
        ///// </summary> 
        ///// <typeparam name="T">返回的实体集合类型</typeparam> 
        ///// <param name="sql">要执行的查询语句</param>    
        ///// <param name="parameters">执行SQL查询语句所需要的参数</param>    
        ///// <param name="commandType">执行的SQL语句的类型</param> 
        ///// <returns></returns> 
        //public List<T> QueryForList<T>(string sql, IList<DbParameter> parameters, CommandType commandType) where T : new() 
        //{ 
        //    DataTable data = ExecuteDataTable(sql, parameters, commandType); 
        //    return EntityReader.GetEntities<T>(data); 
        //} 
        #endregion



        public DbParameter CreateDbParameter(string name, object value) 
        { 
            return CreateDbParameter(name, ParameterDirection.Input, value); 
        } 
 
        public DbParameter CreateDbParameter(string name, ParameterDirection parameterDirection, object value) 
        { 
            DbParameter parameter = providerFactory.CreateParameter(); 
            parameter.ParameterName = name; 
            parameter.Value = value; 
            parameter.Direction = parameterDirection; 
            return parameter; 
        } 
 
        /// <summary> 
        /// 创建一个DbCommand对象 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param>    
        /// <param name="parameters">执行SQL查询语句所需要的参数</param> 
        /// <param name="commandType">执行的SQL语句的类型</param> 
        /// <returns></returns> 
        private DbCommand CreateDbCommand(string sql, IList<DbParameter> parameters, CommandType commandType) 
        { 
            DbConnection connection = providerFactory.CreateConnection(); 
            DbCommand command = providerFactory.CreateCommand(); 
            connection.ConnectionString = ConnectionString; 
            command.CommandText = sql; 
            command.CommandType = commandType; 
            command.Connection = connection; 
            if (!(parameters == null || parameters.Count == 0)) 
            { 
                foreach (DbParameter parameter in parameters) 
                { 
                    command.Parameters.Add(parameter); 
                } 
            } 
            return command; 
        } 
        /// <summary>
        /// 创建一个OleDbConnection对象
        /// </summary>
        /// <returns></returns>
        private OleDbConnection CreateOleDbConnection()
        {
            return new OleDbConnection(ConnectionString);
        }
        /// <summary>
        /// 用OleDbSchemaGuid获取数据库表名称列表
        /// </summary>
        /// <returns>数据库表名称的list</returns>
        public List<string> GetTableName()
        {
            List<string> tableNames=new List<string> ();
            OleDbConnection conn = CreateOleDbConnection();
            conn.Open();
            DataTable table = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables , new object[] { null, null, null, null });
            foreach (DataRow row in table.Rows )
            {
                tableNames.Add(row["Table_Name"].ToString());
            }
            return tableNames;
        }
        /// <summary>
        /// 获取一个表的全部字段定义及描述，返回一个datatable
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DataTable GetDbField(string tablename )
        {
            OleDbConnection conn = CreateOleDbConnection();
            conn.Open();
            DataTable table = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tablename, null });
            return table;
        }
        public DataTable GetDbPK(string tablename)
        {
            OleDbConnection conn = CreateOleDbConnection();
            conn.Open();
            DataTable table = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, null);
            return table;
        }
    }
}
