using System.Text.Json;
using Content.Server.Database;
using Content.Shared._SV.CharacterDocuments;
using Content.Shared.Paper;
using Robust.Shared.Log;

namespace Content.Server._SV.CharacterDocuments;

public static class CharacterDocumentDeserializer
{
    private static readonly ISawmill Sawmill = Logger.GetSawmill("sv.docs.stamps");

    public static List<SVModel.CharacterDocument> DeserializeDocument(JsonDocument serializedDocument)
    {
        return JsonSerializer.Deserialize<List<SVModel.CharacterDocument>>(serializedDocument)
            ?? new List<SVModel.CharacterDocument>();
    }

    /// <summary>
    /// Takes a JSON string from SerializeStamps and converts it back into the CharacterDocumentStamp Object.
    /// Falls back to an empty list with a warning if the blob is unparseable — silently dropping
    /// stamps without a log line is a debugging nightmare when a stamp prototype is renamed/removed upstream.
    /// </summary>
    public static List<CharacterDocumentStamp> DeserializeStamps(string data)
    {
        if (string.IsNullOrEmpty(data))
            return new List<CharacterDocumentStamp>();

        var options = new JsonSerializerOptions { IncludeFields = true };
        try
        {
            return JsonSerializer.Deserialize<List<CharacterDocumentStamp>>(data, options)
                ?? new List<CharacterDocumentStamp>();
        }
        catch (JsonException ex)
        {
            Sawmill.Warning($"Failed to deserialize SV document stamps blob (len={data.Length}): {ex.Message}");
            return new List<CharacterDocumentStamp>();
        }
    }
}

