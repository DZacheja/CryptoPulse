using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse;
public static class AppSettings
{
	public static bool ShowPageForTest { get; set; } = true; // Change to false in production
	public static bool ApiKeys { get; set; } = true;
	public const string ApiKeyName = "CryptoPulseAPIKEY";
	public const string ApiPrvateKeyName = "CryptoPulseAPIPRIVATEKEY";
	public static bool IsValidKeys { get; set; } = false;
}

