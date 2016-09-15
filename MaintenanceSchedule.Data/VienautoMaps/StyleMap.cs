using NHibernate.Mapping.ByCode;
using MaintenanceSchedule.Entity.Vienauto;
using NHibernate.Mapping.ByCode.Conformist;

namespace MaintenanceSchedule.Data.Vienauto
{
    public class StyleMap : ClassMapping<Style>
    {
        public StyleMap()
        {
            Schema("hdt");
            Table("Type_Product");
            Lazy(true);
            Id(x => x.Id, map =>
            {
                map.Column("Id_TypeProduct");
                map.Generator(Generators.Identity);
            });
            Property(x => x.Name, c => c.Column("Name_TypeProduct"));
            Property(x => x.RewriteName, c => c.Column("ReWrite_TypeProduct"));
            Bag(x => x.Models, map => map.Key(k => k.Column("Id_TypeProduct")), m => m.OneToMany());
            ManyToOne(x => x.Manufacturer, map => map.Column("Id_Manufacturer"));
        }
    }
}
