using CryptoPulse.Database.DTO;
using CryptoPulse.Database.Service.Interfaces;
using CryptoPulse.Database.SQLLite;
using SQLite;

namespace CryptoPulse.Database.Service;

// All the code in this file is included in all platforms.
public class DatabaseSQLLiteService : IDatabaseClientService
{
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
	}

	public async Task InitializeAsync()
	{

		DatabaseMigrations databaseMigrations = new DatabaseMigrations(_database);
		await databaseMigrations.ApplyPendingMigrations();  // Await the migration process asynchronously
	}

	public async Task<List<CryptocurrencyPairDto>> GetPairsAsync()
	{
		try
		{

			return await _database.Table<CryptocurrencyPairDto>().ToListAsync();

		}
		catch (Exception ex)
		{
			throw new Exception("");
		}
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
