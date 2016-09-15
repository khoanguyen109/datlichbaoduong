using System;

namespace MaintenanceSchedule.Entity
{
    /// <summary>
    ///     ProvIdes a base class for your objects which will be persisted to the database.
    /// </summary>
    /// <remarks>
    ///     Benefits include the addition of an Id property along with a consistent manner for
    ///     comparing entities.
    ///     Since nearly all of the entities you create will have a type of int Id, this 
    ///     base class leverages this assumption. If you want an entity with a type other 
    ///     than int, such as string, then use <see cref="EntityWithTypedId{IdT}" /> instead.
    /// </remarks>
    [Serializable]
    public abstract class Entity : EntityWithTypedId<int>
    {
    }

    [Serializable]
    public abstract class EntityWithTypedId<TKey>
    {
        private int? oldHashCode;

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as EntityWithTypedId<TKey>;
            if (other == null)
                return false;
            //to handle the case of comparing two new objects
            bool otherIsTransient = Equals(other.Id, default(TKey));
            bool thisIsTransient = Equals(this.Id, default(TKey));
            if (otherIsTransient && thisIsTransient)
                return ReferenceEquals(other, this);
            return other.Id.Equals(Id);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            //This is done se we won't change the hash code
            if (oldHashCode.HasValue)
                return oldHashCode.Value;
            bool thisIsTransient = Equals(this.Id, default(TKey));
            //When we are transient, we use the base GetHashCode()
            //and remember it, so an instance can't change its hash code.
            if (thisIsTransient)
            {
                oldHashCode = base.GetHashCode();
                return oldHashCode.Value;
            }
            return Id.GetHashCode();
        }

        /// <summary>
        /// Get or set the Id of this entity
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// Equality operator so we can have == semantics
        /// </summary>
        public static bool operator ==(EntityWithTypedId<TKey> x, EntityWithTypedId<TKey> y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// Inequality operator so we can have != semantics
        /// </summary>
        public static bool operator !=(EntityWithTypedId<TKey> x, EntityWithTypedId<TKey> y)
        {
            return !(x == y);
        }
    }
}