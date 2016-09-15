using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using MaintenanceSchedule.Entity.Datlichbaoduong;

namespace MaintenanceSchedule.Data.DatlichbaoduongMaps
{
    public class ServiceMap : ClassMapping<Service>
    {
        public ServiceMap()
        {
            Schema("dbo");
            Table("Services");
            Lazy(true);
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Tags);
            Property(x => x.Title);
            Property(x => x.Image);
            Property(x => x.EntryDate);
            Property(x => x.UpdateDate);
            Property(x => x.Description);
            Property(x => x.Content, x => x.Type(NHibernateUtil.StringClob));
        }
    }
}
