using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite.Caching
{
    /// <summary>
    /// A wrapper around MemoryCache to support clearing of all cache entries as
    /// well as sub-sets e.g. by cache namespace.
    /// </summary>
    /// <remarks>
    /// Hopefully this will be addressed in the framework at some point, see 
    /// https://github.com/aspnet/Caching/issues/96
    /// </remarks>
    public class ClearableSqlDistributedCache: IDisposable
    {
        private readonly IDistributedCache _sqlServerCache = null;
        private readonly HashSet<string> _keys = new HashSet<string>();
        private readonly object _lock = new object();

        public ClearableSqlDistributedCache(IDistributedCache cache)
        {
            _sqlServerCache = cache;
        }

        public T Get<T>(string key)
        {
            var entry = _sqlServerCache.Get(key);

            var t = FromByteArray<T>(entry);
            
            return t;
        }

        public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        public T GetOrAdd<T>(string key, Func<T> getter, DateTimeOffset? expiry = null)
        {
            var entry = _sqlServerCache.Get(key);
            string serialized;
            T newEntry;

            if (entry != null)
            {
                serialized = Encoding.UTF8.GetString(entry);
                newEntry = JsonConvert.DeserializeObject<T>(serialized, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });

                return newEntry;
            }
            else
            {
                newEntry = getter();
                serialized = JsonConvert.SerializeObject(newEntry, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
                var encodedEntry = Encoding.UTF8.GetBytes(serialized);

                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTime.Now.AddHours(6));

                _sqlServerCache.Set(key, encodedEntry, options);

                return newEntry;
            }
        }


        public async Task<T> GetAsync<T>(string key)
        {
            string json = "Not found";
            var value = await _sqlServerCache.GetAsync(key);

            if(value != null)
            {
                json = Encoding.UTF8.GetString(value);
            }

            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            });  
        }

        
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getter, DateTimeOffset? expiry = null)
        {
            T entry;

            string serializedEntry;
            var encodedCache = await _sqlServerCache.GetAsync(key);

            if (encodedCache != null)
            {
                serializedEntry = Encoding.UTF8.GetString(encodedCache);
                entry = JsonConvert.DeserializeObject<T>(serializedEntry, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
                return Task.FromResult(entry).Result;
            }
            else
            {


                entry = await getter();
                serializedEntry = JsonConvert.SerializeObject(entry, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
                var encodedEntry = Encoding.UTF8.GetBytes(serializedEntry);

                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTime.Now.AddHours(6));

                await _sqlServerCache.SetAsync(key, encodedEntry, options);

                return entry;
            }
        }
        public void ClearAll(string cacheNamespace = null)
        {
            List<string> listToClear;

            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(cacheNamespace))
                {
                    listToClear = _keys.ToList();
                }
                else
                {
                    listToClear = _keys
                        .Where(k => k.StartsWith(cacheNamespace))
                        .ToList();
                }

                foreach (var key in listToClear)
                {
                    _keys.Remove(key);
                    _sqlServerCache.Remove(key);
                }
            }
        }

       /* internal object GetOrAddAsync<T>(string fullKey, Func<Task<T>> getter, DateTimeOffset? expiry)
        {
            return this.GetOrAddAsync(fullKey, getter, expiry);
        }*/

        public void ClearEntry(string key = null)
        {
            lock (_lock)
            {
                _keys.Remove(key);
                _sqlServerCache.Remove(key);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
