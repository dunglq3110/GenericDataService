using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.Infrastructure.Pipeline
{
    public class ServicePipeline
    {
        private readonly IEnumerable<IPreStep> _preSteps;
        private readonly IEnumerable<IPostStep> _postSteps;

        public ServicePipeline(
            IEnumerable<IPreStep> preSteps,
            IEnumerable<IPostStep> postSteps)
        {
            _preSteps = preSteps;
            _postSteps = postSteps;
        }

        public async Task<ServiceResult> RunAsync(
            ServiceContext ctx,
            Func<ServiceContext, Task<ServiceResult>> handler)
        {
            // 1. Build and run the pre-step chain
            Func<Task<ServiceResult?>> chain = async () =>
            {
                // 2. This is the "inner" — calls the actual DB handler
                var result = await handler(ctx);
                ctx.Result = result;

                // 3. Run post-steps sequentially after handler completes
                foreach (var post in _postSteps)
                {
                    try
                    {
                        await post.ExecuteAsync(ctx);
                    }
                    catch (Exception ex)
                    {
                        // Post-steps (audit, cache, notify) should NEVER
                        // fail the main response — log and continue
                        Console.Error.WriteLine($"[PostStep:{post.GetType().Name}] {ex.Message}");
                    }
                }

                return ctx.Result;
            };

            foreach (var step in _preSteps.Reverse())
            {
                var current = step;
                var nextInChain = chain;
                chain = () => current.ExecuteAsync(ctx, () => nextInChain()!);
            }

            return await chain() ?? ServiceResult.Fail("Pipeline produced no result.");
        }
    }
}
