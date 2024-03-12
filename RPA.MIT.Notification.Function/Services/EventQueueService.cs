using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.ServiceBusProvider;

namespace RPA.MIT.Notification.Function.Services;

public class EventQueueService : IEventQueueService
{
    private IConfiguration _configuration;
    private readonly ServiceBusProvider _serviceBusProvider;

    public EventQueueService(ServiceBusProvider serviceBusProvider, IConfiguration configuration)
    {
        _serviceBusProvider = serviceBusProvider;
        _configuration = configuration;
    }

    public async Task CreateMessage(string id, string status, string action, string message, string data)
    {
        var eventRequest = new Event()
        {
            Name = "Notification",
            Properties = new EventProperties()
            {
                Id = id,
                Status = status,
                Checkpoint = "Notification Api",
                Action = new EventAction()
                {
                    Type = action,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    Data = data
                }
            }
        };

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventRequest));
        await _serviceBusProvider.SendMessageAsync(_configuration["EventQueueName"] ,Convert.ToBase64String(bytes));
    }
}