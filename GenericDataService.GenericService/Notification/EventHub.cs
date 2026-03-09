using GenericDataService.GenericService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GenericDataService.GenericService.Notification
{
    public class EventHub : Hub
    {
        private readonly GenericServiceDbContext _db;

        public EventHub(GenericServiceDbContext db)
        {
            _db = db;
        }

        public async Task Register(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var entry = new ConnectionRegistry
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId,
                    ConnectionTime = DateTime.UtcNow
                };
                _db.ConnectionRegistries.Add(entry);
                await _db.SaveChangesAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var record = await _db.ConnectionRegistries
                .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

            if (record != null)
            {
                _db.ConnectionRegistries.Remove(record);
                await _db.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
