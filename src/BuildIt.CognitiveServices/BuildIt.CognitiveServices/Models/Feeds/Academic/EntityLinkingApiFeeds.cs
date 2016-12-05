using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.CognitiveServices.Models.Feeds.Academic
{
    public class EntityLinkingApiFeeds
    {
        public List<Entity> entities { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }

    public class Entry
    {
        public int offset { get; set; }
    }

    public class Match
    {
        public string text { get; set; }
        public List<Entry> entries { get; set; }
    }

    public class Entity
    {
        public List<Match> matches { get; set; }
        public string name { get; set; }
        public string wikipediaId { get; set; }
        public double score { get; set; }
    }

}
