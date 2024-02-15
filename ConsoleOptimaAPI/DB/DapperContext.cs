using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Intrinsics.X86;

namespace OptimaAPI.DB
{

    public class DapperContext//: IDapperContext
    {
        private readonly IConfiguration _configuration;

        private static ConfigRoot configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ConfigRoot GetConfiguration => configuration;

        public IDbConnection CreateConnection()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string appJsonPath = Path.Combine(currentDirectory, "appsettings.json");
            string jsonContent = File.ReadAllText(appJsonPath);
            configuration = JsonConvert.DeserializeObject<ConfigRoot>(jsonContent);
            return new SqlConnection(configuration?.ConnectionStrings.DBContext);
        }

        public string? GetAttributeFilter(OwnSettings OwnSettings, List<OwnAttribute> lista)
        {
            if (lista.Any())
            {
                string prefix = OwnSettings.CanPrefixed ? OwnSettings.Prefix : "";
                return string.Format(" AND (DeA_Kod NOT IN ({0}) or DeA_Kod IS NULL)", string.Join(",", lista.Select(x => string.Format("'{0}{1}'", prefix, x.Name))));
            }
            return default;
        }
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
    public class ConfigRoot
    {
        public string AllowedHosts { get; set; }
        public List<APIUsers> APIUsers { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public Logging Logging { get; set; }
        public List<XLLoginInfo> OptimaUsers { get; set; }
        public List<XLLoginInfo> XLUsers { get; set; }
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
    public class APIUsers
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class XLLoginInfo
    {
        public System.Int32? UtworzWlasnaSesje { get; set; }
        public System.Int32? Winieta { get; set; }
        public System.Int32? TrybWsadowy { get; set; } = 1;
        public System.Int32? TrybNaprawy { get; set; } = 0;
        public System.Int32? PortHTTPSerweraKlucza { get; set; }
        public System.Int32? RodzajSerweraKlucza { get; set; }
        [StringLength(41)]
        public System.String ProgramID { get; set; }
        [StringLength(21)]
        public System.String Baza { get; set; }
        [StringLength(401)]
        public System.String OpeIdent { get; set; }
        [StringLength(128)]
        public System.String OpeHaslo { get; set; }
        [StringLength(200)]
        public System.String PlikLog { get; set; }
        [StringLength(101)]
        public System.String SerwerKlucza { get; set; }
        [StringLength(37)]
        public System.String SesjaKlucza { get; set; }
        [StringLength(129)]
        public System.String Serwer { get; set; }
    }

}
