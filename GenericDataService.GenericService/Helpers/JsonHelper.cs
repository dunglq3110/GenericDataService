using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Helpers
{
    public static class JsonHelper
    {
        public static TEntity DeserializeGeneric<TEntity>(JsonElement payload) =>
            JsonSerializer.Deserialize<TEntity>(payload.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        public static object GetRawValue(JsonElement el) => el.ValueKind switch
        {
            JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => DBNull.Value,
            _ => el.GetString() ?? string.Empty
        };
    }
}
