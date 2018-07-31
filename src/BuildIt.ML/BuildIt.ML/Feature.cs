using System;
using System.Collections.Generic;
using System.Text;

namespace BuildIt.ML
{
    public class Feature
    {
        public string Name { get; set; }

        public virtual FeatureType FeatureType { get; }
    }
}
