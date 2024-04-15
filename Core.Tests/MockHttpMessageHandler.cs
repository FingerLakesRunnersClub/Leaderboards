namespace FLRC.Leaderboards.Core.Tests;

internal class MockHttpMessageHandler : HttpMessageHandler
{
	public HttpRequestMessage LastRequested => Requests.LastOrDefault();
	public IList<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

	private readonly string _data;


	public MockHttpMessageHandler(string data)
	{
		_data = data;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		Requests.Add(request);
		var content = new StringContent(_data);
		var message = new HttpResponseMessage { Content = content };
		return Task.FromResult(message);
	}
}