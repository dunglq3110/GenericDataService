using GenericDataService.GenericService.Attributes;
using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Models;
using GenericDataService.Infrastructure.Pipeline;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.Infrastructure
{
    public abstract class GenericService<TEntity> : IGenericService
        where TEntity : class, new()
    {

        private readonly ServicePipeline _pipeline;
        protected readonly DbContext _db;
        protected readonly DbSet<TEntity> _set;
        protected virtual string CurrentUserId => "admin";

        protected GenericService(DbContext db, ServicePipeline pipeline)
        {
            _db = db;
            _set = db.Set<TEntity>();
            _pipeline = pipeline;
        }

        private string GetSignature()
        {
            var attr = GetType().GetCustomAttribute<ServiceSignatureAttribute>();

            if (attr == null)
                throw new InvalidOperationException(
                    $"{GetType().Name} must define [ServiceSignature].");

            return attr.Signature;
        }

        // ─── Entry point ──────────────────────────────────────────────────────────

        public Task<ServiceResult> HandleAsync(string functionCode, JsonElement payload)
        {
            var ctx = new ServiceContext
            {
                UserId = CurrentUserId,
                Signature = GetSignature(),
                FunctionCode = functionCode,
                Payload = payload
            };

            // GenericService passes ITSELF as the handler — pipeline calls back into it
            return _pipeline.RunAsync(ctx, DispatchAsync);
        }


        private Task<ServiceResult> DispatchAsync(ServiceContext ctx) =>
            ctx.FunctionCode switch
            {
                "GetData" => HandleGetDataAsync(ctx),
                "Insert" => HandleInsertAsync(ctx),
                "Update" => HandleUpdateAsync(ctx),
                "Delete" => HandleDeleteAsync(ctx),
                "SP" => HandleSpAsync(ctx),
                _ => HandleCustomAsync(ctx)
            };

        protected virtual async Task<ServiceResult> HandleGetDataAsync(ServiceContext context)
        {
            var items = await _set.ToListAsync();
            return ServiceResult.Ok(items);
        }

        protected virtual async Task<ServiceResult> HandleInsertAsync(ServiceContext context)
        {
            var entity = DeserializeDto(context.Payload);

            _set.Add(entity);
            await _db.SaveChangesAsync();
            return ServiceResult.Ok(entity);
        }

        protected virtual async Task<ServiceResult> HandleUpdateAsync(ServiceContext context)
        {
            var entity = DeserializeDto(context.Payload);

            _set.Update(entity);
            await _db.SaveChangesAsync();
            return ServiceResult.Ok(entity);
        }

        protected virtual async Task<ServiceResult> HandleDeleteAsync(ServiceContext context)
        {
            var entity = DeserializeDto(context.Payload);

            _set.Remove(entity);
            await _db.SaveChangesAsync();
            return ServiceResult.Ok("Deleted successfully.");
        }

        protected virtual async Task<ServiceResult> HandleSpAsync(
            ServiceContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Permission.SpName))
                return ServiceResult.Fail("Stored procedure name is not configured.");

            // Build params from AllowedParams list
            var sqlParams = new List<object>();
            var paramNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(context.Permission.AllowedParams))
            {
                var allowed = JsonSerializer.Deserialize<List<string>>(context.Permission.AllowedParams)!;
                foreach (var name in allowed)
                {
                    if (context.Payload.TryGetProperty(name, out var val))
                    {
                        paramNames.Add($"@{name} = {{{sqlParams.Count}}}");
                        sqlParams.Add(GetRawValue(val));
                    }
                }
            }

            var sql = paramNames.Count > 0
                ? $"EXEC {context.Permission.SpName} {string.Join(", ", paramNames)}"
                : $"EXEC {context.Permission.SpName}";

            var results = await _set.FromSqlRaw(sql, sqlParams.ToArray()).ToListAsync();

            return ServiceResult.Ok();
        }

        protected virtual TEntity DeserializeDto(JsonElement payload) =>
            JsonSerializer.Deserialize<TEntity>(payload.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new TEntity();

        private async Task<ServiceResult> HandleCustomAsync(ServiceContext context)
        {
            var method = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m =>
                    m.GetCustomAttribute<FunctionCodeAttribute>()?.Code == context.FunctionCode);

            if (method is null)
                return ServiceResult.Fail($"No handler found for function code '{context.FunctionCode}'.");

            var result = method.Invoke(this, new object[] { context.Payload });

            return result is Task<ServiceResult> task
                ? await task
                : ServiceResult.Fail("Custom handler must return Task<ServiceResult>.");
        }

        private static object GetRawValue(JsonElement el) => el.ValueKind switch
        {
            JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => DBNull.Value,
            _ => el.GetString() ?? string.Empty
        };
    }
    
}
