using System;

namespace SpecEasy.Specs.DatabaseIntegration.Model
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }
    }
}
