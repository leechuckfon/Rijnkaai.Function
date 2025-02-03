using Rijnkaai.Domain;

namespace Rijnkaai.Abstractions
{
    public interface IRijnkaaiService
    {
        Task<IEnumerable<ParkingReport>> GetRijnkaaiClosedDates();
    }
}