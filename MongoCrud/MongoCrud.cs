using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCrud
{
    public class MongoCrud : IDisposable
    {
        // Doc at : https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        // To detect redundant calls
        private bool _disposedValue;

        ~MongoCrud() => Dispose(false);

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        //private const string ConnectionString = "mongodb://localhost:27017";
        //private const string DatabaseName = "mongoDBSample";

        private string ConnectionString;
        private string DatabaseName;

        public MongoCrud(string connectionString, string DBName)
        {
            ConnectionString = connectionString;
            DatabaseName = DBName;
        }

        public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DatabaseName);
            return db.GetCollection<T>(collection);
        }

        public Task InsertRecord<T>(string table, T record)
        {
            var collection = ConnectToMongo<T>(table);
            return collection.InsertOneAsync(record);
        }


        public async Task InsertUniqRecord<T>(string table, T record, string unique_index)
        {
            var collection = ConnectToMongo<T>(table);
            await collection.InsertOneAsync(record);

            // Create the unique index on the field 'unique_index'
            var options = new CreateIndexOptions { Unique = true };
            await collection.Indexes.CreateOneAsync("{ '" + unique_index + "' : 1 }", options);

        }

        

        public async Task<List<T>> LoadRecords<T>(string collection_name)
        {
            var collection = ConnectToMongo<T>(collection_name);
            var result = await collection.FindAsync(_ => true);
            return result.ToList();
        }


        //List records by an index. =====================================
        public List<T> LoadRecordByIndex<T>(string table, string field, string value)
        {
            var collection = ConnectToMongo<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return collection.Find(filter).ToList();
        }


        public T LoadOneRecordByIndex<T>(string table, string field, string value)
        {
            var collection = ConnectToMongo<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return collection.Find(filter).First();
        }


        public T LoadRecordById<T>(string table, ObjectId id)
        {
            var collection = ConnectToMongo<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id); // list only where id =
            return collection.Find(filter).First();
        }

        public List<T> SearchCase<T>(string table, string field, string value)
        {
            var queryExpr = new BsonRegularExpression(new Regex(value, RegexOptions.None));
            var builder = Builders<T>.Filter;
            var collection = ConnectToMongo<T>(table);
            var filter = builder.Regex(field, queryExpr);
            return collection.Find(filter).ToList();
        }


        public List<T> LoadRecordByDate<T>(string table, string field, DateTime datetime)
        {
            var collection = ConnectToMongo<T>(table);
            var filter = Builders<T>.Filter.Eq(field, datetime);
            return collection.Find(filter).ToList();
        }

        

        public async void DeleteRecordByIndex<T>(string table, string field, string value) // Delete record by a value.
        {
            var collection = ConnectToMongo<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            await collection.DeleteOneAsync(filter);
        }

        public void UpsertRecord<T>(string table, ObjectId id, T record) // Update data or if it isnt available create it
        {
            var collection = ConnectToMongo<T>(table);

#pragma warning disable CS0618 // 'IMongoCollection<T>.ReplaceOne(FilterDefinition<T>, T, UpdateOptions, CancellationToken)' is obsolete: 'Use the overload that takes a ReplaceOptions instead of an UpdateOptions.'
            var result = collection.ReplaceOne(new BsonDocument("_id", id), record,
                new UpdateOptions { IsUpsert = true });
#pragma warning restore CS0618 // 'IMongoCollection<T>.ReplaceOne(FilterDefinition<T>, T, UpdateOptions, CancellationToken)' is obsolete: 'Use the overload that takes a ReplaceOptions instead of an UpdateOptions.'
        }
    }
}