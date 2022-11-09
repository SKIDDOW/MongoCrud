![MongoCrud](https://raw.githubusercontent.com/skiddow/MongoCrud/main/assets/banner.jpg)
# MongoCrud
Please note that this doc is under development.

MongoCrud is a class to manage MongoDB data collections in C# .NET

## Installation
- You can search for 'MongoCrud' in NuGet Pakage Manager in Visual Studio.
- Or you can use .NET CLI
```
dotnet add package MongoCrud
```

## Usage

Employee Model

```c#

public  class Employee
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }
}

```

### Insert Employee data

```c#
using MongoCrud;

string connectionString = "mongodb://localhost:27017";
string collectionName = "EmployeeDB";

```

```c#
using (Crud db = new Crud(connectionString, collectionName))
{
    var emp = new Employee()
    {
        Name = "Jone Doe",
        EmpID = 1000,
        Birthday = Convert.ToDateTime("1981-04-13")
    };
    await db.InsertRecord("Employee", emp);
}

```

### Insert unique recored

```c#
using (Crud db = new Crud(connectionString, collectionName))
{
    var emp = new Employee()
    {
        Name = "Jone Doe",
        EmpID = 1000, // Unique ID
        Birthday = Convert.ToDateTime("1981-04-13")
    };
    await db.InsertUniqRecord("Employee", emp, "EmpID");
}
```

### Load all records of a collection

```c#
var rec = db.LoadRecords<Employee>("Employee");
```

### Load records by index

```c#
var rec = db.LoadRecordByIndex<Employee>("Employee", "Name", "Jone Doe");
```

### Load one record by index

```c#
var rec = db.LoadOneRecordByIndex<Employee>("Employee", "Name", "Jone Doe");
```

### Load a record by Id
First, we have to get ObjectId before doing this. To get an ObjectId you can use any method as shown in above.
```c#
ObjectId ObjectID = new ObjectId("6366675caf5305273398cfbd");
```

```c#
var rec = db.LoadRecordById<Employee>("Employee", ObjectID);
```