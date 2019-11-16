using System;
using System.Collections.Generic;

namespace ReGenSDK.Service
{
    [Serializable]
    public class TagFilter
    {
        public List<string> IncludeTags;
        public List<string> ExcludeTags;
    }
}