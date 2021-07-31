namespace Models
{
    using System;
    using Newtonsoft.Json;

    public class Game : ICloneable
    {
        public string Name { get; set; }
        public int PrimaryOrder { get; set; }
        public int? SecundaryOrder { get; set; }
        public bool IsFinished { get; set; } = false;
        public string Location { get; set; }
        public string PathFromLocationToExe { get; set; }
        public string ExecutableName { get; set; }

        [JsonIgnore]
        public string FullPath => $"{Location}{(string.IsNullOrEmpty(PathFromLocationToExe) ? $"//{ExecutableName}" : $"//{PathFromLocationToExe}//{ExecutableName}")}";
        public string CheckSum { get; set; }

        public object Clone()
        {
            return new Game
            {
                Name = Name,
                CheckSum = CheckSum,
                ExecutableName = ExecutableName,
                PathFromLocationToExe = PathFromLocationToExe,
                PrimaryOrder = PrimaryOrder,
                SecundaryOrder = SecundaryOrder,
            };
        }
    }
}
