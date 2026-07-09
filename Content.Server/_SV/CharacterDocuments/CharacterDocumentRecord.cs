using Content.Shared._SV.CharacterDocuments;
using Robust.Shared.Network;

namespace Content.Server._SV.CharacterDocuments;

/// <summary>
///     Authoritative in-round document record for a single character, keyed by
///     <see cref="ProfileId"/>. Held in the <see cref="CharacterDocumentSystem"/> store
///     rather than on the player body, so it survives body destruction (gibbing) and
///     stays accessible + editable for the whole round. Mirrors the DB rows in
///     <c>sv_character_document_entries</c> — persistence is unchanged and still keyed
///     by <see cref="ProfileId"/>.
/// </summary>
public sealed class CharacterDocumentRecord
{
    /// <summary>Stable DB key for the owning profile (primary key in <c>sv_profiles</c>).</summary>
    public int ProfileId;

    /// <summary>Character name, for roster display and criminal-record matching.</summary>
    public string Name = string.Empty;

    /// <summary>
    ///     Owning player's account username, captured at spawn. Persisted as
    ///     <c>SVProfile.PlayerName</c> so it stays correct even after the player ghosts.
    /// </summary>
    public string Username = string.Empty;

    /// <summary>Owning player's account id, used to resolve their session for prefs refreshes.</summary>
    public NetUserId UserId;

    /// <summary>Live document set for this character, keyed by DocID.</summary>
    public Dictionary<int, CharacterDocument> Documents = new();

    /// <summary>Lobby-authored flavour metadata (height, allergies, etc).</summary>
    public CharacterDocumentGeneral General = new();
}
