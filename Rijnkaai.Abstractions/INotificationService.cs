using Rijnkaai.Domain;

namespace Rijnkaai.Abstractions
{
    public interface INotificationService
    {
        Task PostMultipleReports(IEnumerable<ParkingReport> report);
        Task PostSingleReport(ParkingReport parkingReport);
    }
}