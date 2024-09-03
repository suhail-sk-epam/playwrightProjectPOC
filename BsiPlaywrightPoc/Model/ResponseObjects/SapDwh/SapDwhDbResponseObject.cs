using System.Text.Json.Serialization;

namespace BsiPlaywrightPoc.Model.ResponseObjects.SapDwh
{
    public class SapDwhDbResponseObject
    {
        [JsonPropertyName("sapItemsData")]
        public List<SapItemsDatum> SapItemsData { get; set; }

        [JsonPropertyName("orderStatus")]
        public string OrderStatus { get; set; }
    }
}
