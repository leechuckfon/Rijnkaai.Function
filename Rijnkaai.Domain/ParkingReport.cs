namespace Rijnkaai.Domain
{
    public class ParkingReport
    {
        public string DisplayDate { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
