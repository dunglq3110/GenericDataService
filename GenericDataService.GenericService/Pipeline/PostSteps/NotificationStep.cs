using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Models;
using GenericDataService.GenericService.Notification;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Pipeline.PostSteps
{
    internal class NotificationStep : IPostStep
    {
        private readonly IHubContext<EventHub> _hubContext;
        private readonly EventRegistryService _eventRegistry;

        public NotificationStep(IHubContext<EventHub> hubContext, EventRegistryService eventRegistry)
        {
            _hubContext = hubContext;
            _eventRegistry = eventRegistry;
        }

        public async Task ExecuteAsync(ServiceContext ctx)
        {
            // Check if notification is enabled
            if (!ctx.Permission.IsNotify) return;
            if (string.IsNullOrWhiteSpace(ctx.Permission.Events)) return;

            var listEvents = JsonSerializer.Deserialize<List<string>>(ctx.Permission.Events)
                             ?? new List<string>();

            // Unique connectionIds
            var connectionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in listEvents)
            {
                var connections = await _eventRegistry.GetConnectionIdsByActionAsync(item);

                foreach (var conn in connections)
                {
                    connectionIds.Add(conn);
                }
            }

            if (connectionIds.Count == 0)
                return;

            foreach (var item in listEvents)
            {
                await _hubContext.Clients
                    .Clients(connectionIds.ToList())
                    .SendAsync(item, new
                    {
                        message = $"Data from {ctx.Signature} - {ctx.FunctionCode}",
                        data = ctx.Result?.Data
                    });
            }
        }
    }
}
