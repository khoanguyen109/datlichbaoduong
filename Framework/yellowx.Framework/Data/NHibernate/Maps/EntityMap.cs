using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace yellowx.Framework.Data.NHibernate.Maps
{

    public class EntityMap<T, TId> : ClassMap<T>
    {
        protected EntityMap(string tableName) : this("dbo", tableName)
        {

        }

        protected EntityMap(string schema, string tableName)
        {
            Schema(schema);
            Table(tableName);
        }
    }

    public class EntityMap<T> : EntityMap<T, int>
    {
        protected EntityMap(string table) : this("dbo", table)
        {

        }

        protected EntityMap(string schema, string table)
            : base(schema, table)
        {

        }
        public ManyToOnePart<TOther> References<TOther>(Expression<Func<T, TOther>> memberExpression, string referenceName, bool lazyload = true)
        {
            var part = References(memberExpression).Column(referenceName).NotFound.Ignore();
            if (!lazyload)
                part = part.Not.LazyLoad().Fetch.Join();
            return part;
        }

        public OneToManyPart<TChild> HasMany<TChild>(Expression<Func<T, IEnumerable<TChild>>> memberExpression, string keyColumn, bool lazyload = true)
        {
            var part = HasMany(memberExpression).KeyColumn(keyColumn).Inverse().Cascade.SaveUpdate().ForeignKeyConstraintName("none");
            if (!lazyload)
                part = part.Not.LazyLoad().Fetch.Join();
            return part;
        }
    }
}
