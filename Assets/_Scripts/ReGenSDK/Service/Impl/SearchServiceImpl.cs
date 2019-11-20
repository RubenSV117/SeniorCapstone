using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ReGenSDK.Model;
using Debug = UnityEngine.Debug;

namespace ReGenSDK.Service.Impl
{
    class SearchServiceImpl : SearchService
    {
        public SearchServiceImpl(string endpoint, Func<Task<string>> authorizationProvider): base(endpoint, authorizationProvider)
        {
        }

        public override SearchQueryBuilder Builder()
        {
            return new SearchQueryBuilderImpl(this);
        }

        public override Task<List<RecipeLite>> Search(string query, IEnumerable<string> includeTags,
            IEnumerable<string> excludeTags)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (includeTags == null) throw new ArgumentNullException(nameof(includeTags));
            if (excludeTags == null) throw new ArgumentNullException(nameof(excludeTags));
            return Search(query, new TagFilter
            {
                IncludeTags = includeTags.ToList(),
                ExcludeTags = excludeTags.ToList()
            });
        }

        public override Task<List<RecipeLite>> Search(string query, TagFilter tags)
        {
            return HttpGet()
                .Query("q", query)
                .Body(tags)
                .Parse<List<RecipeLiteJson>>()
                .Map(jsonList => jsonList?.ConvertAll(j => j.ToActual()))
                .Execute();
        }
    }
}