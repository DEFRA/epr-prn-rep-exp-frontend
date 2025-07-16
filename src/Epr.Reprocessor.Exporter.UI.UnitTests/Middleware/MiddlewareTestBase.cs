using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Middleware;

public abstract class MiddlewareTestBase
{
    protected Mock<RequestDelegate> MockNext { get; private set; } = null!;
    protected DefaultHttpContext MockHttpContext { get; private set; } = null!;

    [TestInitialize]
    public void SetupBase()
    {
        // Setup mock middleware delegate
        MockNext = new Mock<RequestDelegate>();
        MockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Setup HttpContext with streamable response body
        MockHttpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    protected void SetupCookies(Dictionary<string, string> cookies)
    {
        // Setup mock cookies in HttpContext
        MockHttpContext.Request.Cookies = new MockRequestCookieCollection(cookies);
    }

    // Helper: CookieCollection mock
    private class MockRequestCookieCollection(Dictionary<string, string> cookies) : IRequestCookieCollection
    {
        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            throw new NotImplementedException();
        }

        public int Count => cookies.Count;
        public ICollection<string> Keys => cookies.Keys;
        public bool ContainsKey(string key) => cookies.ContainsKey(key);
        public string this[string key] => cookies[key];
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => cookies.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => cookies.GetEnumerator();
    }
}