using System.Collections.Generic;
using yellowx.Framework.UnitWork;

namespace MaintenanceSchedule.Core.Common.Paging
{
    public class PagingResponse<T> : CommandResponse
    {
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
