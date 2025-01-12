using Microsoft.Maui.Storage;
using MyCryptocurrency.Infrastructure.Services.Interfaces;

namespace MyCryptocurrency.Infrastructure.Services;

public class SecureStorageService : ISecureStorageService
{
	private readonly string _apiKeyName = "MYCRYPTOCURRENCYAPIKEY";
	private readonly string _apiPrvateKeyName = "MYCRYPTOCURRENCYAPIPRIVATEKEY";
	// Save data securely
	public async Task SaveAsync(string key, string value)
	{
		await SecureStorage.SetAsync(key, value);
	}

	// Get data securely
	public async Task<string> GetAsync(string key)
	{
		return await SecureStorage.GetAsync(key);
	}

	// Remove data securely
	public async Task RemoveAsync(string key)
	{
		SecureStorage.Remove(key);
	}

	public async Task SaveApiKeyAsync(string value)
	{
		await SecureStorage.SetAsync(_apiKeyName, value);
	}

	public async Task<string> GetApiKeyAsync()
	{
		try
		{
			return await SecureStorage.GetAsync(_apiKeyName);
		}
		catch (Exception ex)
		{
			return null;
		}
	}

	public async Task SaveApiPrivateKeyAsync(string value)
	{
		await SecureStorage.SetAsync(_apiPrvateKeyName, value);
	}

	public async Task<string> GetApiPrivateKeyAsync()
	{
		try
		{
			return await SecureStorage.GetAsync(_apiPrvateKeyName);
		}
		catch (Exception ex)
		{
			return null;
		}
	}
}
