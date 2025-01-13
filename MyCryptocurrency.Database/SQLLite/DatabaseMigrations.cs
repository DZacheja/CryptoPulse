using MyCryptocurrency.Database.DTO;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.Database.SQLLite;
public class DatabaseMigrations
{
	private readonly int LATEST_DATABASE_VERSION = 2;
	private readonly SQLiteAsyncConnection _database;

	public DatabaseMigrations(SQLiteAsyncConnection db)
	{
			_database = db;
	}
	public async void ApplyPendingMigrations()
	{
			// Retrieve the current database version
			var currentVersion = await _database.ExecuteScalarAsync<int>("PRAGMA user_version");

			// Apply incremental upgrades until the database is up-to-date
			while (currentVersion < LATEST_DATABASE_VERSION)
			{
				currentVersion++;
				switch (currentVersion)
				{
					case 1:
					await InsertCryptocurrencyPairDtoTable();
					break;
					case 2:
					await RemoveUniqueConstraintFromOrderIDAsync();
					break;
					// Add cases for future versions as needed
				}
			}

			// Update the database version to the latest
			await _database.ExecuteAsync($"PRAGMA user_version = {LATEST_DATABASE_VERSION}");
	}

	public async Task InsertCryptocurrencyPairDtoTable()
	{
		await _database.CreateTableAsync<CryptocurrencyPairDto>();
	}

	public async Task RemoveUniqueConstraintFromOrderIDAsync()
	{
		// Begin a transaction
		await _database.RunInTransactionAsync(async transaction =>
		{
			try
			{
				// Rename the existing table
				await _database.ExecuteAsync("ALTER TABLE CryptocurrencyPairDto RENAME TO CryptocurrencyPairDto_backup;");

				// Create the new table without the UNIQUE constraint
				await _database.ExecuteAsync(@"
                CREATE TABLE CryptocurrencyPairDto (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CurrencyName1 TEXT,
                    CurrencyName2 TEXT,
                    OrderID INTEGER
                );
            ");

				// Copy data from the backup table to the new table
				await _database.ExecuteAsync(@"
                INSERT INTO CryptocurrencyPairDto (Id, CurrencyName1, CurrencyName2, OrderID)
                SELECT Id, CurrencyName1, CurrencyName2, OrderID
                FROM CryptocurrencyPairDto_backup;
            ");

				// Commit the transaction
				transaction.Commit();
			}
			catch (Exception ex)
			{
				// Rollback the transaction in case of an error
				transaction.Rollback();
				throw new Exception("An error occurred while removing the UNIQUE constraint from OrderID.", ex);
			}
			finally
			{
				// Drop the backup table if it exists
				await _database.ExecuteAsync("DROP TABLE IF EXISTS CryptocurrencyPairDto_backup;");
			}
		});
	}
}
