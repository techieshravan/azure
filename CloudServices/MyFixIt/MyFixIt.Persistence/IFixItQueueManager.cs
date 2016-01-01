using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyFixIt.Persistence
{
    public interface IFixItQueueManager
    {
        Task ProcessMessagesAsync(CancellationToken token);
        Task SendMessageAsync(FixItTask fixIt);
    }
}
