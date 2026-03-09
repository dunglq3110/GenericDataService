using GenericDataService.GenericService;
using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Pipeline.PostSteps
{
    public class AuditStep : IPostStep
    {
        private readonly GenericServiceDbContext _db;
        public AuditStep(GenericServiceDbContext db) => _db = db;

        public async Task ExecuteAsync(ServiceContext ctx)
        {
            if (ctx.Result is null) return;

            //_db.AuditLogs.Add(new AuditLog
            //{
            //    UserId = ctx.UserId,
            //    Signature = ctx.Signature,
            //    Action = ctx.FunctionCode,
            //    Success = ctx.Result.Success,
            //    OccurredAt = DateTime.UtcNow,
            //    ResultSummary = ctx.Result.Message ?? "ok"
            //});

            //await _db.SaveChangesAsync();
        }
    }
}
