using Newtonsoft.Json;

namespace server
{
    public class Serialization
    {
        public enum SerializationType
        {
            ListOfCategories,
            ListOfRecipes,
        }
        public void Serialize(object obj, string filePath)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public object Deserialize(string path, SerializationType type)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamReader(path))
            using (var reader = new JsonTextReader(sw))
            {
                switch (type)
                {
                    case SerializationType.ListOfCategories:
                        return serializer.Deserialize<List<Category>>(reader);
                    case SerializationType.ListOfRecipes:
                        return serializer.Deserialize<List<Recipe>>(reader);
                    default:
                        return serializer.Deserialize(reader);
                }
            }
        }
    }
}
