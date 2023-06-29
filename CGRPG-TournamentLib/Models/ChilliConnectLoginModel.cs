namespace CGRPG_Tournament.Models
{
    public class ChilliConnectLoginModel
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ChilliConnectId { get; set; } = null!;
        
        public string ConnectAccessToken { get; set; } = null!;
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string MetricsAccessToken { get; set; } = null!;
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string CatalogVersion { get; set; } = null!;
    }
}