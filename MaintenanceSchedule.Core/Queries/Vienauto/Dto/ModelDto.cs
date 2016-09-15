using MaintenanceSchedule.Entity.Vienauto;

namespace MaintenanceSchedule.Core.Queries.Vienauto.Dto
{
    public class ModelDto
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; }
        public string RewriteModel { get; set; }
    }

    public static class ModelExtension
    {
        public static Model ToModel(this ModelDto dto)
        {
            return new Model
            {
                Id = dto.ModelId,
                Name = dto.ModelName,
                RewriteName = dto.RewriteModel
            };
        }
    }
}
