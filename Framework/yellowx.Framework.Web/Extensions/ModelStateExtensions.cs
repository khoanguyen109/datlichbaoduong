using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using yellowx.Framework.Extensions;

namespace yellowx.Framework.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static object JsonValidation(this ModelStateDictionary state)
        {
            return from e in state
                   where e.Value.Errors.Count > 0
                   select new
                   {
                       Name = e.Key,
                       Errors = e.Value.Errors.Select(x => x.ErrorMessage).Concat(e.Value.Errors.Where(x => x.Exception != null).Select(x => x.Exception.Message))
                   };
        }

        public static string GetSummary(this ModelStateDictionary state)
        {
            return state.Values.SelectMany(x => x.Errors).Aggregate(string.Empty, (s, c) => "{0}, ".FormatWith(c.ErrorMessage)).TrimEnd(',');
        }
    }
}
