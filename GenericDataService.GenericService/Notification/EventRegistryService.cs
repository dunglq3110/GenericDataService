using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Notification
{
    public class EventRegistryService
    {
        private readonly GenericServiceDbContext _db;

        public EventRegistryService(GenericServiceDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all active SignalR connection IDs for users registered to the given action/event.
        /// Example: action = "ProductCreated" => returns admin's connection IDs
        ///          action = "ProductDelete"  => returns admin's and dandy's connection IDs
        /// </summary>
        public async Task<List<string>> GetConnectionIdsByActionAsync(string action)
        {
            // Find all userIds who have this action in their events JSON array
            var allRegistrations = await _db.UserEventRegistrations.ToListAsync();

            var matchingUserIds = allRegistrations
                .Where(r =>
                {
                    var events = JsonSerializer.Deserialize<List<string>>(r.Events) ?? [];
                    return events.Contains(action, StringComparer.OrdinalIgnoreCase);
                })
                .Select(r => r.UserId)
                .ToHashSet();

            // Cross-reference with active connections
            var connectionIds = await _db.ConnectionRegistries
                .Where(c => matchingUserIds.Contains(c.UserId))
                .Select(c => c.ConnectionId)
                .ToListAsync();

            return connectionIds;
        }
    }
}
