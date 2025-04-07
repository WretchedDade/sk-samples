using System.Text.Json;
using System.Text.Json.Schema;

namespace SemanticKernelSamples.Helpers;
internal static class JsonHelper
{
    public static string ToJsonSchema(this Type type)
    {
        return JsonSerializerOptions.Web.GetJsonSchemaAsNode(type, new JsonSchemaExporterOptions()
        {
            TreatNullObliviousAsNonNullable = true // This makes the root element not nullable
        }).ToJsonString();
    }
}
