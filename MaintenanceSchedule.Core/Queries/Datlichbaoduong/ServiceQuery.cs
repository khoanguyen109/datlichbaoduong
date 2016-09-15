using yellowx.Framework.Data.Paging;
using MaintenanceSchedule.Entity.Datlichbaoduong;
using MaintenanceSchedule.Data.Repositories.Datlichbaoduong;
using System.Collections.Generic;

namespace MaintenanceSchedule.Core.Queries.Datlichbaoduong
{
    public interface IServiceQuery
    {
        Service GetService(int serviceId);
        Service GetNextService(int serviceId);
        Service GetLastService(int serviceId);
        IList<Service> GetLatestService();
        PagingResult<Service> GetServices(Paging paging);
    }

    public class ServiceQuery : IServiceQuery
    {
        private readonly ServiceRepository _serviceRepository;

        public ServiceQuery(ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public Service GetService(int serviceId)
        {
            return _serviceRepository.GetService(serviceId);
        }

        public Service GetNextService(int serviceId)
        {
            return _serviceRepository.GetNextService(serviceId);
        }

        public Service GetLastService(int serviceId)
        {
            return _serviceRepository.GetLastService(serviceId);
        }

        public IList<Service> GetLatestService()
        {
            return _serviceRepository.GetLatestServices();
        }

        public PagingResult<Service> GetServices(Paging paging)
        {
            return _serviceRepository.ListServices(paging);
        }
    }
}
