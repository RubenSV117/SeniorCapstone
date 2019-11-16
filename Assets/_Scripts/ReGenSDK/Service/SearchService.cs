using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Model;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service
{
    public abstract class SearchService : AbstractService, ISearchApi
    {
        protected SearchService(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
        }

        /// <summary>
        /// Returns a builder to help build queries.
        /// Builders can be reused.
        /// </summary>
        /// <returns>A SearchQueryBuilder</returns>
        public abstract SearchQueryBuilder Builder();

        /// <summary>
        /// Returns a list of recipes matching the query.
        /// </summary>
        /// <param name="query">The term(s) to search for.</param>
        /// <param name="includeTags">A list of tags to include.</param>
        /// <param name="excludeTags">A list of tags to exclude.</param>
        /// <returns>A List of recipes that may be empty but not null</returns>
        public abstract Task<List<RecipeLite>> Search([NotNull] string query, [NotNull] IEnumerable<string> includeTags,
            [NotNull] IEnumerable<string> excludeTags);

        public abstract Task<List<RecipeLite>> Search(string q, TagFilter tags);
    }
}