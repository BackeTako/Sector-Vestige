using Content.Server._SV.StationEvents.Events;
using Content.Shared._EE.Supermatter.Components;
using Content.Shared.Atmos;
using Content.Shared.Destructible.Thresholds;
using Robust.Shared.Map;

namespace Content.Server._SV.StationEvents.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, Access(typeof(SMOffGasSystem)), AutoGenerateComponentPause]
public sealed partial class SMOffGasComponent : Component
{
    /// <summary>
    /// A list of gases, their weights, spawn amounts, and spawn rates for the internal process to select
    /// </summary>
    [DataField("allowedGases")]
    public List<GasSpawnEntry> AllowedGases = new();

    /// <summary>
    /// The type of gas for the SM to off gas
    /// </summary>
    [DataField]
    public Gas SelectedGas;

    /// <summary>
    /// How fast the gas gets emitted
    /// </summary>
    [DataField]
    public int GasAmount;

    /// <summary>
    /// How long until the next leak event occurs
    /// </summary>
    [DataField]
    public float TimeTillNextLeak = 1f;

    /// <summary>
    /// How fast the gas will be produced (Likely will be redundant)
    /// </summary>
    [DataField]
    public int GasRate;

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
    /// What announcement should play from the sm crystal when the event starts?
    /// </summary>
    [DataField]
    public LocId Announcement = "sm-offgas-begin-unspecified";

    /// <summary>
    /// Where is the gas going to be spawned?
    /// </summary>
    [DataField]
    public Vector2i TargetTile;

    /// <summary>
    /// The station that the leak is happening on
    /// </summary>
    [DataField]
    public EntityUid StationUid;

    /// <summary>
    /// The supermatter that was selected
    /// </summary>
    [DataField]
    public EntityUid Supermatter;
}
