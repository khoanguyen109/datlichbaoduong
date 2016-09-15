using System;
using System.Collections.Generic;

namespace MaintenanceSchedule.Entity.Datlichbaoduong
{
    public class Service : Entity
    {
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Content { get; set; }
        public virtual string Tags { get; set; }
        public virtual string Image { get; set; }
        public virtual DateTime? EntryDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
    }
}

