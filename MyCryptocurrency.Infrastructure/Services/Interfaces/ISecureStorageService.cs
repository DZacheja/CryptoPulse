using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.Infrastructure.Services.Interfaces;
public interface ISecureStorageService
{
	Task SaveAsync(string key, string value);
	Task<string> GetAsync(string key);
	Task RemoveAsync(string key);
	Task SaveApiKeyAsync(string value);
	Task<string> GetApiKeyAsync();
	Task SaveApiPrivateKeyAsync(string value);
	Task<string> GetApiPrivateKeyAsync();
}
