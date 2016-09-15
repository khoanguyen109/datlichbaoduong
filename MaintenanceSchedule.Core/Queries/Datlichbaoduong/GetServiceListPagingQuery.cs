using MaintenanceSchedule.Core.Common.Paging;
using MaintenanceSchedule.Entity.Datlichbaoduong;
using System.Collections.Generic;
using System.Linq;
using yellowx.Framework.UnitWork;
using MaintenanceSchedule.Data.Repositories.Datlichbaoduong;

namespace MaintenanceSchedule.Core.Queries.Datlichbaoduong
{
    public interface IGetServiceListPagingQuery : IQuery<GetServiceListPagingRequest, GetServiceListPagingResponse>
    { }

    public class GetServiceListPagingQuery : IGetServiceListPagingQuery
    {
        private readonly IServiceRepository _serviceRepository;

        public GetServiceListPagingQuery(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public GetServiceListPagingResponse Invoke(GetServiceListPagingRequest input)
        {
            var servicesAndCount = _serviceRepository.ListAdminServices(input.PageIndex, input.PageSize);

            var count = (int)servicesAndCount.Keys.FirstOrDefault();
            var services = (IList<Service>)servicesAndCount.Values.FirstOrDefault();
            
            var response = new GetServiceListPagingResponse()
            {
                Items = services,
                TotalCount = count
            };
            return response;
        }
    }

    public class GetServiceListPagingRequest : PagingRequest
    {

    }

    public class GetServiceListPagingResponse : PagingResponse<Service>
    {

    }
}
