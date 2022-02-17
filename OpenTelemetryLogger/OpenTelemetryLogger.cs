using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace OpenTelemetryLogger
{
    public class OpenTelemetryAttribute : ActionFilterAttribute
    {
        readonly Meter meter;
        readonly Histogram<long> histogram;

        public OpenTelemetryAttribute(string meterName, string gaugeName)
        {
            meter = new Meter(meterName);
            histogram = meter.CreateHistogram<long>(gaugeName);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var tag = new System.Collections.Generic.KeyValuePair<string, object>("context", context);

            var stopWatch = Stopwatch.StartNew();
            await next();

            histogram.Record(stopWatch.ElapsedMilliseconds, tag);
        }
    }
}
