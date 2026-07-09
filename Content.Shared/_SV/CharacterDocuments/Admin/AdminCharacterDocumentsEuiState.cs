using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.CharacterDocuments.Admin;

[Serializable, NetSerializable]
public sealed class AdminCharacterDocumentsEuiState : EuiStateBase
{
    public List<AdminSVProfileEntry> Profiles { get; init; } = new();
}

[Serializable, NetSerializable]
public sealed class AdminSVProfileEntry
{
    public int ProfileId;
    /// <summary>
    /// Owning player's stable account UserId. Resolved from Profile → Preference at load time.
    /// Used server-side for live-session lookup instead of <see cref="PlayerName"/> — names can
    /// drift in casing/whitespace between save and login, UserId never does.
    /// </summary>
    public Guid UserId;
    public string PlayerName = string.Empty;
    public string CharacterName = string.Empty;
    public List<CharacterDocument> Documents = new();
}

[Serializable, NetSerializable]
public sealed class AdminSVRefreshMsg : EuiMessageBase;

[Serializable, NetSerializable]
public sealed class AdminSVDocumentEditMsg : EuiMessageBase
{
    public int ProfileId;
    public CharacterDocument Document = default!;
}

[Serializable, NetSerializable]
public sealed class AdminSVDocumentDeleteMsg : EuiMessageBase
{
    public int ProfileId;
    public int DocId;
}

/// <summary>
/// Restores a binned (soft-deleted) document, clearing its deletion timestamp so it
/// returns to normal view.
/// </summary>
[Serializable, NetSerializable]
public sealed class AdminSVDocumentRestoreMsg : EuiMessageBase
{
    public int ProfileId;
    public int DocId;
}

/// <summary>
/// Permanently deletes a single binned (soft-deleted) document, bypassing the retention
/// window. Irreversible. The server refuses to purge a live (non-binned) document.
/// </summary>
[Serializable, NetSerializable]
public sealed class AdminSVDocumentPurgeMsg : EuiMessageBase
{
    public int ProfileId;
    public int DocId;
}

/// <summary>
/// Permanently deletes every binned (soft-deleted) document for the profile — the
/// "empty the recycling bin" action. Live documents are left untouched. Irreversible.
/// </summary>
[Serializable, NetSerializable]
public sealed class AdminSVDocumentEmptyBinMsg : EuiMessageBase
{
    public int ProfileId;
}

[Serializable, NetSerializable]
public sealed class AdminSVDocumentCreateMsg : EuiMessageBase
{
    public int ProfileId;
    public int DocType;
    public string Title = string.Empty;
    public string Content = string.Empty;
    public List<CharacterDocumentStamp> Stamps = new();
}
