using Microsoft.AspNetCore.Http;
using Moq;
using Narato.Correlations.NlogExtensions.LayoutRenderers;
using System;
using System.Reflection;
using System.Text;
using Xunit;

namespace Narato.Correlations.NlogExtensions.Test.LayoutRenderers
{
    public class CorrelationIdLayoutRendererTest
    {
        [Fact]
        public void TestCorrelationIdGetsAdded()
        {
            var guid = Guid.NewGuid();
            var responseHeaderDictionary = new HeaderDictionary();
            responseHeaderDictionary.Add("Nar-Correlation-Id", guid.ToString());

            var httpResponseMoq = new Mock<HttpResponse>();
            httpResponseMoq.SetupGet(hrm => hrm.Headers).Returns(responseHeaderDictionary);

            var httpContextMoq = new Mock<HttpContext>();
            httpContextMoq.SetupGet(hcm => hcm.Response).Returns(httpResponseMoq.Object);

            var httpContextAccessorMoq = new Mock<IHttpContextAccessor>();
            httpContextAccessorMoq.SetupGet(hcam => hcam.HttpContext).Returns(httpContextMoq.Object);

            var renderer = new CorrelationIdLayoutRenderer();
            renderer.HttpContextAccessor = httpContextAccessorMoq.Object;

            var stringBuilder = new StringBuilder();

            renderer.GetType().GetTypeInfo().GetDeclaredMethod("DoAppend").Invoke(renderer, new object[] { stringBuilder, null });

            Assert.Contains(guid.ToString(), stringBuilder.ToString());
        }
    }
}
