using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace semenova_library
{
    public class Partner_Types
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public virtual ICollection<Partners> Partners { get; set; } = new List<Partners>();
    }
}