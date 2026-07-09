namespace Content.Shared._SV.CharacterDocuments.Components;

/// <summary>
///     Marker component for character-document consoles that should see crew listings
///     from every station / map, not just the owning station's. Apply on Central Command
///     terminals so admins / CC staff working off-station can still pull anyone's records.
/// </summary>
[RegisterComponent]
public sealed partial class CrossMapDocumentAccessComponent : Component
{
}
