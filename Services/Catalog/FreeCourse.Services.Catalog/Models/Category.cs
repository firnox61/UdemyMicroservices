using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    internal class Category
    {
       // IOptions<DatabaseSett>
        [BsonId]//ıd olaark mongodb de tanımlansın diye
        [BsonRepresentation(BsonType.ObjectId)]//tipi
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
