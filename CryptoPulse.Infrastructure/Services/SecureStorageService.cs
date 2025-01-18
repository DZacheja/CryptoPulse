using Microsoft.Maui.Storage;
using CryptoPulse.Infrastructure.Services.Interfaces;

namespace CryptoPulse.Infrastructure.Services;

public class SecureStorageService : ISecureStorageService
{
	private const string? _apiKeyName = "CryptoPulseAPIKEY";
	private const string? _apiPrvateKeyName = "CryptoPulseAPIPRIVATEKEY";
	private string? _apiKeyValue {get; set;} = "";
	private string? _apiPrvateKeyValue { get; set; } = "";
	// Save data securely
	public async Task SaveAsync(string key, string value)
	{
		await SecureStorage.SetAsync(key, value);
	}

	// Get data securely
	public async Task<string?> GetAsync(string key)
	{
		return await SecureStorage.GetAsync(key);
	}

	public void Remove(string key)
	{
		SecureStorage.Remove(key);
	}

	public async Task SaveApiKeyAsync(string value)
	{
		await SecureStorage.SetAsync(_apiKeyName, value);
		_apiKeyValue = value;
	}

	public async Task<string?> GetApiKeyAsync()
	{
		try
		{
			_apiKeyValue = await SecureStorage.GetAsync(_apiKeyName);
			return _apiKeyValue;
		}
		catch (Exception)
		{
			throw new Exception("Error occured trying to get your API key.");
		}
	}

	public async Task SaveApiPrivateKeyAsync(string value)
	{
		await SecureStorage.SetAsync(_apiPrvateKeyName, value);
		_apiPrvateKeyValue = value;
	}

	public async Task<string?> GetApiPrivateKeyAsync()
	{
		try
		{
			_apiPrvateKeyValue = await SecureStorage.GetAsync(_apiPrvateKeyName);
			return _apiPrvateKeyValue;
		}
		catch (Exception)
		{
			throw new Exception("Error occured trying to get your Private key.");
		}
	}

	public async Task SaveApiKeyAndPriveteKeyAsync(string apiKey, string privateKey)
	{
		await SaveApiKeyAsync(apiKey);
		await SaveApiPrivateKeyAsync(privateKey);
	}

	public string GetApiKeyCashed()
	{
		return _apiKeyValue!;
	}

	public string GetApiPrivateKeyCashed()
	{
		return _apiPrvateKeyValue!;
	}
}
