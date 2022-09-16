using MongoDB.Bson.Serialization.Attributes;

namespace Business.Persistence
{
    public record Quote(string Hash, string Content, int Score)
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}