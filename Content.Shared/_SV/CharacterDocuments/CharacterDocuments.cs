using Content.Shared.Paper;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.CharacterDocuments;

public enum DocumentType
{
    Employment,
    Medical,
    Security,
    CentralCommand,
    Syndicate,
    Admin,
}

[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class CharacterDocument
{
    [DataField]
    public int DocID;

    [DataField]
    public int DocType = (int)DocumentType.Employment;

    [DataField]
    public string DocTitle = string.Empty;

    [DataField]
    public string DocAuthor = string.Empty;

    [DataField]
    public string DocLastEditedBy = string.Empty;

    [DataField]
    public DateTime DocDateLastEdited = DateTime.Today.AddYears(200); //Today plus 200 years

    [DataField]
    public string DocContent = string.Empty;

    [DataField]
    public List<CharacterDocumentStamp> DocStamps = new();

    /// <summary>
    /// When non-null, this document has been "binned" (soft-deleted): it is hidden from
    /// the normal document lists but retained so Central Command / admins can still see
    /// and restore it. A background sweep permanently purges binned docs once they are
    /// older than the configured retention window (see <c>SVCCVars.CharacterDocumentBinRetentionDays</c>).
    /// Stored as UTC.
    /// </summary>
    [DataField]
    public DateTime? DeletedAt;
}

[DataDefinition]
[Serializable, NetSerializable]
public partial struct CharacterDocumentStamp()
{
    [DataField]
    public StampDisplayInfo DocStamp = default;

    [DataField]
    public string DocStampState = string.Empty;
}
