

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Test;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string UserRole { get; set; }
}
