using System;
using MaintenanceSchedule.Data;
using yellowx.Framework.UnitWork;
using MaintenanceSchedule.Entity.Datlichbaoduong;

namespace MaintenanceSchedule.Core.Commands.Datlichbaoduong
{
    public interface IDeleteServiceCommand : ICommand<DeleteServiceRequest, DeleteServiceResponse>
    { }

    public class DeleteServiceCommand : Command<DeleteServiceRequest, DeleteServiceResponse>, IDeleteServiceCommand
    {
        private readonly IBaseRepository _repository;

        public DeleteServiceCommand(IBaseRepository repository)
        {
            _repository = repository;
        }

        public override DeleteServiceResponse Invoke(DeleteServiceRequest request)
        {
            try
            {
                using (var session = _repository.GetSession())
                {
                    var service = _repository.Get<Service>(request.ServiceId);

                    if (service == null)
                        return new DeleteServiceResponse { Status = false };

                    _repository.Delete(service);
                    _repository.CommitChanges();

                    return new DeleteServiceResponse
                    {
                        Status = true,
                        Image = service.Image
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeleteServiceResponse
                {
                    Exception = ex,
                    Status = false
                };
            }
        }
    }

    public class DeleteServiceRequest
    {
        public int ServiceId { get; set; }
    }

    public class DeleteServiceResponse : CommandResponse<bool>
    {
        public string Image { get; set; }
    }
}

