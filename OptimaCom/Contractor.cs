using Newtonsoft.Json;
using System;

namespace OptimaCom
{
    public class Commodity : OptimaBase
    {
        public string Kod { get; set; }
        public string Nazwa { get; set; }
    }
    public class Contractor : OptimaBase
    {
        public string Akronim { get; set; }
    }
    public class Document : OptimaBase
    {
        public string Akronim { get; set; }
        public string NumerPelny { get; set; }
    }

    public class OptimaBase
    {
        public string GIDNumer { get; set; } 
        public string OptimaId { get; set; }
        public string StatusZmiany { get; set; }

        // właściwości dla Attributes/All
        [JsonIgnore]
        public string DataZmiany { get => DateTime.UtcNow.ToString("s"); }
    }
}