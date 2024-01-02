using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models.Base;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace MonsterTradingCards.Repositories.Base
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected string connectionString { get; set; }
        protected string primaryKeyColumnName { get; }

        public GenericRepository(string connectionString)
        {
            this.connectionString = connectionString;

            PropertyInfo? foundPrimaryKey = typeof(T).GetProperties().FirstOrDefault(property => IsPrimaryKey(property.Name));
            if (foundPrimaryKey == null)
                throw new ArgumentException("The entity does not define primary key via the DbColumnAttribute");

            primaryKeyColumnName = GetColumnName(foundPrimaryKey.Name) ?? throw new ArgumentException("The entity does not define primary key via the DbColumnAttribute");
        }

        public void Add(T t)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = t.GetType().GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    var paramNames = properties.Select((field, index) => $"@param{index}");

                    var query = $"INSERT INTO {GetEntityName()} ({fieldNames}) VALUES ({string.Join(", ", paramNames)})";

                    command.CommandText = query;

                    var index = 0;
                    foreach (var property in properties)
                    {
                        var paramName = $"@param{index}";
                        var value = property.GetValue(t);
                        var dbType = GetColumnDbType(property.Name) ?? throw new ArgumentException("DbColumnAttribute - Name is set, but the dbtype is missing");
                        if (property.PropertyType.IsEnum && value != null)
                            value = (int)value;
                        command.AddParameterWithValue(paramName, dbType, value);
                        index++;
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText = $"DELETE FROM {GetEntityName()} WHERE {primaryKeyColumnName} = @id";

                    command.AddParameterWithValue("id", DbType.Guid, id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public T Get(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(T).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $"SELECT {fieldNames} FROM {GetEntityName()} WHERE {primaryKeyColumnName} = @id";

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "id";
                    pFID.Value = id;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToObject(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<T> GetAll()
        {
            List<T> result = new List<T>();
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(T).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $"SELECT {fieldNames} FROM {GetEntityName()}";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapToObject(reader));
                        }
                    }
                }
            }
            return result;
        }

        public void Update(T t, params string[] parameters)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(T).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();

                    if (parameters.Length > 0)
                    {
                        properties = properties.Where(property => parameters.Contains(property.Name) || IsPrimaryKey(property.Name)).ToArray();
                    }

                    var setStatements = properties.Select(property => $"{GetColumnName(property.Name)} = @{GetColumnName(property.Name)}");
                    command.CommandText = $"UPDATE {GetEntityName()} SET {string.Join(", ", setStatements)} WHERE {primaryKeyColumnName} = @id";

                    foreach (var property in properties)
                    {
                        var columnName = GetColumnName(property.Name);
                        var propertyValue = typeof(T).GetProperty(property.Name)?.GetValue(t);

                        if (property.PropertyType.IsEnum && propertyValue != null)
                            propertyValue = (int)propertyValue;

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = $"@{columnName}";
                        parameter.Value = propertyValue ?? DBNull.Value;
                        parameter.DbType = GetColumnDbType(property.Name) ?? throw new ArgumentException("DbColumnAttribute - Name is set, but the dbtype is missing");
                        command.Parameters.Add(parameter);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        protected string? GetColumnName(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            var dbColumnAttribute = property?.GetCustomAttribute<DbColumnAttribute>();
            return dbColumnAttribute?.ColumnName;
        }
        protected DbType? GetColumnDbType(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            var dbColumnAttribute = property?.GetCustomAttribute<DbColumnAttribute>();
            return dbColumnAttribute?.DbType;
        }

        protected bool IsPrimaryKey(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            var dbColumnAttribute = property?.GetCustomAttribute<DbColumnAttribute>();
            return dbColumnAttribute?.IsPrimaryKey ?? false;
        }

        protected string? GetEntityName()
        {
            var dbEntityAttribute = typeof(T).GetCustomAttribute<DbEntityAttribute>();
            return dbEntityAttribute?.EntityName;
        }

        protected T MapToObject(IDataReader reader)
        {
            var instance = Activator.CreateInstance<T>(); // Erstellt eine Instanz der generischen Entität

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var propertyName = typeof(T).GetProperties().FirstOrDefault(f => GetColumnName(f.Name) == columnName).Name;

                if (!string.IsNullOrEmpty(propertyName))
                {
                    var property = typeof(T).GetProperty(propertyName);
                    Object value;
                    if (property.PropertyType == typeof(double))
                        value = reader.IsDBNull(i) ? null : (double) reader.GetDecimal(i);
                    else
                        value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    property.SetValue(instance, value);
                }
            }

            return instance;
        }
    }
}
