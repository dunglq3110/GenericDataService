using GenericDataService.GenericService;
using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Pipeline.PreSteps
{
    public class AuthorizationStep : IPreStep
    {
        private readonly GenericServiceDbContext _db;
        public AuthorizationStep(GenericServiceDbContext db) => _db = db;

        public async Task<ServiceResult?> ExecuteAsync(ServiceContext ctx, Func<Task<ServiceResult?>> next)
        {
            var perm = await _db.ServicePermissions.FirstOrDefaultAsync(p =>
                p.UserId == ctx.UserId && p.Signature == ctx.Signature && p.Action == ctx.FunctionCode);

            if (perm is null || !perm.IsAllow)
                return ServiceResult.Fail($"Not authorized: {ctx.FunctionCode}");

            ctx.Permission = perm; // pass it forward via the context bag
            return await next();
        }
    }
}
