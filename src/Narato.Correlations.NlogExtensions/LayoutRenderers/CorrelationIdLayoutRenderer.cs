using Narato.Correlations.Correlations;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;
using System.Text;

namespace Narato.Correlations.NlogExtensions.LayoutRenderers
{
    [LayoutRenderer("correlation-id")]
    public class CorrelationIdLayoutRenderer : AspNetLayoutRendererBase
    {

        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var headerDictionary = HttpContextAccessor?.HttpContext?.Response?.Headers;

            if (headerDictionary == null)
                return;

            if (headerDictionary.ContainsKey(CorrelationIdProvider.CORRELATION_ID_HEADER_NAME))
            {
                builder.Append(headerDictionary[CorrelationIdProvider.CORRELATION_ID_HEADER_NAME]);
            }
            else
            {
                builder.Append("**NO CORRELATION ID ON RESPONSE!**");
            }
        }
    }
}
