using NHibernate.Mapping.ByCode;
using MaintenanceSchedule.Entity.Vienauto;
using NHibernate.Mapping.ByCode.Conformist;

namespace MaintenanceSchedule.Data.Vienauto
{
    public class YearMap : ClassMapping<Year>
    {
        public YearMap()
        {
            Schema("hdt");
            Table("[Year]");
            Lazy(true);
            Id(x => x.Id, map =>
            {
                map.Column("Id_Year");
                map.Generator(Generators.Identity);
            });
            Property(x => x.Name, c => c.Column("Name_Year"));
            ManyToOne(x => x.Model, map => map.Column("Id_Mode_Product"));
        }
    }
}
