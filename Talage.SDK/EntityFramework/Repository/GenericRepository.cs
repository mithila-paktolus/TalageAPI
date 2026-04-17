using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talage.SDK.EntityFramework.MISC;

namespace Talage.SDK.EntityFramework.Repository
{
    public interface IGenericRepository<T> : IDisposable where T : DbContext
    {
        IEnumerable<TEntity> Get<TEntity>() where TEntity : class;
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;
        IQueryable<TEntity> GetAllNoTracking<TEntity>() where TEntity : class;
        void Track<TEntity>(TEntity entity) where TEntity : class;
        void UnTrack<TEntity>(TEntity entity) where TEntity : class;
        void MarkForUpdate<TEntity>(TEntity entity) where TEntity : class;
        TEntity Delete<TEntity>(TEntity entity) where TEntity : class;
        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        void Save();
        Task SaveAsync();

        void BeginTransaction();
        void Commit();
        void Rollback();
        DbContext GetContext();

    }

    public class GenericRepository<T> : IGenericRepository<T>, IDisposable where T : DbContext
    {
        protected DbContext _entities;
        protected IDbContextTransaction? _transaction;

        public GenericRepository(DbContext context)
        {
            _entities = context;
        }

        public DbContext GetContext()
        {
            return _entities;
        }

        public IEnumerable<TEntity> Get<TEntity>() where TEntity : class
        {
            return GetDbSet<TEntity>();
        }

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return GetDbSet<TEntity>();
        }

        public IQueryable<TEntity> GetAllNoTracking<TEntity>() where TEntity : class
        {
            return GetDbSet<TEntity>().AsNoTracking();
        }

        protected DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            DbSet<TEntity> objectSet = _entities.Set<TEntity>();
            return objectSet;
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }

        public virtual async Task SaveAsync()
        {
            await _entities.SaveChangesAsync();
        }

        public virtual TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            GetDbSet<TEntity>().Add(entity);
            return entity;
        }

        public virtual IEnumerable<TEntity> AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            GetDbSet<TEntity>().AddRange(entities);
            return entities;
        }

        public virtual TEntity Delete<TEntity>(TEntity entity) where TEntity : class
        {
            GetDbSet<TEntity>().Remove(entity);
            return entity;
        }

        public void Track<TEntity>(TEntity entity) where TEntity : class
        {
            GetDbSet<TEntity>().Attach(entity);
        }

        public void UnTrack<TEntity>(TEntity entity) where TEntity : class
        {
            _entities.Entry(entity).State = EntityState.Detached;
        }

        public void MarkForUpdate<TEntity>(TEntity entity) where TEntity : class
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        public void BeginTransaction()
        {
            _transaction = _entities.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public void Update()
        {
            FixState(_entities);
            Save();
            foreach (var entry in _entities.ChangeTracker.Entries<IObjectState>())
            {
                entry.State = EntityState.Unchanged;
            }
        }

        private void FixState(DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries<IObjectState>())
            {
                IObjectState stateInfo = entry.Entity;
                entry.State = ConvertState(stateInfo.ObjectState);
            }
        }

        private static EntityState ConvertState(ObjectState objectState)
        {
            switch (objectState)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                case ObjectState.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Unchanged;
            }
        }
        public void Dispose()
        {
            _transaction?.Dispose();
            _entities?.Dispose();
        }
    }
}
