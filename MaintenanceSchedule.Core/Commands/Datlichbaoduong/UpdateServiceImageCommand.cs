using System;
using MaintenanceSchedule.Data;
using yellowx.Framework.UnitWork;
using MaintenanceSchedule.Entity.Datlichbaoduong;

namespace MaintenanceSchedule.Core.Queries.Datlichbaoduong
{
    public interface IUpdateServiceImageCommand : ICommand<UpdateServiceImageRequest, UpdateServiceImageResponse>
    { }

    public class UpdateServiceImageCommand : Command<UpdateServiceImageRequest, UpdateServiceImageResponse>, IUpdateServiceImageCommand
    {
        private readonly IBaseRepository _repository;

        public UpdateServiceImageCommand(IBaseRepository repository)
        {
            _repository = repository;
        }

        public override UpdateServiceImageResponse Invoke(UpdateServiceImageRequest request)
        {
            try
            {
                using (var session = _repository.GetSession())
                {
                    var service = _repository.Get<Service>(request.ServiceId);

                    if (service == null)
                        return new UpdateServiceImageResponse { Status = false };

                    service.Image = request.ServiceImage;
                    _repository.Update(service);
                    _repository.CommitChanges();

                    return new UpdateServiceImageResponse { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new UpdateServiceImageResponse
                {
                    Exception = ex,
                    Status = false
                };
            }
        }
    }

    public class UpdateServiceImageRequest
    {
        public int ServiceId { get; set; }
        public string ServiceImage { get; set; }
    }

    public class UpdateServiceImageResponse : CommandResponse<bool>
    { }
}

