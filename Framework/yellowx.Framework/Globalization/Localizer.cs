using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yellowx.Framework.Globalization
{
    public interface ILocalizer
    {
        string Get(string key, string @default = null);
    }
    public class Localizer : ILocalizer
    {
        public string Get(string key, string @default)
        {
            return @default;
        }
    }
}
