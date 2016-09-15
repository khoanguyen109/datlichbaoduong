using System;
using MaintenanceSchedule.Data;
using System.Collections.Generic;
using yellowx.Framework.UnitWork;
using MaintenanceSchedule.Core.Web.Data;
using MaintenanceSchedule.Entity.Vienauto;


namespace MaintenanceSchedule.Core.Queries.Vienauto
{
    public interface IGetAllManufacturerListQuery : IQuery<GetAllManufacturerListQueryRequest, GetAllManufacturerListQueryResponse>
    { }

    public class GetAllManufacturerListQuery : IGetAllManufacturerListQuery
    {
        private readonly IBaseRepository _repository;

        public GetAllManufacturerListQuery(IBaseRepository repository)
        {
            _repository = repository;
        }

        public GetAllManufacturerListQueryResponse Invoke(GetAllManufacturerListQueryRequest request)
        {
            try
            {
                IList<Manufacturer> manufacturers = null;

                using (var session = _repository.GetSession("nhibernate.vienauto_factory_key"))
                {
                    manufacturers = _repository.ListAll<Manufacturer>();
                }

                return new GetAllManufacturerListQueryResponse()
                {
                    Items = manufacturers,
                    ResponseStatus = GetAllManufacturerStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new GetAllManufacturerListQueryResponse()
                {
                    Exception = ex,
                    ResponseStatus = GetAllManufacturerStatus.Fail
                };
            }
        }
    }

    public class GetAllManufacturerListQueryRequest
    {

    }

    public class GetAllManufacturerListQueryResponse : DataResponse<Manufacturer, GetAllManufacturerStatus>
    { }

    public enum GetAllManufacturerStatus
    {
        Success = 1,
        Fail
    }
}
