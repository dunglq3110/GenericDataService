using GenericDataService.GenericService.Helpers;
using GenericDataService.GenericService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService;

public abstract partial class GenericService<TEntity>
{
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
                    sqlParams.Add(JsonHelper.GetRawValue(val));
                }
            }
        }

        var sql = paramNames.Count > 0
            ? $"EXEC {context.Permission.SpName} {string.Join(", ", paramNames)}"
            : $"EXEC {context.Permission.SpName}";

        var results = await _set.FromSqlRaw(sql, sqlParams.ToArray()).ToListAsync();

        return ServiceResult.Ok();
    }
}
