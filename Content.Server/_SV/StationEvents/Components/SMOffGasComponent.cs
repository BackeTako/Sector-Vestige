using Content.Server._SV.StationEvents.Events;
using Content.Shared.Atmos;
using Content.Shared.Destructible.Thresholds;

namespace Content.Server._SV.StationEvents.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, Access(typeof(SMOffGasSystem)), AutoGenerateComponentPause]
public sealed partial class SMOffGasComponent : Component
{
    /// <summary>
    /// The type of gas for the SM to off gas
    /// </summary>
    [DataField]
    public Gas AllowedGasTypes { get; set; } = new();

    /// <summary>
    /// How much of the gas will be produced
    /// </summary>
    [DataField]
    public MinMax GasAmount;

    /// <summary>
    /// How fast the gas will be produced (Likely will be redundant)
    /// </summary>
    [DataField]
    public MinMax GasRate;

    /// <summary>
    /// At what point will the SM stop producing gas
    /// </summary>
    [DataField]
    public TimeSpan TimeTillEnd;

    /// <summary>
    /// What temperature the gas should be coming out of the crystal
    /// </summary>
    [DataField]
    public float GasTemp = 293.15f;

    /// <summary>
    /// What temperature the gas should be coming out of the crystal
    /// </summary>
    [DataField]
    public LocId Announcement = "sm-offgas-begin-unspecified";


}
