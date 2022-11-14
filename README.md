![MongoCrud](https://raw.githubusercontent.com/skiddow/MongoCrud/main/assets/banner.jpg)
# MongoCrud ➕ 🔄️ ❌
MongoCrud is a simple c# class for MongoDB CRUD operations.

## Features
- Create, read, update and delete documents
    - Create unique records
    - Case-Insensitive search
    - Read by index/Id
    - Delete by index/Id

## Installation
- You can search for 'MongoCrud' in NuGet Pakage Manager in Visual Studio.
- Or you can use .NET CLI
    ```
    dotnet add package MongoCrud
    ```
---

## Usage

Employee Model

```c#
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public  class Employee
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }
}

```

### Open connection to MongoDB (using statement)

```c#
using MongoCrud;
```

```c#
string connectionString = "mongodb://localhost:27017";
string databaseName = "EmployeeDB";

using (Crud db = new Crud(connectionString, databaseName))
{
    // do your things here ...
}

```
---

### Insert Employee data

```c#
var emp = new Employee()
{
    Name = "Jone Doe",
    EmpID = 1000,
    Birthday = Convert.ToDateTime("1981-04-13")
};
await db.InsertRecord("Employee", emp);

```
---

### Insert unique record

```c#
var emp = new Employee()
{
    Name = "Jone Doe",
    EmpID = 1000, // Unique ID
    Birthday = Convert.ToDateTime("1981-04-13")
};
await db.InsertUniqRecord("Employee", emp, "EmpID");
```
---

### Load all records of a collection

```c#
var rec = await db.LoadRecords<Employee>("Employee");
```
---

### Load records by index

```c#
var rec = db.LoadRecordByIndex<Employee>("Employee", "Name", "Jone Doe");
```
---

### Load one record by index

```c#
var rec = db.LoadOneRecordByIndex<Employee>("Employee", "Name", "Jone Doe");
```
---

### Load a record by Id
First, we have to get ObjectId before doing this. To get an ObjectId you can use any method as shown in above.
```c#
ObjectId ObjectID = new ObjectId("6366675caf5305273398cfbd");
```

```c#
var rec = db.LoadRecordById<Employee>("Employee", ObjectID);
```
---

### Search case
Below example will display all records from Employee collection, which Name starts from 'J'
```c#
var rec = db.SearchCase<Employee>("Employee", "Name", "J");
```
---

### Delete all records by index
This will delete all records where EmpID is, '1000'. However, if EmpID is unique this will also delete a single record.
```c#
db.DeleteRecordByIndex<Employee>("Employee", "EmpID", "1000");
```
---

### Delete a record
To delete a record we have to get ObjectId
```c#
db.DeleteRecord<Employee>("Employee", ObjectID);
```
---

### Updating a record
First we need to load record, and then update.
```c#
var oneRec = db.LoadRecordById<Employee>("Employee", ObjectID);
oneRec.Id = ObjectID; // or, oneRec.Id = oneRec.Id;
oneRec.Name = "Jone Doe Smith"
```
This will update Employee Name.

```c#
db.UpsertRecord("Employee", oneRec.Id, oneRec);
```
---
## Bonus configuration for database connection.

Create a new class name, `dbConn`

```c#
namespace MongoCrud;
public class dbConn
{
    public static string connString = "mongodb://localhost:27017";
    public static string dbName = "EmployeeDB";
}
```
And then you can run CRUD operations, as below

```c#
using MongoCrud;

using (Crud db = new Crud(dbConn.connString, dbConn.dbName))
{
    // do your things here .... 
}
```
---
## A full example for insert

```c#
using MongoCrud;

public class MyProject
{
    public void InsertData()
    {
        using (Crud db = new Crud(dbConn.connString, dbConn.dbName))
        {
            var emp = new Employee()
            {
                Name = "Jone Doe",
                EmpID = 1000, // Unique ID
                Birthday = Convert.ToDateTime("1981-04-13")
            };
            await db.InsertUniqRecord("Employee", emp, "EmpID");
        }
    }
}
```

# ❤️😍
