using System.Collections.Generic;
using BuildIt.States.Interfaces;

namespace BuildIt.Forms
{
    public class VisualStateGroup : List<VisualState>
    {
        public IStateGroup StateGroup { get; set; }

        public string Name { get; set; }

        public string DefinitionCacheKey { get; set; }

        public VisualStateGroup()
        {

        }

    }
}