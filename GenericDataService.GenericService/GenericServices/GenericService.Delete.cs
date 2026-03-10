using GenericDataService.GenericService.Helpers;
using GenericDataService.GenericService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService;

public abstract partial class GenericService<TEntity>
{
    protected virtual async Task<ServiceResult> HandleDeleteAsync(ServiceContext context)
    {
        var entity = JsonHelper.DeserializeGeneric<TEntity>(context.Payload);

        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok("Deleted successfully.");
    }
}
