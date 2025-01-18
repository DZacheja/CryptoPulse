using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Infrastructure.Services.Interfaces;
public interface ISecureStorageService
{
	Task SaveAsync(string key, string value);
	Task<string?> GetAsync(string key);
	void Remove(string key);
	Task SaveApiKeyAsync(string value);
	Task<string?> GetApiKeyAsync();
	Task SaveApiPrivateKeyAsync(string value);
	Task<string?> GetApiPrivateKeyAsync();
	Task SaveApiKeyAndPriveteKeyAsync(string apiKey, string privateKey);
	public string? GetApiKeyCashed();
	public string? GetApiPrivateKeyCashed();
}
