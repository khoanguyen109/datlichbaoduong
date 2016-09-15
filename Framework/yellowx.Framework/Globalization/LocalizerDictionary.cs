using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace yellowx.Framework.Globalization
{
    public class LocalizerDictionary : StringDictionary
    {
        public string CountryCode { get; set; }
    }


    public interface ILocalizerProvider
    {
        string GetString(string countryCode, string key);
    }

    public class LocalizerProvider : ILocalizerProvider
    {
        protected readonly Dictionary<string, LocalizerDictionary> _localizers;

        public LocalizerProvider()
        {
            _localizers = new Dictionary<string, LocalizerDictionary>();
        }

        public string GetString(string countryCode, string key)
        {
            var value = "";
            if (_localizers.ContainsKey(countryCode))
                value = _localizers[countryCode][key];
            return string.IsNullOrWhiteSpace(value) ? key : value;
        }

        /// <summary>
        /// Load a xml for language into memory. Temporarily code.
        /// </summary>
        /// <param name="filePath"></param>
        public void Load(LocalizerDictionary localizerDictionary)
        {
            _localizers.Add(localizerDictionary.CountryCode, localizerDictionary);
        }
    }
}
