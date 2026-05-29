namespace FLRC.Leaderboards.Core.Tests;

public class MockHttpMessageHandler(string data) : HttpMessageHandler
{
	public HttpRequestMessage LastRequested => Requests.LastOrDefault();
	public List<HttpRequestMessage> Requests { get; } = [];


	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		Requests.Add(request);
		var content = new StringContent(data);
		var message = new HttpResponseMessage { Content = content };
		return Task.FromResult(message);
	}
}