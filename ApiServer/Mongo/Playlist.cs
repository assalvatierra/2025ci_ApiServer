using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ApiServer.Mongo;

public class Playlist {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;
    public List<string> MovieIds { get; set; } = new List<string>();
}
