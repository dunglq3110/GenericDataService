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

namespace GenericDataService.GenericService;

public abstract partial class GenericService<TEntity> : IGenericService
    where TEntity : class, new()
{

    private readonly ServicePipeline _pipeline;
    protected readonly DbContext _db;
    protected readonly DbSet<TEntity> _set;
    private readonly Dictionary<string, Func<ServiceContext, Task<ServiceResult>>> _handlers;

    protected virtual string CurrentUserId => "admin";

    protected GenericService(DbContext db, ServicePipeline pipeline)
    {
        _db = db;
        _set = db.Set<TEntity>();
        _pipeline = pipeline;

        _handlers = new()
        {
            ["GetData"] = HandleGetDataAsync,
            ["Insert"] = HandleInsertAsync,
            ["Update"] = HandleUpdateAsync,
            ["Delete"] = HandleDeleteAsync,
            ["InsertMany"] = HandleInsertManyAsync,
            ["UpdateMany"] = HandleUpdateManyAsync,
            ["DeleteMany"] = HandleDeleteManyAsync,
            ["SP"] = HandleSpAsync
        };
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


    private Task<ServiceResult> DispatchAsync(ServiceContext ctx)
    {
        if (_handlers.TryGetValue(ctx.FunctionCode, out var handler))
            return handler(ctx);

        return HandleCustomAsync(ctx);
    }

    protected void RegisterHandler(
        string functionCode,
        Func<ServiceContext, Task<ServiceResult>> handler)
    {
        _handlers[functionCode] = handler;
    }
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
}

