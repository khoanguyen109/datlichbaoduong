using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yellowx.Framework
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
        DateTime EntryDate { get; set; }
        DateTime UpdateDate { get; set; }
    }
    public class Entity<TId> : Object, IEntity<TId>
    {
        public virtual TId Id
        {
            get;

            set;
        }
        public virtual DateTime EntryDate
        {
            get;

            set;
        }
        public virtual DateTime UpdateDate
        {
            get;

            set;
        }
    }

    public class Entity : Entity<int>
    {

    }
}
