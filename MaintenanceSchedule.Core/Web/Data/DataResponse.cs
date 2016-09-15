using System.Collections.Generic;
using yellowx.Framework.UnitWork;

namespace MaintenanceSchedule.Core.Web.Data
{
    public class DataResponse<T> : CommandResponse
    {
        public IList<T> Items { get; set; }
    }

    public class DataResponse<T, TStatus> : DataResponse<T>
    {
        public TStatus ResponseStatus { get; set; }
    }
}
