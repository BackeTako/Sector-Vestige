
using Content.Shared._SV.CharacterDocuments;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Security;
using Robust.Shared.Audio;

namespace Content.Server._SV.CharacterDocuments.Consoles;

[RegisterComponent]
public sealed partial class CharacterDocumentConsoleComponent : Component
{
    [DataField(required: true), ViewVariables(VVAccess.ReadOnly)]
    public DocumentType DocumentType;

    /// <summary>
    /// Extra document types this console can view in addition to <see cref="DocumentType"/>.
    /// Used by multi-type consoles like the Central Command computer. The primary
    /// <see cref="DocumentType"/> is what newly scanned docs are tagged as.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public List<DocumentType> AdditionalDocumentTypes = new();

    /// <summary>Selected player's stable ProfileId, or 0 for no selection.</summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public int SelectedPlayer;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public CharacterDocument? SelectedDocument;

    /// The internal scanner part ///
    /// <summary>
    /// Contains the item to be sent, assumes it's paper...
    /// </summary>
    [DataField(required: true)]
    public ItemSlot PaperSlot = new();

    /// <summary>
    /// Sound to play when we print something out
    /// </summary>
    [DataField]
    public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg");

    /// <summary>
    /// Sound to play when something went right like importing a document
    /// </summary>
    [DataField]
    public SoundSpecifier SuccessSound = new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg");

    /// <summary>
    /// Sound to play when something goes wrong
    /// </summary>
    [DataField]
    public SoundSpecifier ErrorSound = new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_sigh.ogg");


    /// Security console specefic rules

    [DataField]
    public SecurityStatus SecurityStatus;

    [DataField]
    public string? SecurityReason;


}
