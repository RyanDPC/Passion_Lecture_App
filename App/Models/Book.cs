using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace App.Models
{
    public enum RatingBook
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }
    public enum RatingComments
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }
    public class Book
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string name { get; set; }
        public string passage { get; set; }
        public string summary { get; set; }
        public int editionYear { get; set; }
        public Blob coverImage { get; set; }
        public int pages { get; set; }

        public DateTime Created { get; set; }
    }
}
