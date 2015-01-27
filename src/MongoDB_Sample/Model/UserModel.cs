namespace MongoDB_Sample.Model
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { set; get; }
        
        public string Name { set; get; }

        public int Age { set; get; }
    }
}