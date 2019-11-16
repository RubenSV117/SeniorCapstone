using System.Collections.Generic;
using System.Threading.Tasks;
using ReGenSDK.Model;

namespace ReGenSDK.Service.Impl
{
    internal class SearchQueryBuilderImpl : SearchQueryBuilder
    {
        private readonly SearchService _searchService;
        private string _query;
        private readonly HashSet<string> _include;
        private HashSet<string> _exclude;

        public SearchQueryBuilderImpl(SearchService searchService)
        {
            _searchService = searchService;
            _query = "";
            _include = new HashSet<string>();
            _exclude = new HashSet<string>();
        }

        public override SearchQueryBuilder Add(string term)
        {
            if (_query.Length == 0)
            {
                _query = term;
            }
            else
            {
                _query += $" {term}";
            }

            return this;
        }

        public override SearchQueryBuilder ClearQuery()
        {
            _query = "";
            return this;
        }

        public override SearchQueryBuilder IncludeTag(string tag)
        {
            _include.Add(tag);
            return this;
        }

        public override SearchQueryBuilder IncludeTag(IEnumerable<string> tag)
        {
            _include.UnionWith(tag);
            return this;
        }

        public override SearchQueryBuilder ClearIncludeTags()
        {
            _include.Clear();
            return this;
        }

        public override SearchQueryBuilder ExcludeTag(string tag)
        {
            _exclude.Add(tag);
            return this;
        }

        public override SearchQueryBuilder ExcludeTag(IEnumerable<string> tag)
        {
            _exclude.UnionWith(tag);
            return this;
        }

        public override SearchQueryBuilder ClearExcludeTags()
        {
            _exclude.Clear();
            return this;
        }

        public override Task<List<RecipeLite>> Execute()
        {
            return _searchService.Search(_query, _include, _exclude);
        }
    }
}