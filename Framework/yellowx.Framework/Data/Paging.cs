using System;
using System.Collections.Generic;
using yellowx.Framework.Extensions;
using yellowx.Framework.Transformers;

namespace yellowx.Framework.Data.Paging
{
    public class Paging
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public IList<PagingSort> Sorts { get; set; }
        public IList<PagingFilter> Filters { get; set; }
    }

    public class PagingResult<T>
    {
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
        public Exception Error { get; set; }

        public PagingResult()
        {
            Items = new List<T>();
        }

        public PagingResult<TDest> Transform<TDest>()
        {
            var transformer = TransformerFactory.GetTransformer<T, TDest>();
            var result = new PagingResult<TDest> { TotalCount = TotalCount };
            if (transformer != null)
                Items.ForEach(i => { var desc = transformer.Transform(i); result.Items.Add(desc); });
            return result;
        }

        public PagingResult<TDest> Transform<TDest>(Func<T, TDest> transformer)
        {
            var result = new PagingResult<TDest> { TotalCount = TotalCount };
            if (transformer != null)
                Items.ForEach(i => { var desc = transformer(i); result.Items.Add(desc); });
            return result;
        }
    }

    public class PagingFilter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public override bool Equals(object obj)
        {
            var filter = (PagingFilter)obj;
            return string.Compare(filter.Name, Name, false) == 0 && string.Compare(filter.Value, Value, false) == 0;
        }
        public override int GetHashCode()
        {
            return Name.IsNullOrEmpty() ? 0 : Name.GetHashCode();
        }
    }

    public class PagingSort
    {
        public string Name { get; set; }
        public bool Ascending { get; set; }

        public override bool Equals(object obj)
        {
            var pagingSort = (PagingSort)obj;
            return string.Compare(pagingSort.Name, Name, false) == 0 && pagingSort.Ascending == Ascending;
        }
        public override int GetHashCode()
        {
            if (Name.IsNullOrEmpty())
                return 0;
            return Name.GetHashCode();
        }
    }
}
