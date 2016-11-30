using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    public class EntityLinking
    {
        public List<Entity> entities { get; set; }
        public string message { get; set; }
        public string code { get; set; }

        public string Name {
            get
            {
                var Name = string.Empty;
                foreach (var entity in entities)
                {
                    foreach (var entityMatch in entity.matches)
                    {
                        Name = $"{entityMatch.text } match:  {entity.name}, wikipedia Id: {entity.wikipediaId}"; 
                    }
                }
                return Name;
            }
        }
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

        public string Result
        {
            get
            {
                var displayName = string.Empty;
                foreach (var match in matches)
                {
                    displayName = $"{ match.text} match: {name}. wikipedia Id:{wikipediaId}";
                }
                return displayName;
            }
        }
    }
}
