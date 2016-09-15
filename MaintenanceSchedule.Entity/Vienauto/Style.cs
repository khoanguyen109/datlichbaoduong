using System.Collections.Generic;

namespace MaintenanceSchedule.Entity.Vienauto
{
    public class Style : Entity
    {
        public virtual string Name { get; set; }
        public virtual string RewriteName { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual IList<Model> Models { get; set; }
    }
}
