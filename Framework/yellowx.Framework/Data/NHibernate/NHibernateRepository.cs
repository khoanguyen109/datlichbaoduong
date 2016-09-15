using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace yellowx.Framework.Data.NHibernate
{
    public interface INHibernateTransaction
    {
        void CommitChanges();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
        bool TransactionActive { get; }
    }

    public interface INHibernateRepository : INHibernateTransaction
    {
        ISession Session { get; }

        int Count<T>(Expression<Func<T, bool>> where = null);

        void Create(object entity);

        void Update(object entity);

        void CreateOrUpdate(object entity);

        void Evict(object entity);

        void Delete(object entity);

        bool Duplicate<T>(Expression<Func<T, bool>> where);

        T Get<T>(Expression<Func<T, bool>> where);

        T Get<T>(object id);

        IList<T> List<T>(Expression<Func<T, bool>> where);

        IList<T> ListAll<T>();

        IList<T> ListPage<T>(int page, int pageSize = 10);

        IList<T> ListPage<T>(Expression<Func<T, bool>> where, int page, int pageSize = 10);

        IList<T> ListPage<T>(Expression<Func<T, bool>> where, Expression<Func<T, bool>> order, int page = 0, int pageSize = 10);

        IQueryable<T> Queryable<T>();
    }
    public abstract class NHibernateRepository : INHibernateRepository
    {
        public const int PageSize = 20;

        #region Properties

        public ISession Session
        {
            get
            {
                string factoryKey = NHibernateSessionAttribute.GetFactoryKey(this);
                var session = NHibernateSessionFactory.GetCurrentSession(factoryKey);
                return session;
            }
        }

        #endregion

        #region Public methods

        public int Count<T>(Expression<Func<T, bool>> where = null)
        {
            var count = 0;
            if (where != null)
                count = Queryable<T>().Count(where);
            else
                count = Queryable<T>().Count();
            return count;
        }

        public void Create(object entity)
        {
            Session.Save(entity);
        }

        public void Update(object entity)
        {
            Session.Update(entity);
        }

        public void CreateOrUpdate(object entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public void Evict(object entity)
        {
            Session.Evict(entity);
        }

        public void Delete(object entity)
        {
            Session.Delete(entity);
        }

        public bool Duplicate<T>(Expression<Func<T, bool>> where)
        {
            return Count(where) > 0;
        }

        public T Get<T>(Expression<Func<T, bool>> where)
        {
            return Queryable<T>().FirstOrDefault(where);
        }

        public T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        public IList<T> List<T>(Expression<Func<T, bool>> where)
        {
            if (where != null)
                return Queryable<T>().Where(where).ToList();
            else
                return Queryable<T>().ToList();
        }

        public IList<T> ListAll<T>()
        {
            ICriteria criteria = Session.CreateCriteria(typeof(T));
            return criteria.List<T>();
        }

        public IList<T> ListPage<T>(int page, int pageSize = 10)
        {
            return ListPage<T>(null, null, page, pageSize);
        }

        public IList<T> ListPage<T>(Expression<Func<T, bool>> where, int page, int pageSize = 10)
        {
            return ListPage<T>(where, null, page, pageSize);
        }

        public IList<T> ListPage<T>(Expression<Func<T, bool>> where, Expression<Func<T, bool>> order, int page = 0, int pageSize = 10)
        {
            var skipSize = pageSize * page;
            var queryable = Queryable<T>();
            if (where != null)
                queryable = queryable.Where(where);
            if (order != null)
                queryable = queryable.OrderBy(order);
            if (pageSize <= 0)
                pageSize = PageSize;
            return queryable.Skip(skipSize).Take(pageSize).ToList();
        }

        public IQueryable<T> Queryable<T>()
        {
            return Session.Query<T>();
        }

        public void Close()
        {
            Session.Close();
        }

        #endregion

        #region Transaction methods

        public void CommitChanges()
        {
            Session.Flush();
        }

        public void BeginTransaction()
        {
            Session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Session.Transaction.Commit();
        }

        public void RollbackTransaction()
        {
            try
            {
                Session.Transaction.Rollback();
            }
            catch (Exception ex)
            {
                Configuration.LogWriter.Log("Unable to rollback", ex);
            }
        }

        public bool TransactionActive
        {
            get { return Session.Transaction.IsActive; }
        }

        #endregion
    }
}
