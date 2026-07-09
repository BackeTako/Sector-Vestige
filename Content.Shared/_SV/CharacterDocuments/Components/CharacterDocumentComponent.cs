
namespace Content.Shared._SV.CharacterDocuments.Components;

/// <summary>
///     Link component placed on a spawned player body that ties it to its character
///     document record. The documents themselves no longer live here — they live in a
///     round-stable, <c>ProfileId</c>-keyed store owned by the server
///     <c>CharacterDocumentSystem</c> so they survive body destruction (gibbing). This
///     component only carries the identifiers needed to (a) drive station roster
///     membership on spawn / parent-change and (b) link a live body back to its record.
/// </summary>
[RegisterComponent]
public sealed partial class CharacterDocumentComponent : Component
{
    /// <summary>
    /// Profile ID used as a stable identifier for this player's character across rounds.
    /// Resolved from the DB during the async document load; 0 until then.
    /// </summary>
    [DataField]
    public int ProfileId;

    [DataField]
    public string ProfileName = string.Empty;

    /// <summary>
    ///     Owning player's account username, captured at spawn while the session is
    ///     guaranteed attached. Fed into the record so the DB save paths keep
    ///     <c>SVProfile.PlayerName</c> correct even when the player has since ghosted /
    ///     detached (which would make <c>TryGetSessionByEntity</c> return null).
    /// </summary>
    [DataField]
    public string PlayerUsername = string.Empty;
}
