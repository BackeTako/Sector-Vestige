using System.Diagnostics.Contracts;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.CharacterDocuments;

/// <summary>
///     Character-wide "flavour" metadata authored in the lobby and shipped alongside
///     the SV character documents. Read-only public surface; mutate via the With* helpers.
/// </summary>
[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class CharacterDocumentGeneral
{
    public const int TextMedLen = 64;

    public const int MaxHeight = 800;
    public const int MaxWeight = 300;

    // General data about a character //
    [DataField]
    public int Height { get; set; }

    [DataField]
    public int Weight { get; set; }

    [DataField]
    public string EmergencyContactName { get; set; } = string.Empty;

    // Employment
    [DataField]
    public bool HasWorkAuthorization { get; set; }

    // Security
    [DataField]
    public string IdentifyingFeatures { get; set; } = string.Empty;

    // Medical
    [DataField]
    public string Allergies { get; set; } = string.Empty;

    [DataField]
    public string DrugAllergies { get; set; } = string.Empty;

    [DataField]
    public string PostmortemInstructions { get; set; } = string.Empty;

    public CharacterDocumentGeneral() { }

    public CharacterDocumentGeneral(CharacterDocumentGeneral other)
    {
        Height = other.Height;
        Weight = other.Weight;
        EmergencyContactName = other.EmergencyContactName;
        HasWorkAuthorization = other.HasWorkAuthorization;
        IdentifyingFeatures = other.IdentifyingFeatures;
        Allergies = other.Allergies;
        DrugAllergies = other.DrugAllergies;
        PostmortemInstructions = other.PostmortemInstructions;
    }

    public static CharacterDocumentGeneral Default() => new()
    {
        Height = 170,
        Weight = 70,
        HasWorkAuthorization = true,
        EmergencyContactName = string.Empty,
        IdentifyingFeatures = string.Empty,
        Allergies = "None",
        DrugAllergies = "None",
        PostmortemInstructions = "Return home",
    };

    public void EnsureValid()
    {
        Height = Math.Clamp(Height, 0, MaxHeight);
        Weight = Math.Clamp(Weight, 0, MaxWeight);
        EmergencyContactName = Clamp(EmergencyContactName, TextMedLen);
        IdentifyingFeatures = Clamp(IdentifyingFeatures, TextMedLen);
        Allergies = Clamp(Allergies, TextMedLen);
        DrugAllergies = Clamp(DrugAllergies, TextMedLen);
        PostmortemInstructions = Clamp(PostmortemInstructions, TextMedLen);
    }

    public bool MemberwiseEquals(CharacterDocumentGeneral other)
    {
        return Height == other.Height
               && Weight == other.Weight
               && HasWorkAuthorization == other.HasWorkAuthorization
               && EmergencyContactName == other.EmergencyContactName
               && IdentifyingFeatures == other.IdentifyingFeatures
               && Allergies == other.Allergies
               && DrugAllergies == other.DrugAllergies
               && PostmortemInstructions == other.PostmortemInstructions;
    }

    public CharacterDocumentGeneral WithHeight(int v) => new(this) { Height = v };
    public CharacterDocumentGeneral WithWeight(int v) => new(this) { Weight = v };
    public CharacterDocumentGeneral WithEmergencyContactName(string v) => new(this) { EmergencyContactName = v };
    public CharacterDocumentGeneral WithWorkAuthorization(bool v) => new(this) { HasWorkAuthorization = v };
    public CharacterDocumentGeneral WithIdentifyingFeatures(string v) => new(this) { IdentifyingFeatures = v };
    public CharacterDocumentGeneral WithAllergies(string v) => new(this) { Allergies = v };
    public CharacterDocumentGeneral WithDrugAllergies(string v) => new(this) { DrugAllergies = v };
    public CharacterDocumentGeneral WithPostmortemInstructions(string v) => new(this) { PostmortemInstructions = v };

    [Pure]
    private static string Clamp(string? s, int maxLen)
    {
        if (s == null) return string.Empty;
        return s.Length > maxLen ? s[..maxLen] : s;
    }
}
