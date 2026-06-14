namespace Lift.Data.Models
{
    public class ProcessInterruption
    {
        public int Id { get; set; }

        public int StoppedByUserId { get; set; }
        public User? StoppedByUser { get; set; }
        public DateTime StoppedAt { get; set; }

        public int? RestartedByUserId { get; set; }
        public User? RestartedByUser { get; set; }
        public DateTime? RestartedAt { get; set; }
    }
}