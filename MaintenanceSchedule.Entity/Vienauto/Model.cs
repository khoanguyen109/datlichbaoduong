using System.Collections.Generic;

namespace MaintenanceSchedule.Entity.Vienauto
{
    public class Model : Entity
    {
        public virtual string Name { get; set; }
        public virtual string RewriteName { get; set; }
        public virtual Style Style { get; set; }
        public virtual IList<Year> Years { get; set; }
    }
}
