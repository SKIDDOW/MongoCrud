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

using (MongoCrud db = new MongoCrud("mongodb://localhost:27017", "EmployeeDB"))
{
    var emp = new Employee()
    {
        Name = NameComboBox.Text,
        Birthday = Convert.ToDateTime("1981-04-13")
    };
    await db.Insert(emp, "Employee"); 
}

```


