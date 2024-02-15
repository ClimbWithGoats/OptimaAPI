using System.Collections.Generic;

namespace OptimaCom.Controller
{
    public abstract class OwnSettings
    {
        public string Prefix { get; set; }
        public bool CanPrefixed { get; set; }
        public AttributeObjectType Type { get; set; }
        public List<OwnAttribute> Attribute { get; set; }
    }

    public enum AttributeObjectType
    {
        Ignore = 0,
        Commodity = 1,
        COntractor = 2,
        //   SrodekTrwaly = 3,
        Document = 4
    }
}
