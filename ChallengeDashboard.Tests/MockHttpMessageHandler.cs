using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard.Tests;

internal class MockHttpMessageHandler : HttpMessageHandler
{
	private readonly string _data;

	public MockHttpMessageHandler(string data)
	{
		_data = data;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var content = new StringContent(_data);
		var message = new HttpResponseMessage { Content = content };
		return Task.FromResult(message);
	}
}
