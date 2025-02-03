
using Rijnkaai.Domain;

namespace Rijnkaai.Abstractions
{
    public interface ISlackService
    {
        Task PostMessageToSlack(IEnumerable<ParkingReport> report);
        Task PostSingleMessageToSlack(ParkingReport parkingReport);
    }
}