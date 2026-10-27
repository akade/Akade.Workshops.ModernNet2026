using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;

namespace AI.StarterApp.TicketTriage;

internal sealed class FAQRecord
{
    /// <summary>
    /// Local: 384, Lyceum: 4096
    /// </summary>
    public const int VectorDimensions = 384;

    public const string CollectionName = "ticket-triage-faq";

    [VectorStoreKey(StorageName = "key")]
    [JsonPropertyName("key")]
    public ulong Key { get; set; }

    [VectorStoreData(StorageName = "title")]
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [VectorStoreData(StorageName = "content")]
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [VectorStoreVector(VectorDimensions, DistanceFunction = DistanceFunction.CosineSimilarity, StorageName = "embedding")]
    [JsonPropertyName("embedding")]
    public string Vector => $"{Title}\n{Content}";
}
