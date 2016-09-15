using MaintenanceSchedule.Library.NHibernate;

namespace MaintenanceSchedule.Data
{
    public interface IBaseRepository : IRepository
    { }

    public class BaseRepository : Repository, IBaseRepository
    { }
}
