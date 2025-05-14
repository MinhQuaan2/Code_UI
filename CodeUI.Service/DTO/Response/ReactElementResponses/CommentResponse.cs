using CodeUI.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReactElementResponse
{
    public class CommentResponse
    {
        public int Id;
        public string? CommentContent { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? Timestamp { get; set; }
        public List<CommentResponse> InverseRootComment { get; set; }
        public AccountReturn? Account { get; set; }
    }
    public class AccountReturn
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public ProfileReturn? Profile { get; set; }
    }
    public class ProfileReturn
    {
        public string? ImageUrl { get; set; }
    }
}
public class CustomDateTimeConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is DateTime dateTime)
        {
            // Format the DateTime as a string with the desired format
            writer.WriteValue(dateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // Implement this if needed
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }
}