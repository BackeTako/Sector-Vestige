using System.Collections.Generic;

namespace Content.Shared._SV.CharacterDocuments;

/// <summary>
///     Helpers for keeping <see cref="CharacterDocument.DocID"/> usable as a stable, unique key in
///     the lobby document editor.
/// </summary>
public static class CharacterDocumentIds
{
    /// <summary>
    ///     Hands every document with <c>DocID == 0</c> a locally-unique negative temp ID — mutating
    ///     the list in place — and returns the next free temp ID.
    /// </summary>
    /// <param name="docs">Documents to normalise in place.</param>
    /// <param name="nextTempId">The next temp ID to assign; negative and decreasing.</param>
    /// <returns>The updated next temp ID, kept below every negative ID now in the list.</returns>
    public static int AssignTempIds(IReadOnlyList<CharacterDocument> docs, int nextTempId)
    {
        // Keep the counter below any negative IDs already present (e.g. docs the editor added
        // earlier this session) so a freshly-assigned ID can never collide with them.
        foreach (var doc in docs)
        {
            if (doc.DocID < 0 && doc.DocID <= nextTempId)
                nextTempId = doc.DocID - 1;
        }

        foreach (var doc in docs)
        {
            if (doc.DocID == 0)
                doc.DocID = nextTempId--;
        }

        return nextTempId;
    }
}
