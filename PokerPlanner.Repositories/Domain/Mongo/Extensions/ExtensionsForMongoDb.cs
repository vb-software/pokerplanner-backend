using MongoDB.Driver;
using PokerPlanner.Entities.Domain.Mongo.Attributes;

namespace PokerPlanner.Repositories.Domain.Mongo.Extensions
{
    public static class ExtensionsForMongoDb
    {
        public static IMongoCollection<T> GetGlobalCollection<T>(this IMongoDatabase db)
        {
            return db.GetCollection<T>(MongoDocumentAttribute.GetDocumentName<T>());
        }

        public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase db, string collectionName)
        {
            return db.GetCollection<T>(collectionName);
        }
    }
}