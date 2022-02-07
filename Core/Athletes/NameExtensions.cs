using System.Security.Cryptography;
using System.Text;

namespace FLRC.Leaderboards.Core.Athletes;

public static class NameExtensions
{
	public static uint GetID(this string name)
		=> BitConverter.ToUInt32(MD5.HashData(Encoding.UTF8.GetBytes(name)));
}