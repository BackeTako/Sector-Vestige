using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._SV.CharacterDocuments.Admin;

[AdminCommand(AdminFlags.Admin)]
public sealed partial class AdminCharacterDocumentsCommand : LocalizedCommands
{
    [Dependency] private EuiManager _euis = default!;

    public override string Command => "svadmindocs";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-only-players-can-run-this-command"));
            return;
        }

        var ui = new AdminCharacterDocumentsEui();
        _euis.OpenEui(ui, player);
    }
}
