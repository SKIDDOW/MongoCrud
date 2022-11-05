# MongoCrud

MongoCrud is a class to manage MongoDB data collections in C# .NET

# Usage

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

Insert Employee data

```c#
string connectionString = "mongodb://localhost:27017";
string collectionName = "EmployeeDB";

```

```c#
using (MongoCrud.MongoCrud db = new MongoCrud.MongoCrud(connectionString, collectionName))
{
    var emp = new Employee()
    {
        Name = "Jone Doe",
        Birthday = Convert.ToDateTime("1981-04-13")
    };
    await db.InsertRecord("Employee", emp);
}

```




