using System.Text.Json;
using Content.Server.Database;
using Content.Shared._SV.CharacterDocuments;
using Content.Shared.Paper;

namespace Content.Server._SV.CharacterDocuments;

public static class CharacterDocumentSerializer
{
    public static JsonDocument SerializeDocument(IReadOnlyCollection<SVModel.CharacterDocument> characterDocuments)
    {
        var serializeddoc = JsonSerializer.Serialize(characterDocuments);
        return JsonDocument.Parse(serializeddoc);
    }

    /// <summary>
    /// Takes a CharacterDocumentStamp object and turns it into a JSON
    /// </summary>
    /// <param name="stamps"></param>
    /// <returns></returns>
    public static string SerializeStamp(List<CharacterDocumentStamp> stamps)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { IncludeFields = true };
        return JsonSerializer.Serialize(stamps, jsonSerializerOptions);
    }
}
