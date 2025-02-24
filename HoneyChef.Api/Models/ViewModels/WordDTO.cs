namespace IOITCore.Models.ViewModels
{
    public class WordDTO
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public WordDTO(string key, string value)
        {
            Key = key;
            Value = value;
        }

    }
}
