using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talage.SDK.EntityFramework.TalageIntegration.Context;

namespace Talage.SDK.EntityFramework.Repository
{
    public interface ITalageIntegrationRepository
    {
        IQueryable<T> GetAll<T>() where T : class;
        IQueryable<T> GetAllNoTracking<T>() where T : class;
        void Update();
        void Save();
        Task SaveAsync();
        void Track<TEntity>(TEntity entity) where TEntity : class;
        void UnTrack<TEntity>(TEntity entity) where TEntity : class;
        void MarkForUpdate<TEntity>(TEntity entity) where TEntity : class;
        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        TEntity Delete<TEntity>(TEntity entity) where TEntity : class;
      
       
        Dictionary<string, object> LoadRowFromSQL(string Sql, Dictionary<string, object> Params);

       
    }

    public class TalageIntegrationRepository : GenericRepository<TalageIntegrationContext>, ITalageIntegrationRepository
    {
        private readonly TalageIntegrationContext _context;

        public TalageIntegrationRepository(TalageIntegrationContext context) : base(context)
        {
            _context = context; 
        }

      
        public Dictionary<string, object> LoadRowFromSQL(string Sql, Dictionary<string, object> Params)
        {
            var result = new Dictionary<string, object>();

            var connection = _context.Database.GetDbConnection();

            var shouldCloseConnection = connection.State != ConnectionState.Open;

            try
            {
                if (shouldCloseConnection)
                    connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = Sql;
                    foreach (KeyValuePair<string, object> p in Params)
                    {
                        DbParameter dbParameter = cmd.CreateParameter();
                        dbParameter.ParameterName = p.Key;
                        dbParameter.Value = p.Value == null ? DBNull.Value : p.Value;
                        cmd.Parameters.Add(dbParameter);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            {
                                result.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                            }
                        }
                        else
                        {
                            // No data returned - throw exception
                            throw new InvalidOperationException($"No data was returned from stored procedure '{Sql}'.");
                        }
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection && connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return result;
        }

    }

}
