using System;
using MaintenanceSchedule.Data;
using yellowx.Framework.UnitWork;
using MaintenanceSchedule.Entity.Datlichbaoduong;

namespace MaintenanceSchedule.Core.Commands.Datlichbaoduong
{
    public interface IUpdateServiceCommand : ICommand<UpdateServiceRequest, UpdateServiceResponse>
    { }

    public class UpdateServiceCommand : Command<UpdateServiceRequest, UpdateServiceResponse>, IUpdateServiceCommand
    {
        private readonly IBaseRepository _repository;

        public UpdateServiceCommand(IBaseRepository repository)
        {
            _repository = repository;
        }

        public override UpdateServiceResponse Invoke(UpdateServiceRequest request)
        {
            try
            {
                var Id = request.ServiceId;
                var Title = request.ServiceTitle;

                using (var session = _repository.GetSession())
                {
                    if (_repository.Duplicate<Service>(s => s.Id != Id && s.Title == Title))
                        return new UpdateServiceResponse
                        {
                            Status = UpdateServiceStatus.Duplicated
                        };

                    var service = _repository.Get<Service>(Id);
                    service.Title = Title;
                    service.UpdateDate = DateTime.Now;
                    service.Tags = request.ServiceTags;
                    service.Content = request.ServiceContent;
                    service.Description = request.ServiceDescription;
                    service.Image = !string.IsNullOrEmpty(request.ServiceImage) ? request.ServiceImage : service.Image;

                    _repository.Update(service);
                    _repository.CommitChanges();

                    return new UpdateServiceResponse
                    {
                        Status = UpdateServiceStatus.Success
                    };
                }
            }
            catch (Exception ex)
            {
                return new UpdateServiceResponse
                {
                    Exception = ex,
                    Status = UpdateServiceStatus.Fail
                };
            }
        }
    }

    public class UpdateServiceRequest
    {
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceContent { get; set; }
        public string ServiceTags { get; set; }
        public string ServiceImage { get; set; }
        public string ServiceDescription { get; set; }
    }

    public class UpdateServiceResponse : CommandResponse<UpdateServiceStatus>
    {

    }

    public enum UpdateServiceStatus
    {
        Success = 1,
        Duplicated,
        Fail,
    }
}
