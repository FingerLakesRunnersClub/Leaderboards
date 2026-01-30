using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FLRC.Leaderboards.Core.Auth;

public sealed class DiscourseAuthenticator : IDiscourseAuthenticator
{
	private readonly string _baseURL;
	private readonly byte[] _secret;

	public DiscourseAuthenticator(string baseURL, string secret)
	{
		_baseURL = baseURL;
		_secret = Encoding.UTF8.GetBytes(secret);
	}

	public string GetLoginURL(string currentHost)
	{
		var returnURL = $"{currentHost}/Account/Redirect";
		var nonce = Guid.NewGuid();

		var payload = $"nonce={nonce}&return_sso_url={returnURL}";
		var payloadBytes = Encoding.UTF8.GetBytes(payload);
		var payloadBase64 = Convert.ToBase64String(payloadBytes);
		var base64Bytes = Encoding.UTF8.GetBytes(payloadBase64);
		var hash = HMACSHA256.HashData(_secret, base64Bytes);

		var sso = WebUtility.UrlEncode(payloadBase64);
		var sig = Convert.ToHexString(hash).ToLower();

		return $"{_baseURL}/session/sso_provider?sso={sso}&sig={sig}";
	}

	public bool IsValidResponse(string sso, string sig)
	{
		var ssoBytes = Encoding.UTF8.GetBytes(sso);
		var ssoHash = HMACSHA256.HashData(_secret, ssoBytes);
		var sigHash = Convert.FromHexString(sig);
		return ssoHash.SequenceEqual(sigHash);
	}

	public IDictionary<string, string> ParseResponse(string sso)
	{
		var data = GetDataResponse(sso);
		return Dictionary(data);
	}

	private static NameValueCollection GetDataResponse(string sso)
	{
		var ssoBase64 = Convert.FromBase64String(sso);
		var response = Encoding.UTF8.GetString(ssoBase64);
		return HttpUtility.ParseQueryString(response);
	}

	private static Dictionary<string, string> Dictionary(NameValueCollection data)
		=> data.AllKeys
			.Where(key => key is not null && data[key] is not null)
			.ToDictionary(key => key!, key => data[key]!);
}