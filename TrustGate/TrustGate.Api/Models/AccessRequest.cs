namespace TrustGate.Api.Models
{
    public class AccessRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EntitlementId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string DecidedBy { get; set; }
    }
}
