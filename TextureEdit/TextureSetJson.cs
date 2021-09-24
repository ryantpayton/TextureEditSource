using Newtonsoft.Json;

namespace TextureEdit
{
    public class TextureSetJson
    {
        public TextureSetJson(string nameWithoutExtension)
        {
            int lastIndex = nameWithoutExtension.LastIndexOf('\\') + 1;
            string textureName = nameWithoutExtension.Substring(lastIndex, nameWithoutExtension.Length - lastIndex);
            
            TextureSet = new TextureSet(textureName);
        }

        [JsonProperty("format_version")]
        public string FormatVersion => "1.16.100";

        [JsonProperty("minecraft:texture_set")]
        public TextureSet TextureSet { get; }
    }

    public class TextureSet
    {
        internal TextureSet(string textureName)
        {
            Color = textureName;
            MetalnessEmissiveRoughness = $"{textureName}_mer";
            Normal = $"{textureName}_normal";
        }

        [JsonProperty("color")]
        public string Color { get; }

        [JsonProperty("metalness_emissive_roughness")]
        public string MetalnessEmissiveRoughness { get; }
        
        [JsonProperty("normal")]
        public string Normal { get; }
    }
}