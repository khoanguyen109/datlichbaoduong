using System;
using NHibernate.Transform;
using MaintenanceSchedule.Data;
using yellowx.Framework.UnitWork;
using System.Collections.Generic;
using MaintenanceSchedule.Core.Web.Data;
using MaintenanceSchedule.Entity.Vienauto;
using MaintenanceSchedule.Core.Queries.Vienauto.Dto;

namespace MaintenanceSchedule.Core.Queries.Vienauto
{
    public interface IGetModelListQuery : IQuery<GetModelListQueryRequest, GetModelListQueryResponse>
    { }

    public class GetModelListQuery : IGetModelListQuery
    {
        private readonly IBaseRepository _repository;

        public GetModelListQuery(IBaseRepository repository)
        {
            _repository = repository;
        }

        public GetModelListQueryResponse Invoke(GetModelListQueryRequest request)
        {
            try
            {
                ModelDto modelDto = null;
                Model modelAlias = null;
                Style styleAlias = null;
                Manufacturer manufacturerAlias = null;

                using (var session = _repository.GetSession("nhibernate.vienauto_factory_key"))
                {
                    var modelDtos = session.QueryOver<Model>(() => modelAlias)
                                      .JoinAlias(model => model.Style, () => styleAlias)
                                      .JoinAlias(style => styleAlias.Manufacturer, () => manufacturerAlias)
                                      .Where(() => manufacturerAlias.Id == request.ManufacturerId)
                                      .SelectList(list => list
                                        .SelectGroup(() => modelAlias.Id).WithAlias(() => modelDto.ModelId)
                                        .SelectGroup(() => modelAlias.Name).WithAlias(() => modelDto.ModelName))
                                  .TransformUsing(Transformers.AliasToBean<ModelDto>())
                                  .List<ModelDto>();
                    
                    var models = new List<Model>();
                    foreach (var dto in modelDtos)
                        models.Add(dto.ToModel());

                    return new GetModelListQueryResponse()
                    {
                        Items = models,
                        ResponseStatus = GetModelStatus.Success
                    };
                }
            }
            catch (Exception ex)
            {
                return new GetModelListQueryResponse()
                {
                    Exception = ex,
                    ResponseStatus = GetModelStatus.Fail
                };
            }
        }
    }

    public class GetModelListQueryRequest
    {
        public int ManufacturerId { get; set; }
    }

    public class GetModelListQueryResponse : DataResponse<Model, GetModelStatus>
    { }

    public enum GetModelStatus
    {
        Success = 1,
        Fail
    }
}
