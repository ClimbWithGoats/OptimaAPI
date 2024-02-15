using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OptimaCom.Controller
{
    public class ConfigRoot
    {
        public string AllowedHosts { get; set; }
        public List<APIUsers> APIUsers { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public Logging Logging { get; set; }
        public List<XLLoginInfo> OptimaUsers { get; set; }
        public Attributes Attributes { get; set; }

    }
    public class Attributes
    {
        public List<OwnAttribute> All { get; set; }
        public DocumentsSettings DocumentAlias { get; set; }
        public ContractorsSettings contractorAlias { get; set; }
        public CommoditiesSettings CommodityAlias { get; set; }
        public bool CanPrefixed { get; set; }
    }
    public class ConnectionStrings
    {
        public string DBContext { get; set; }
    }
    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }
    public class LogLevel
    {
        public string Default { get; set; }

        [JsonProperty("Microsoft.AspNetCore")]
        public string MicrosoftAspNetCore { get; set; }
    }
    public class APIUsers
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class XLLoginInfo
    {
        //public System.Int32? UtworzWlasnaSesje { get; set; }
        //public System.Int32? Winieta { get; set; }
        //public System.Int32? TrybWsadowy { get; set; } = 1;
        //public System.Int32? TrybNaprawy { get; set; } = 0;
        //public System.Int32? PortHTTPSerweraKlucza { get; set; }
        //public System.Int32? RodzajSerweraKlucza { get; set; }
        //[StringLength(41)]
        //public System.String ProgramID { get; set; }
        [StringLength(21)]
        public System.String NazwaFirmy { get; set; }
        [StringLength(401)]
        public System.String OpeIdent { get; set; }
        [StringLength(128)]
        public System.String OpeHaslo { get; set; }
        //[StringLength(200)]
        //public System.String PlikLog { get; set; }
        //[StringLength(101)]
        //public System.String SerwerKlucza { get; set; }
        //[StringLength(37)]
        //public System.String SesjaKlucza { get; set; }
        //[StringLength(129)]
        //public System.String Serwer { get; set; }
    }
}