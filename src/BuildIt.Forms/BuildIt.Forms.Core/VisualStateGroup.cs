using System.Collections.Generic;
using BuildIt.States.Interfaces;

namespace BuildIt.Forms.Core
{
    public class VisualStateGroup : List<VisualState>
    {
        public IStateGroup StateGroup { get; set; }

        public string Name { get; set; }

        public VisualState CurrentState { get; set; }



        public VisualStateGroup()
        {

        }

    }
}