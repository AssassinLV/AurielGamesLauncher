namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Data : ICloneable
    {
        public string LastPlayedProject { get; set; }
        public string LastPlayedGameName { get; set; }

        public List<Project> Projects { get; set; }

        public object Clone() => new Data
        {
            Projects = Projects.Select(o => (Project)o.Clone()).ToList(),
        };
    }
}
