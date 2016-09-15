using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace FrameStore.Core.Mvc.Extensions
{
    public static class SelectListExtensions
    {
        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> text, Func<T, string> value, string defaultOption)
        {
            var items = enumerable.Select(f => new SelectListItem() { Text = text(f), Value = value(f) }).ToList();
            items.Insert(0, new SelectListItem() { Text = defaultOption, Value = "0" });
            return items;
        }
    }
}
