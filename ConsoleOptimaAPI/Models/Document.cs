//Kontrahenci

using Newtonsoft.Json;

namespace OptimaAPI.Models
{
    public class Document : BaseDocument
    {
        [JsonProperty("Pozycje")]
        public new List<TraElem>? Lista { get; set; } = new List<TraElem>();
        [JsonProperty("OptimaDataWystawienia")]
        public new string TrnDataWystawienia { get => base.TrnDataWystawienia.Value.ToString("d") ?? ""; }
        [JsonProperty("OptimaDataOperacji")]
        public new string TrnDataOperacji { get => base.TrnDataOperacji.Value.ToString("d") ?? ""; }


        public Document(BaseDocument baseDocument)
        {
            base.PropertyChanged += DeserializeJson;
            base.Initialize(baseDocument);
            base.PropertyChanged -= DeserializeJson;
        }

        private void DeserializeJson(object sender, PropertyEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Lista)))
            {
                Lista = JsonConvert.DeserializeObject<List<TraElem>>(e.Json) ?? new List<TraElem>();
            }
        }

    }
}

public class TraElem
{
    [JsonProperty("GIDLp")]
    public int LP { get; set; }
    [JsonProperty("TowarNazwa")]
    public string? Nazwa { get; set; }
    [JsonProperty("TowarKod")]
    public string? Kod { get; set; }
    [JsonProperty("Ilosc")]
    public string? Ilosc { get; set; }
    [JsonProperty("JMz")]
    public string? JM { get; set; }
    [JsonProperty("Cena")]
    public string? cena { get; set; }
    [JsonProperty("Wartosc")]
    public string? Wartosc { get; set; }
}
