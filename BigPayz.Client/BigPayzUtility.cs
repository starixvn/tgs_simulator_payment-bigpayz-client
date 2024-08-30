using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payment.BigPayz.Client;

public class BigPayzUtility
{
    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = {
        }
    };
}
