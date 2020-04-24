using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Newtonsoft.Json;
using RcoResendBuffer.Models;
using RcoResendBuffer.Models.Enums;
using RcoResendBuffer.Context;

namespace RcoResendBuffer.Service
{

    public class AuthSyncMessages
    {
        private List<AuthSyncMessage> Messages { get; set; }
        public void AddMessage(AuthSyncMessage authSyncMessage)
        {
            Messages.Add(authSyncMessage);
        }
        public void Submit()
        {

        }
    }

    public class QueueWorkerService : BackgroundService
    {
        private readonly ILogger<QueueWorkerService> _logger;
        private readonly IHost _host;
        private readonly IHttpClientFactory _httpClientFactory;


        public QueueWorkerService(ILogger<QueueWorkerService> logger, IHost host, IHttpClientFactory http)
        {
            _host = host;
            _logger = logger;
            _httpClientFactory = http;
        }

        private List<AuthSyncMessage> Messages { get; set; } = new List<AuthSyncMessage>();

        public void AddMessage(object data, string url)
        {
            var msg = new AuthSyncMessage();
            msg.Data = JsonConvert.SerializeObject(data, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            msg.HttpVerb = HttpVerb.Post;
            msg.State = MessageState.Unprocessed;
            msg.Url = url;
            Messages.Add(msg);
        }
        public void Submit()
        {
            if (!Messages.Any())
                return;
            using (var dbContext = _host.Services.GetService<AuthContext>())
            {
                foreach (var msg in Messages)
                {
                    dbContext.AuthSyncMessages.Add(msg);

                }
                dbContext.SaveChangesAsync();
            }
            Messages.Clear();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var dbContext = _host.Services.GetService<AuthContext>())
                    {
                        var dbMsg = dbContext.AuthSyncMessages.FirstOrDefault();
                        if (dbMsg == null)
                        {
                            await Task.Delay(3000);
                            _logger.LogInformation("Nothing to do!");
                        }
                        else
                        {
                            _logger.LogInformation($"Procces message {dbMsg.Id}");
                            //var http = _httpClientFactory.CreateClient();
                            //await http.GetAsync("http://lassem.se").ContinueWith(res =>
                            //{
                              //  if (res.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
                                //{
                                    var msg = dbContext.AuthSyncMessages.FirstOrDefault(a => a.Id == dbMsg.Id);
                                    dbContext.AuthSyncMessages.Remove(msg);
                                    await dbContext.SaveChangesAsync();
                                //}
                            //});
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fail!");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }

}
