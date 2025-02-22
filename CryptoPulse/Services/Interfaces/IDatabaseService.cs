﻿using CryptoPulse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Services.Interfaces;
public interface IDatabaseService
{
	public Task<List<CryptocurrencyPair>> GetPairsAsync();

	public Task<int> AddPairAsync(CryptocurrencyPair pair);

	public Task<int> DeletePairAsync(CryptocurrencyPair pair);
}
