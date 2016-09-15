using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Caching;

namespace yellowx.Framework.Caching
{
    /// <summary>
    /// An engine to stored re-usable objects. 
    /// </summary>
    public interface ICacheEngine
    {
        /// <summary>
        /// Get the cache object. Use this method when the objects can be accessed by many sources.
        /// </summary>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        T Get<T>(string key, Func<T> getter, int cacheMinutes) where T : class;
        /// <summary>
        /// Get the cache object. Use this method when the objects can be accessed by many sources.
        /// If unable to get from getter, use cache file.
        /// </summary>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        /// <param name="enabledCacheFile">true if want to store the object into a file. This file will be used when getter function return nothing..</param>
        T Get<T>(string key, Func<T> getter, int cacheMinutes, bool enabledCacheFile) where T : class;
        /// <summary>
        /// Get the cache object form a specified region. Use this method when object specify clearly.
        /// </summary>
        /// <param name="region">The region to stored object. This also the folder stored the cache file.</param>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        T Get<T>(string region, string key, Func<T> getter, int cacheMinutes) where T : class;
        /// <summary>
        /// Get the cache object form a specified region. Use this method when object specify clearly.
        /// </summary>
        /// <param name="region">The region to stored object. This also the folder stored the cache file.</param>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        /// <param name="enabledCacheFile">true if want to store the object into a file. This file will be used when getter function return nothing..</param>
        T Get<T>(string region, string key, Func<T> getter, int cacheMinutes, bool enabledCacheFile) where T : class;

        void Remove(string key);
        void Remove(string region, string key);
    }

    /// <summary>
    /// Default cache engine.
    /// </summary>
    public class CacheEngine : Object, ICacheEngine
    {
        private const string GLOBAL = "global";
        private const string CACHE_FILE_EXTENSION = ".dat";

        protected readonly string cacheFolder;
        protected readonly string cacheFileExtension;
        private readonly ConcurrentDictionary<string, MemoryCache> memoryCaches;

        public CacheEngine(string cacheFolder, string cacheFileExtension)
        {
            this.cacheFolder = cacheFolder ?? AppDomain.CurrentDomain.BaseDirectory;
            this.cacheFileExtension = cacheFileExtension;
            memoryCaches = new ConcurrentDictionary<string, MemoryCache>();
        }

        public CacheEngine(string cacheFolder) : this(cacheFolder, CACHE_FILE_EXTENSION)
        {
        }

        public CacheEngine() : this(null)
        {
        }

        /// <summary>
        /// Get the cache object. Use this method when the objects can be accessed by many sources.
        /// </summary>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        public T Get<T>(string key, Func<T> getter, int cacheMinutes) where T : class
        {
            return Get(key, getter, cacheMinutes, true);
        }
        /// <summary>
        /// Get the cache object. Use this method when the objects can be accessed by many sources.
        /// If unable to get from getter, use cache file.
        /// </summary>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        /// <param name="enabledCacheFile">true if want to store the object into a file. This file will be used when getter function return nothing..</param>
        public T Get<T>(string key, Func<T> getter, int cacheMinutes, bool enabledCacheFile) where T : class
        {
            return Get(GLOBAL, key, getter, cacheMinutes, enabledCacheFile);
        }
        /// <summary>
        /// Get the cache object form a specified region. Use this method when object specify clearly.
        /// </summary>
        /// <param name="region">The region to stored object. This also the folder stored the cache file.</param>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        public T Get<T>(string region, string key, Func<T> getter, int cacheMinutes) where T : class
        {
            return Get(region, key, getter, cacheMinutes, true);
        }
        /// <summary>
        /// Get the cache object form a specified region. Use this method when object specify clearly.
        /// </summary>
        /// <param name="region">The region to stored object. This also the folder stored the cache file.</param>
        /// <param name="key">The key of object.</param>
        /// <param name="getter">The func to get object when object is not in cache</param>
        /// <param name="cacheMinutes">The minutes to stored object before expires.</param>
        /// <param name="enabledCacheFile">true if want to store the object into a file. This file will be used when getter function return nothing..</param>
        public T Get<T>(string region, string key, Func<T> getter, int cacheMinutes, bool enabledCacheFile) where T : class
        {
            if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(key))
                return default(T);

            EnsureDirectories(region, key);
            var item = GetItemFromCache<T>(region, key);
            if (item != null)
                return item;

            item = GetItemFromGetter(getter);
            if (enabledCacheFile)
            {
                if (item != null)
                    StoreItemToFile(region, key, item);
                else
                    item = GetCacheFromFile<T>(region, key);
            }
            if (item != null)
                StoreItemToCache(region, key, item, cacheMinutes);
            return item;
        }

        public void Remove(string key)
        {
            Remove(GLOBAL, key);
        }
        public void Remove(string region, string key)
        {
            var memoryCache = GetMemoryCacheByRegion(region);
            memoryCache.Remove(key);
        }

        #region Protected virtual methods
        protected virtual T GetItemFromCache<T>(string region, string key) where T : class
        {
            try
            {
                var memoryCache = GetMemoryCacheByRegion(region);
                var cacheItem = memoryCache.GetCacheItem(key);
                if (cacheItem != null)
                    return cacheItem.Value as T;
            }
            catch (Exception ex)
            {
                var details = new StringDictionary();
                details.Add("Cache name", GetType().Name);
                details.Add("Region", region);
                details.Add("Key", key);
                details.Add("Item type", typeof(T).FullName);
                Configuration.LogWriter.Log("Unable to GetItemFromCache.", ex, details);
            }
            return default(T);
        }
        protected virtual T GetItemFromGetter<T>(Func<T> getter)
        {
            if (getter == null)
                return default(T);
            try
            {
                return getter();
            }
            catch (Exception ex)
            {
                Configuration.LogWriter.Log("Unable to get item from getter", ex);
            }
            return default(T);
        }
        protected virtual T GetCacheFromFile<T>(string region, string key) where T : class
        {
            var fullFilePath = GetFullFileCache(region, key);
            var item = Configuration.Serializer.Deserialize<T>(fullFilePath);
            return item;
        }
        protected virtual void StoreItemToFile(string region, string key, object item)
        {
            var fullFilePath = GetFullFileCache(region, key);
            Configuration.Serializer.Serialize(fullFilePath, item);
        }
        protected virtual void StoreItemToCache(string region, string key, object item, int cacheMinutes)
        {
            var cacheItem = new CacheItem(key, item, region);
            var memoryCache = GetMemoryCacheByRegion(region);
            memoryCache.Add(cacheItem, new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMinutes(cacheMinutes) });
        }
        #endregion

        #region Private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private MemoryCache GetMemoryCacheByRegion(string region)
        {
            lock (this)
            {
                if (!memoryCaches.ContainsKey(region))
                    memoryCaches.GetOrAdd(region, new MemoryCache(region));
                var memoryCache = memoryCaches[region];
                return memoryCache;
            }
        }

        private void EnsureDirectories(string region, string key)
        {
            try
            {
                var path = Path.Combine(cacheFolder, region);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (System.Exception ex)
            {
                var details = new StringDictionary();
                details.Add("Method", "EnsureDirectories");
                details.Add("Cache folder", cacheFolder);
                details.Add("Region", region);
                Configuration.LogWriter.Log("Unable to EnsureDirectories", ex, details);
            }
        }
        private string GetFullFileCache(string region, string key)
        {
            var extension = cacheFileExtension.StartsWith(".") ? cacheFileExtension : "." + cacheFileExtension;
            return Path.Combine(cacheFolder, region, key + extension);
        }
        #endregion
    }
}
