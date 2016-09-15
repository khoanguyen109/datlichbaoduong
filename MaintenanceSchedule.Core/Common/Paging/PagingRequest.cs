using System.Collections.Generic;

namespace MaintenanceSchedule.Core.Common.Paging
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class PagingRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
        public IDictionary<string, SortDirection> Sorts { get; set; }
    }
}
