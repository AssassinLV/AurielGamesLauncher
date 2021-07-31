namespace AurielGamesLauncher.Helpers
{
    using Models;
    using Newtonsoft.Json;
    using System.IO;

    public static class JsonHelper
    {
        public static Data LoadData()
        {
            using StreamReader file = File.OpenText(@"GamesDefinition.json");
            JsonSerializer serializer = new ();
            return (Data)serializer.Deserialize(file, typeof(Data));
        }

        public static void SaveData(this Data data)
        {
            using StreamWriter file = File.CreateText(@"GamesDefinition.json");
            JsonSerializer serializer = new();
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(file, data);
        }

        public static void SaveData<T>(this T data, string fileName)
        {
            using StreamWriter file = File.CreateText(fileName);
            JsonSerializer serializer = new();
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(file, data);
        }
    }
}
