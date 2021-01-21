using Cofoundry.Core.Caching;
using Cofoundry.BasicTestSite.Caching;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Cofoundry.BasicTestSite.Caching
{
    public class SqlDistributedObjectCacheFactory: IObjectCacheFactory
    {
        private readonly ClearableSqlDistributedCache _sqlCache;
        private readonly IDistributedCache _cache = null;
        public SqlDistributedObjectCacheFactory(
            IOptions<SqlServerCacheOptions> optionsAccessor,
            IDistributedCache cache
            )
        {
            _cache = cache;
            _sqlCache = new ClearableSqlDistributedCache(_cache);
        }

        /// <summary>
        /// Gets an instance of an IObjectCache.
        /// </summary>
        /// <param name="cacheNamespace">The cache namespace to organise cache entries under</param>
        /// <returns>IObjectCache instance</returns>
        public IObjectCache Get(string cacheNamespace)
        {
            return new SqlDistributedObjectCache(_sqlCache, cacheNamespace);
        }

        /// <summary>
        /// Clears all object caches created with the factory of all data
        /// </summary>
        public void Clear()
        {
            _sqlCache.ClearAll();
        }
    }
}
