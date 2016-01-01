using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using MyFixIt.Logging;
using MyFixIt.Persistence;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MyFixIt.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private IContainer _container;
        private ILogger _logger;
        readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public override void Run()
        {
            _logger.Information("MyFixIt.WorkerRole entry point called");

            Task task = RunAsync(_tokenSource.Token);
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled exception in FixIt worker role.");
            }
        }

        private async Task RunAsync(CancellationToken token)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                IFixItQueueManager queueManager = scope.Resolve<IFixItQueueManager>();
                try
                {
                    await queueManager.ProcessMessagesAsync(token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception in worker role Run loop.");
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            var builder = new ContainerBuilder();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<FixItTaskRepository>().As<IFixItTaskRepository>();
            builder.RegisterType<FixItQueueManager>().As<IFixItQueueManager>();
            _container = builder.Build();

            _logger = _container.Resolve<ILogger>();

            return base.OnStart();
        }

        public override void OnStop()
        {
            _tokenSource.Cancel();
            _tokenSource.Token.WaitHandle.WaitOne();
            base.OnStop();
        }
    }
}
