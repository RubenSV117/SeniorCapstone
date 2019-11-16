using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Model;

namespace ReGenSDK.Service.Api
{
    public interface ISearchApi
    {
        /// <summary>
        /// Returns a list of recipes matching the query.
        /// </summary>
        /// <param name="query">The query term(s).</param>
        /// <param name="tags">A list of tags to include and exclude</param>
        /// <returns>A List of recipes matching the query sorted by relevance</returns>
//        [Post("")]
        Task<List<RecipeLite>> Search([NotNull] string q, [NotNull] TagFilter tags);
    }
}