namespace TrustGate.Api.Models
{
    public class Entitlement
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string Permissions { get; set; }
        public string RiskLevel { get; set; }
    }
}
