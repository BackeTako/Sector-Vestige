using Content.Shared._SV.CharacterDocuments;
using Content.Shared._SV.CharacterDocuments.Consoles;
using Content.Shared.Security;
using Robust.Client.UserInterface;

namespace Content.Client._SV.CharacterDocuments.UI;

public sealed class CharacterDocumentConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private CharacterDocumentConsoleWindow? _window;

    public CharacterDocumentConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<CharacterDocumentConsoleWindow>();
        _window.OnPlayerSelected += player => SendMessage(new SelectCharacterDocumentPlayer { ProfileId = player });
        _window.OnDocumentSelected += (player, docId) => SendMessage(new SelectCharacterDocument { ProfileId = player, DocID = docId });
        _window.OnButtonScanPressed += (player, title, docType) => SendMessage(new CharacterDocumentScan
        {
            ProfileId = player,
            DocTitle = title,
            DocType = docType.HasValue ? (int)docType.Value : null,
        });
        _window.OnButtonPrintPressed += (player, doc) => SendMessage(new CharacterDocumentPrint { ProfileId = player, CharacterDocument = doc });
        _window.OnButtonDeletePressed += (player, doc) => SendMessage(new CharacterDocumentDelete { ProfileId = player, CharacterDocument = doc });
        _window.OnButtonRestorePressed += (player, docId) => SendMessage(new CharacterDocumentRestore { ProfileId = player, DocID = docId });
        _window.OnButtonPurgePressed += (player, docId) => SendMessage(new CharacterDocumentPurge { ProfileId = player, DocID = docId });
        _window.OnButtonEmptyBinPressed += player => SendMessage(new CharacterDocumentEmptyBin { ProfileId = player });
        _window.OnButtonEditPressed += (player, doc) => SendMessage(new CharacterDocumentEdit { ProfileId = player, CharacterDocument = doc });
        _window.OnDocumentDeselected += () => SendMessage(new CharacterDocumentDeselect());
        _window.OnStatusButtonPressed += player =>
        {
            var popup = new CharacterDocumentStatusPopup();
            popup.OnConfirmed += (status, reason) =>
                SendMessage(new CharacterDocumentSecurityStatus { ProfileId = player, Status = status, Reason = reason });
            popup.OpenCentered();
        };

    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not CharacterDocumentConsoleState cast)
            return;

        _window?.UpdateState(cast);
    }
}
