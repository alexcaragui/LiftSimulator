namespace Lift.Data.Models
{
    public class LiftEvent
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty; // "StateChange", "Emergency", "Movement"
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}