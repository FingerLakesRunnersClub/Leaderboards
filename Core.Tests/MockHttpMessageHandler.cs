namespace FLRC.Leaderboards.Core.Tests;

internal class MockHttpMessageHandler : HttpMessageHandler
{
	public HttpRequestMessage Requested { get; private set; }

	private readonly string _data;

	public MockHttpMessageHandler(string data)
	{
		_data = data;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		Requested = request;
		var content = new StringContent(_data);
		var message = new HttpResponseMessage { Content = content };
		return Task.FromResult(message);
	}
}