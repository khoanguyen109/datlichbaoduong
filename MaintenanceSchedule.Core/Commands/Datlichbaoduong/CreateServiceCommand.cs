using System;
using MaintenanceSchedule.Data;
using yellowx.Framework.UnitWork;
using MaintenanceSchedule.Entity.Datlichbaoduong;

namespace MaintenanceSchedule.Core.Commands.Datlichbaoduong
{
    public interface ICreateServiceCommand : ICommand<CreateServiceRequest, CreateServiceResponse>
    { }

    public class CreateServiceCommand : Command<CreateServiceRequest, CreateServiceResponse>, ICreateServiceCommand
    {
        private readonly IBaseRepository _repository;
        
        public CreateServiceCommand(IBaseRepository repository)
        {
            _repository = repository;
        }

        public override CreateServiceResponse Invoke(CreateServiceRequest request)
        {
            try
            {
                using (var session = _repository.GetSession())
                {
                    if (_repository.Duplicate<Service>(s => s.Title == request.ServiceTitle))
                        return new CreateServiceResponse
                        {
                            Status = CreateServiceStatus.Duplicated
                        };

                    var serviceId = _repository.Create(new Service
                    {
                        EntryDate = DateTime.Now,
                        Tags = request.ServiceTags,
                        Title = request.ServiceTitle,
                        Image = request.ServiceImage,
                        Content = request.ServiceContent,
                        Description = request.ServiceDescrtiption
                    });

                    return new CreateServiceResponse
                    {
                        ServiceId = serviceId,
                        Status = CreateServiceStatus.Success
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreateServiceResponse
                {
                    Exception = ex,
                    Status = CreateServiceStatus.Fail
                };
            }
        }
    }

    public class CreateServiceRequest
    {
        public int ServiceId { get; set; }
        public string ServiceTags { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceImage { get; set; }
        public string ServiceContent { get; set; }
        public string ServiceDescrtiption { get; set; }
    }

    public class CreateServiceResponse : CommandResponse<CreateServiceStatus>
    {
        public int ServiceId { get; set; }
    }

    public enum CreateServiceStatus
    {
        Success = 1,
        Duplicated,
        Fail,
    }
}
