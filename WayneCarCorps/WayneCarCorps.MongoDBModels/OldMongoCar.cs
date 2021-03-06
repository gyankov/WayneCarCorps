﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WayneCarCorps.MongoDBModels.Interfaces;

namespace WayneCarCorps.MongoDBModels
{
    public class OldMongoCar : IMongoModel
    {
        public OldMongoCar(int modelId, int year, decimal price, int power, int colourId, int dealerId)
        {
            this.Id = ObjectId.GenerateNewId().ToString();
            this.ModelId = modelId;
            this.Year = year;
            this.Price = price;
            this.Power = power;
            this.ColourId = colourId;
            this.DealerId = dealerId;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("model")]
        public int ModelId { get; set; }

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("power")]
        public int Power { get; set; }

        [BsonElement("colourId")]
        public int ColourId { get; set; }

        [BsonElement("dealerId")]
        public int DealerId { get; set; }
    }
}
