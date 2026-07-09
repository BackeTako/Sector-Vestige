namespace Content.Shared._SV.CharacterDocuments.Components;

[RegisterComponent]
public sealed partial class CharacterDocumentStationComponent : Component
{
    /// <summary>
    ///     Crew roster for this station's document consoles, keyed by the stable
    ///     <c>ProfileId</c> (not the transient body entity). Entries are added once a
    ///     player's documents finish loading and are <b>never removed on gib / death /
    ///     disconnect</b>, so a crew member stays listed — with accessible documents —
    ///     for the whole round. Maps ProfileId → character name (for display / filtering).
    /// </summary>
    [DataField]
    public Dictionary<int, string> RosterByProfile = new();
}
