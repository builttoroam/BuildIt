# SQLiteWrapper
Wrapper library to make creating and working with Sqlite database files from applications easier

# Usage

As the SQLiteWrapper under the hood uses [SQLite.Net-PCL library](https://github.com/oysteinkrog/SQLite.Net-PCL) you should probably start from reading a bit about it and then start going through what's written below. Also I wrote a ["walk through" blog post](http://progrunning.net/sqlite-wrapper-for-windows-10-uwp-applications/) about this wrapper, which you might find useful

Last but not least you should definitely check out the samples directory that we have in the Git repository

# Database Services

## Base

`BaseDatabaseService` is an abstract class with two key methods in it `CreateSQLiteConnection` and `CreateDatabaseTables`. First one is responsible for creating SQLite db connection. Second one, `CreateDatabaseTables`, is responsible for creating tables in the database after the SQLite connection has been established. Those two are abstract, hence are yours responsibility to write their implementation. What is more, this service consists of some basic database `Entites` manipulation methods (e.g. `Get` or `InsertOrUpdate`).

## Basic

`BasicDatabaseService` is a basic, exemplary, implementation of a database service, which inherits from `BaseDatabaseService`. It is there to give you an idea how the implementation of `CreateSQLiteConnection` method could look like. Besides that it can be easily used as a base class for your database service (as shown in SQLiteWrapperUWP_PCL_MvvmCross_CRUD sample project).

Constructor of `BasicDatabaseService` takes three interfaces `ISqlitePlatformProvider`, `IDatabaseNameProvider` and `IFileService` as parameters. All of them are necessary to create and establish SQLite database connection. Implementation of first one could be done with  [SQLite.Net-PCL library](https://github.com/oysteinkrog/SQLite.Net-PCL) and might look like this

	public class SqlitePlatformProvider : ISqlitePlatformProvider
    {
        public ISQLitePlatform SqLitePlatform { get; set; }

        public SqlitePlatformProvider()
        {
            SqLitePlatform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        }
    }

When it comes to the second one `IDatabaseNameProvider` the situation is very simple. It is the place where you specify your database name

	public class DatabaseNameProvider : IDatabaseNameProvider
    {
        public string DatabseName { get; set; } = Constants.DatabaseName; // <some_name>.db
    }

Lastly `IFileService`, which implementation could look like this

	public class LocalFileService : IFileService
    {
        public async Task<string> RetrieveNativePath(string filePath)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
            return file.Path;
        }
    }

## Entities

All your database entities should derive from `BaseEntity` class. This base class has already an `Id` property in it and it will be the public key of database table created from this entity

	public class PersonEntity : BaseEntity<PersonEntity>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }

As you have probably noticed `BaseEntity` is a self-reference generic class. It was designed like this so that one could write an `UpdateFromEntity` logic inside of the `Entity` class. What it means is basically that you can keep your `Entity` update logic inside of the class and decide what records in the database will be updated / changed and what conditions have to be fulfilled so those changes could be applied. `UpdateFromEntity` whenever  `BaseRepository` method `InsertOrUpdate` is being called.

	public class PersonEntity : BaseEntity<PersonEntity>
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public override bool UpdateFromEntity(PersonEntity entityToBeUpdated, PersonEntity entitySource)
        {
            // Checks if the ID's of those two entites are the same
            if (base.UpdateFromEntity(entityToBeUpdated, entitySource))
            {
                entityToBeUpdated.Name = entitySource.Name;
                entityToBeUpdated.Surname = entitySource.Surname;

                // Update changed values
                return true;
            }

            // Do not update
            return false;
        }
    }

## Repositories

`BaseRepository` holds all the logic that sits behind CRUD operations for database tables. Its usage is very easy. You create a repository for your entity `BaseRepository<TEntity>`, which represents a database table and allows execution of basic operation e.g. `Get`, `Insert`, `Update` and `Delete`.  

	var newPerson = new PersonEntity()
    {
    	Name = "Nick",
        Surname = "Cage"
    };
	using (var personRepo = new BaseRepository<PersonEntity>(dbConnection))
    {
    	personRepo.Insert(person);
	}
	
or 

	using (var personRepo = new BaseRepository<PersonEntity>(dbConnection))
	{
		return personRepo.Table.ToList();
	}
	
NOTE: `BaseRepository` class implements `IDisposable` interface so you should use `using` statement when operating on it
