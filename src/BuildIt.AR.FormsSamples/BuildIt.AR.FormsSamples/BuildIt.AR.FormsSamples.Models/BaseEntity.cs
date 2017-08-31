using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.AR.FormsSamples.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
