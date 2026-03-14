using Content.Shared._CD.Records;

namespace Content.Client.Lobby.UI;

/// <summary>
/// This handles the CD's Lobby UI
/// </summary>
public sealed partial class HumanoidProfileEditor
{
    private void UpdateProfileRecords(PlayerProvidedCharacterRecords records)
    {
        if (Profile is null)
            return;
        Profile = Profile.WithCDCharacterRecords(records);
        IsDirty = true;
    }
}
