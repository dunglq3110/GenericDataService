using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Pipeline.PreSteps
{
    public class ValidationStep : IPreStep
    {
        public async Task<ServiceResult?> ExecuteAsync(ServiceContext ctx, Func<Task<ServiceResult?>> next)
        {
            if (!ctx.Permission.IsValidation) return await next();

            // 2. Allowed-params check
            if (!string.IsNullOrWhiteSpace(ctx.Permission.AllowedParams))
            {
                var paramCheck = ValidateAllowedParams(ctx.Payload, ctx.Permission.AllowedParams);
                if (!paramCheck.Success) return paramCheck;
            }

            // 3. DTO validation (if enabled)
            if (ctx.Permission.IsValidation && !string.IsNullOrWhiteSpace(ctx.Permission.ValidationOptions))
            {
                var valCheck = ValidateDto(ctx.Payload, ctx.Permission.ValidationOptions);
                if (!valCheck.Success) return valCheck;
            }

            return await next();
        }

        private ServiceResult ValidateAllowedParams(JsonElement payload, string allowedParamsJson)
        {
            var allowed = JsonSerializer.Deserialize<List<string>>(allowedParamsJson)
                          ?? new List<string>();

            if (payload.ValueKind != JsonValueKind.Object)
                return ServiceResult.Fail("Payload must be a JSON object.");

            foreach (var prop in payload.EnumerateObject())
            {
                if (!allowed.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                    return ServiceResult.Fail($"Parameter '{prop.Name}' is not allowed for this action.");
            }

            return ServiceResult.Ok();
        }

        private ServiceResult ValidateDto(JsonElement payload, string validationOptionsJson)
        {
            var errors = new List<string>();
            var options = JsonSerializer.Deserialize<ValidationOptions>(validationOptionsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (options?.Rules is null) return ServiceResult.Ok();

            foreach (var rule in options.Rules)
            {
                var hasField = payload.TryGetProperty(rule.Field, out var fieldVal);
                var isEmpty = !hasField || fieldVal.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
                               || fieldVal.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(fieldVal.GetString());

                if (rule.Required == true && isEmpty)
                {
                    errors.Add($"'{rule.Field}' is required.");
                    continue;
                }

                if (isEmpty) continue;

                if (rule.MaxLength.HasValue && fieldVal.ValueKind == JsonValueKind.String)
                {
                    var str = fieldVal.GetString()!;
                    if (str.Length > rule.MaxLength.Value)
                        errors.Add($"'{rule.Field}' exceeds max length of {rule.MaxLength}.");
                }

                if ((rule.Min.HasValue || rule.Max.HasValue) && fieldVal.TryGetDecimal(out var num))
                {
                    if (rule.Min.HasValue && num < rule.Min.Value)
                        errors.Add($"'{rule.Field}' must be >= {rule.Min}.");
                    if (rule.Max.HasValue && num > rule.Max.Value)
                        errors.Add($"'{rule.Field}' must be <= {rule.Max}.");
                }

                if (!string.IsNullOrWhiteSpace(rule.Regex) && fieldVal.ValueKind == JsonValueKind.String)
                {
                    var str = fieldVal.GetString()!;
                    if (!System.Text.RegularExpressions.Regex.IsMatch(str, rule.Regex))
                        errors.Add($"'{rule.Field}' does not match required format.");
                }
            }

            return errors.Count == 0
                ? ServiceResult.Ok()
                : ServiceResult.Fail(string.Join(" | ", errors));
        }
    }
}
