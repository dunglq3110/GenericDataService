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
    protected virtual async Task<ServiceResult> HandleGetDataAsync(ServiceContext context)
    {
        var items = await _set.ToListAsync();
        return ServiceResult.Ok(items);
    }
}
