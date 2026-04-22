using System.Text.Json.Serialization;

namespace GLMS_ST10445830.Services
{

    /*the purpose of this class is to represent the structure of the API response,
     * this class then deserializes the json from the api and allows the rest of
     * code to perform the conversion
     * 
    */
    public class ExchangeRateApiResponse
    {
        //the fields are mapped to a property in the json response from the api, this allows for easy deserialization
        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;

        [JsonPropertyName("documentation")]
        public string Documentation { get; set; } = string.Empty;

        [JsonPropertyName("terms_of_use")]
        public string TermsOfUse { get; set; } = string.Empty;

        [JsonPropertyName("time_last_update_unix")]
        public long TimeLastUpdateUnix { get; set; }

        [JsonPropertyName("time_last_update_utc")]
        public string TimeLastUpdateUtc { get; set; } = string.Empty;

        [JsonPropertyName("time_next_update_unix")]
        public long TimeNextUpdateUnix { get; set; }

        [JsonPropertyName("time_next_update_utc")]
        public string TimeNextUpdateUtc { get; set; } = string.Empty;

        [JsonPropertyName("base_code")]
        public string BaseCode { get; set; } = string.Empty;

        [JsonPropertyName("target_code")]
        public string TargetCode { get; set; } = string.Empty;

        [JsonPropertyName("conversion_rate")]
        public decimal ConversionRate { get; set; }

        [JsonPropertyName("conversion_result")]
        public decimal ConversionResult { get; set; }
    }
}