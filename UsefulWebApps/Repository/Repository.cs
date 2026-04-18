using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace UsefulWebApps.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly MySqlConnection _connection;
        protected MySqlTransaction? _transaction;
        public Repository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(MySqlTransaction? txn)
        {
            _transaction = txn;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            string tableName = GetTableName();
            string sql = $"SELECT * FROM {tableName}";
            List<T> allDbRows = (List<T>)await _connection.QueryAsync<T>(sql);
            return allDbRows;
        }

        public async Task<IEnumerable<T>> GetAllWhere(string column, string value)
        {
            string tableName = GetTableName();
            string sql = $"SELECT * FROM {tableName} WHERE {column} = @Parameter";
            List<T> allDbRows = (List<T>)await _connection.QueryAsync<T>(sql, new { Parameter = value });
            return allDbRows;
        }

        public async Task<T> GetByUserId(string? id)
        {
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string keyProperty = GetKeyPropertyName();
            //string sql = $"SELECT * FROM {tableName} WHERE {keyColumn} = @{keyProperty}";
            string sql = $"SELECT * FROM {tableName} WHERE {keyColumn} = @id";
            T singleDbRow = await _connection.QuerySingleAsync<T>(sql, new { id });
            return singleDbRow;
        }
        public async Task<T> GetById(long? id)
        {
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string keyProperty = GetKeyPropertyName();
            //string sql = $"SELECT * FROM {tableName} WHERE {keyColumn} = @{keyProperty}";
            string sql = $"SELECT * FROM {tableName} WHERE {keyColumn} = @id";
            T singleDbRow = await _connection.QuerySingleAsync<T>(sql, new { id });
            return singleDbRow;
        }

        public async Task<T> GetRandomRow()
        {
            string tableName = GetTableName();
            string sql = $"SELECT * FROM {tableName} ORDER BY RAND() LIMIT 1;";
            T singleDbRow = await _connection.QuerySingleAsync<T>(sql);
            return singleDbRow;
        }

        public async Task<bool> Add(T entity)
        {
            int rowsEffected = 0;
            string tableName = GetTableName();
            string columns = GetColumns(excludeKey: true);
            string properties = GetPropertyNames(excludeKey: true);
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties})";

            rowsEffected = await _connection.ExecuteAsync(query, entity);
            return rowsEffected > 0 ? true : false;
        }

        public async Task<bool> Update(T entity)
        {
            int rowsEffected = 0;

            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string keyProperty = GetKeyPropertyName();

            StringBuilder query = new StringBuilder();
            query.Append($"UPDATE {tableName} SET ");

            foreach (var property in GetProperties(true))
            {
                var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

                string propertyName = property.Name;
                string columnName = columnAttr.Name;

                query.Append($"{columnName} = @{propertyName},");
            }
            //remove last , in query -- UPDATE table_name SET column1 = value1, column2 = value2, WHERE id = @id
            query.Remove(query.Length - 1, 1);

            query.Append($" WHERE {keyColumn} = @{keyProperty}");
            //https://github.com/DapperLib/Dapper/issues/540
            rowsEffected = await _connection.ExecuteAsync(query.ToString(), entity);

            return rowsEffected > 0 ? true : false;
        }

        public async Task<bool> Delete(long? id)
        {
            int rowsEffected = 0;

            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string keyProperty = GetKeyPropertyName();
            string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyProperty}";

            rowsEffected = await _connection.ExecuteAsync(query, new { id });

            return rowsEffected > 0 ? true : false;
        }

        //transaction method
        public async Task<bool> DeleteAll()
        {
            int rowsEffected = 0;
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();

            string query1 = $"DELETE FROM {tableName} WHERE {keyColumn} >= 1";
            string query2 = $"ALTER TABLE {tableName} AUTO_INCREMENT = 1";
            rowsEffected = await _connection.ExecuteAsync(query1, transaction: _transaction);
            await _connection.ExecuteAsync(query2, transaction: _transaction);
            return rowsEffected > 0 ? true : false;
        }

        public async Task<bool> DeleteAllWhere(string column, string value)
        {
            int rowsEffected = 0;
            string tableName = GetTableName();
            string sql = $"DELETE FROM {tableName} WHERE {column} = @Parameter";
            rowsEffected = await _connection.ExecuteAsync(sql, new { Parameter = value });
            return rowsEffected > 0 ? true : false;
        }
        private static string GetTableName()
        {
            string tableName = "";
            Type type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();

            tableName = tableAttr.Name;
            return tableName;
        }

        private static string GetKeyColumnName()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

                if (keyAttributes != null && keyAttributes.Length > 0)
                {
                    object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

                    if (columnAttributes != null && columnAttributes.Length > 0)
                    {
                        ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                        return columnAttribute.Name;
                    }
                    else
                    {
                        return property.Name;
                    }
                }
            }
            return null;
        }
        private static IEnumerable<PropertyInfo> GetScaffoldableProperties(bool excludeKey = false)
        {
            return typeof(T).GetProperties().Where(p =>
            {
                // Exclude key if requested
                // p.IsDefined(typeof(KeyAttribute) returns true for key column and false otherwise
                if (excludeKey && p.IsDefined(typeof(KeyAttribute)))
                    return false;

                // Exclude DB generated fields (Identity or Computed)
                DatabaseGeneratedAttribute? dbGenerated = p.GetCustomAttribute<DatabaseGeneratedAttribute>();
                if (dbGenerated != null && dbGenerated.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
                    return false;

                return true;
            });
        }
        private static string GetColumns(bool excludeKey = false)
        {
            string columns = string.Join(", ", GetScaffoldableProperties(excludeKey)
                .Select(p =>
                {
                    var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                    return columnAttr != null ? columnAttr.Name : p.Name;
                }));

            return columns;
        }
        
        private static string GetPropertyNames(bool excludeKey = false)
        {
            string values = string.Join(", ", GetScaffoldableProperties(excludeKey)
                .Select(p => $"@{p.Name}"));

            return values;
        }

        private static IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

            return properties;
        }

        private static string GetKeyPropertyName()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null);

            if (properties.Any())
            {
                return properties.FirstOrDefault().Name;
            }

            return null;
        }
    }
}
