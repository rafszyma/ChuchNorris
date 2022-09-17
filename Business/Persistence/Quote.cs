using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Business.Persistence
{
    public record Quote(string Key, string Content, int Score)
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }
    }
    
    public class QuoteHashComparer : EqualityComparer<Quote>
    {
        public override bool Equals(Quote? x, Quote? y)
        {
            return x is not null && y is not null && x.Key.Equals(y.Key);
        }

        public override int GetHashCode(Quote obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}