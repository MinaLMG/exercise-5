using Newtonsoft.Json;

namespace server
{
    public class Serialization
    {

        public void Serialize(object obj, string filePath)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public object Deserialize(string path, string type)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamReader(path))
            using (var reader = new JsonTextReader(sw))
            {
                switch (type)
                {
                    case "cat":
                        return serializer.Deserialize<List<Category>>(reader);
                    case "rec":
                        return serializer.Deserialize<List<Recipe>>(reader);
                    default:
                        return serializer.Deserialize(reader);
                }
            }
        }
    }
}
