namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Project : ICloneable
    {
        public string Name { get; set; }
        public int CurrentOrderType { get; set; } = 1;
        public string PrimaryOrderName { get; set; }
        public string PrimaryOrderDescription { get; set; }
        public string SecundaryOrderName { get; set; }
        public string SecundaryOrderDescription { get; set; }
        public bool HideCompleted { get; set; } = true;
        public List<Game> Games { get; set; } = new List<Game>();

        public object Clone() => new Project
        {
            CurrentOrderType = CurrentOrderType,
            Name = Name,
            PrimaryOrderDescription = PrimaryOrderDescription,
            PrimaryOrderName = PrimaryOrderName,
            SecundaryOrderDescription = SecundaryOrderDescription,
            SecundaryOrderName = SecundaryOrderName,
            Games = Games.Select(o => (Game)o.Clone()).ToList(),
        };
    }
}
