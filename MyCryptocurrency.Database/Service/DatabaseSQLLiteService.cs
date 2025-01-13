using MyCryptocurrency.Database.DTO;
using MyCryptocurrency.Database.Service.Interfaces;
using MyCryptocurrency.Database.SQLLite;
using SQLite;

namespace MyCryptocurrency.Database.Service;

// All the code in this file is included in all platforms.
public class DatabaseSQLLiteService: IDatabaseClientService
{
	private readonly int LATEST_DATABASE_VERSION = 1;
	private SQLiteAsyncConnection _database;
	public const string DatabaseFilename = "Cryptocurrency.db3";

	public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

	public static string DatabasePath =>
		Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

	public DatabaseSQLLiteService()
	{
		if (_database is not null)
			return;

		_database = new SQLiteAsyncConnection(DatabasePath, Flags);

		DatabaseMigrations databaseMigrations = new DatabaseMigrations(_database);
		databaseMigrations.ApplyPendingMigrations();
	}

	public async Task<List<CryptocurrencyPairDto>> GetPairsAsync()
	{
		return await _database.Table<CryptocurrencyPairDto>().ToListAsync();
	}

	public async Task<int> SavePairAsync(CryptocurrencyPairDto pair)
	{
		pair.CurrencyName1 = pair.CurrencyName1.ToUpper();
		pair.CurrencyName2 = pair.CurrencyName2.ToUpper();
		if (pair.Id != 0)
			return await _database.UpdateAsync(pair);
		else
			return await _database.InsertAsync(pair);
	}

	public async Task<int> DeletePairAsync(CryptocurrencyPairDto pair)
	{
		return await _database.DeleteAsync(pair);
	}

}
