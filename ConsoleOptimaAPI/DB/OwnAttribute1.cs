namespace OptimaAPI.DB
{

    public class DocumentsSettings : OwnSettings
    {

    }
    public class ContractorsSettings : OwnSettings
    {

    }
    public class CommoditiesSettings : OwnSettings
    {

    }
    public class OwnAttribute
    {
        public string JsonPropertyName { get; set; } = "";
        public string Name { get; set; } = "";
        public AttributeFormat Format { get; set; } = 0;
        public object Value { get; set; }
    }

    public enum AttributeFormat
    {
        Ignore = -1,
        Text = 0,
        Numeric = 2,
        List = 3,
        Date = 4,
        Binary = 5
    }
}