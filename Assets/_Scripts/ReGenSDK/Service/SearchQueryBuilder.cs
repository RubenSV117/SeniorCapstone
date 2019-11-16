using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Model;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service
{
    public abstract class SearchQueryBuilder
    {
        /// <summary>
        /// Adds a term to the query.
        /// </summary>
        /// <param name="term">the term to search for</param>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder Add(string term);
        /// <summary>
        /// Clears the query.
        /// </summary>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder ClearQuery();
        /// <summary>
        /// Adds a tag to a list of tags that must be included.
        /// </summary>
        /// <param name="tag">the tag to include</param>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder IncludeTag([NotNull] string tag);
        /// <summary>
        /// Adds multiple tags to a list of tags that must be included.
        /// </summary>
        /// <param name="tag">the tags to include</param>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder IncludeTag([NotNull] IEnumerable<string> tag);
        /// <summary>
        /// Removes all previously included tags
        /// </summary>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder ClearIncludeTags();
        /// <summary>
        /// Adds a tag to a list of tags that must be not be included.
        /// </summary>
        /// <param name="tag">the tag to include</param>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder ExcludeTag([NotNull] string tag);
        /// <summary>
        /// Adds multiple tags to a list of tags that must not be included.
        /// </summary>
        /// <param name="tag">the tags to include</param>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder ExcludeTag([NotNull] IEnumerable<string> tag);
        /// <summary>
        /// Removes all previously excluded tags
        /// </summary>
        /// <returns>this</returns>
        public abstract SearchQueryBuilder ClearExcludeTags();

        /// <inheritdoc cref="ISearchApi.Search"/>
        public abstract Task<List<RecipeLite>> Execute();


    }
}