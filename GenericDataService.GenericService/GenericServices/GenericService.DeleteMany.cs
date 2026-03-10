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
    protected virtual async Task<ServiceResult> HandleDeleteManyAsync(ServiceContext ctx)
    {
        var entities = JsonHelper.DeserializeGeneric<List<TEntity>>(ctx.Payload);

        if (entities == null || entities.Count == 0)
            return ServiceResult.Fail("Empty list.");

        _set.RemoveRange(entities);
        await _db.SaveChangesAsync();

        return ServiceResult.Ok(entities);
    }
}
