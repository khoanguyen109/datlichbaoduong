using MaintenanceSchedule.Entity.Vienauto;

namespace MaintenanceSchedule.Core.Queries.Vienauto.Dto
{
    public class YearDto
    {
        public int YearId { get; set; }
        public string YearName { get; set; }
    }

    public static class YearExtension
    {
        public static Year ToYear(this YearDto dto)
        {
            return new Year
            {
                Id = dto.YearId,
                Name = dto.YearName
            };
        }
    }
}
