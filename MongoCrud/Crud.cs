﻿using System;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Crud;

/// <summary>
/// This is a simple class for MongoDB crud operations.
/// </summary>
public class Crud : IDisposable
{
    // Doc at : https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    // To detect redundant calls
    private bool _disposedValue;

    ~Crud() => Dispose(false);

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


    public Crud(string connectionString, string DBName)
    {
        ConnectionString = connectionString;
        DatabaseName = DBName;
    }

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public Task InsertRecord<T>(string collection, T record)
    {
        var coll = ConnectToMongo<T>(collection);
        return coll.InsertOneAsync(record);
    }


    public async Task InsertUniqRecord<T>(string collection, T record, string unique_index)
    {
        var coll = ConnectToMongo<T>(collection);
        await coll.InsertOneAsync(record);

        // Create the unique index on the field 'unique_index'
        var options = new CreateIndexOptions { Unique = true };
        await coll.Indexes.CreateOneAsync("{ '" + unique_index + "' : 1 }", options);
    }        

    public async Task<List<T>> LoadRecords<T>(string collection)
    {
        var coll = ConnectToMongo<T>(collection);
        var result = await coll.FindAsync(_ => true);
        return result.ToList();
    }


    /// <summary>
    /// <para>
    /// List records by an index. This method will replace with SearchAsync.
    /// </para> 
    /// <para>
    /// ⚠️ This method will be replace with SearchAsync in v2.0
    /// </para>
    /// <para>
    /// So, you should better using SearchAsync than LoadRecordByIndex
    /// </para>
    /// </summary>
    public List<T> LoadRecordByIndex<T>(string collection, string field, string value)
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Eq(field, value);
        return coll.Find(filter).ToList();
    }

    // Search async with case in-sensitive
    /// <summary>
    /// This method is case in-sensitive
    /// </summary>
    public async Task<List<T>> SearchAsync<T>(string collection, string field, string value)
    {
        var queryExpr = new BsonRegularExpression(new Regex(value, RegexOptions.IgnoreCase));
        var builder = Builders<T>.Filter;
        var coll = ConnectToMongo<T>(collection);
        var filter = builder.Regex(field, queryExpr);
        var result = await coll.FindAsync(filter);
        return result.ToList();
    }


    public T LoadOneRecordByIndex<T>(string collection, string field, string value)
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Eq(field, value);            
        return coll.Find(filter).FirstOrDefault();
    }

    /// <summary>
    /// This method is case in-sensitive
    /// </summary>
    public async Task<T> LoadOneRecordByIndexAsync<T>(string collection, string field, string value)
    {
        var queryExpr = new BsonRegularExpression(new Regex(value, RegexOptions.IgnoreCase));
        var builder = Builders<T>.Filter;
        var coll = ConnectToMongo<T>(collection);
        var filter = builder.Regex(field, queryExpr);
        var result = await coll.FindAsync(filter);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// This method can be replace with LoadRecordByIdAsync
    /// <para>
    /// ⚠️ This method will be replace with LoadRecordByIdAsync in v2.0
    /// </para>
    /// </summary>
    public T LoadRecordById<T>(string collection, ObjectId id)
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Eq("Id", id); // list only where id =
        return coll.Find(filter).First();
    }

    public async Task<T> LoadRecordByIdAsync<T>(string collection, ObjectId id)
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Eq("Id", id); // list only where id =
        var result = await coll.FindAsync(filter);
        return result.First();
    }

    /// <summary>
    /// <para>
    /// ⚠️ This method will be replace with SearchCaseAsync in v2.0
    /// </para>
    /// </summary>
    public List<T> SearchCase<T>(string collection, string field, string value)
    {
        var queryExpr = new BsonRegularExpression(new Regex(value, RegexOptions.None));
        var builder = Builders<T>.Filter;
        var coll = ConnectToMongo<T>(collection);
        var filter = builder.Regex(field, queryExpr);
        return coll.Find(filter).ToList();
    }

    public async Task<List<T>> SearchCaseAsync<T>(string collection, string field, string value)
    {
        var queryExpr = new BsonRegularExpression(new Regex(value, RegexOptions.None));
        var builder = Builders<T>.Filter;
        var coll = ConnectToMongo<T>(collection);
        var filter = builder.Regex(field, queryExpr);
        var result = await coll.FindAsync(filter);
        return result.ToList();
    }


    public List<T> LoadRecordByDate<T>(string collection, string field, DateTime datetime)
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Eq(field, datetime);
        return coll.Find(filter).ToList();
    }
           
    public async Task<List<T>> LoadBetweenDates<T>(string collection, string date_field, DateTime fromDate, DateTime toDate)
    {
        var coll = ConnectToMongo<T>(collection);

        // Gte  = greater than or equal 
        // Gt  = greater than 
        // Lte  = less than or equal
        // Lt  = less than

        //var from_filter = Builders<T>.Filter.Gte(date_field, fromDate);
        //var to_filter = Builders<T>.Filter.Lt(date_field, toDate);

        var filter = Builders<T>.Filter.Gte(date_field, fromDate) & Builders<T>.Filter.Lt(date_field, toDate);
        //var filter = Builders<T>.Filter.And(new[] { from_filter, to_filter });
        var result = await coll.FindAsync(filter);
        return result.ToList();
    }


    public async Task<List<T>> GreaterThanNumber<T>(string collection, string field, double number)
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Gt(field, number);
        var result = await coll.FindAsync(filter);
        return result.ToList();
    }

    public async void DeleteRecordByIndex<T>(string collection, string field, string value) // Delete record by a value.
    {
        var coll = ConnectToMongo<T>(collection);
        var filter = Builders<T>.Filter.Eq(field, value);
        await coll.DeleteOneAsync(filter);
    }

    public void DeleteRecord<T>(string table, ObjectId id) // Delete record by id.
    {
        var collection = ConnectToMongo<T>(table);
        var filter = Builders<T>.Filter.Eq("Id", id);
        collection.DeleteOne(filter);
    }

    public void UpsertRecord<T>(string collection, ObjectId id, T record) // Update data or if it isnt available create it
    {
        var coll = ConnectToMongo<T>(collection);
        // 'IMongoCollection<T>.ReplaceOne(FilterDefinition<T>, T, UpdateOptions, CancellationToken)' is obsolete: 'Use the overload that takes a ReplaceOptions instead of an UpdateOptions.'
        var result = coll.ReplaceOne(new BsonDocument("_id", id), record,
        new UpdateOptions { IsUpsert = true });
        // 'IMongoCollection<T>.ReplaceOne(FilterDefinition<T>, T, UpdateOptions, CancellationToken)' is obsolete: 'Use the overload that takes a ReplaceOptions instead of an UpdateOptions.'
    }

    public async Task UpsertRecordAsync<T>(string collection, ObjectId id, T record)
    {
        var coll = ConnectToMongo<T>(collection);
        var result = await coll.ReplaceOneAsync(new BsonDocument("_id", id), record, new ReplaceOptions { IsUpsert = true });
    }

}