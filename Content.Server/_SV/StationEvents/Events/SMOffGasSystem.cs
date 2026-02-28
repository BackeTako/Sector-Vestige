using Content.Server._EE.Supermatter.Systems;
using Content.Server._SV.StationEvents.Components;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Content.Shared._EE.Supermatter.Components;
using Content.Shared.Database;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._SV.StationEvents.Events;

/// <summary>
/// This handles...
/// </summary>
public sealed class SMOffGasSystem : GameRuleSystem<SMOffGasComponent>
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly SupermatterSystem _superMatter = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private const float LeakCooldown = .25f;

    /// <inheritdoc/>
    protected override void Added(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        base.Added(uid, component, gameRule, args);
        Log.Level = LogLevel.Debug;

        if (!TryGetRandomStation(out var chosenStation))
            return;

        //Find a list of all ACTIVE supermatters. Do not select supermatters that are not currently in a nominal status as it could cause some !FUN! problems.
        var possibleTargets = new List<Entity<SupermatterComponent>>();
        var query = EntityQueryEnumerator<SupermatterComponent, TransformComponent>();
        while (query.MoveNext(out var smUid, out var smComponent, out var xform))
        {
            if (smComponent.Status == SupermatterStatusType.Normal && CompOrNull<StationMemberComponent>(xform.GridUid)?.Station == chosenStation)
            {
                possibleTargets.Add((smUid, smComponent));
            }
        }
        //End the event if there is no supermatters available
        if (possibleTargets.Count <= 0)
        {
            Log.Debug($"Terminating event {uid} as no supermatter found");
            _adminLogger.Add(LogType.EventStopped,
                LogImpact.Low,
                $"Terminating event {uid} as no valid supermatter was found");
            ForceEndSelf(uid, gameRule);
            return;
        }

        component.Supermatter = RobustRandom.Pick(possibleTargets);

        component.TargetTile = _transform.GetGridOrMapTilePosition(component.Supermatter);
        component.StationUid = chosenStation.Value;

        Log.Debug($"Selected supermatter for event {ToPrettyString(uid)} is {ToPrettyString(component.Supermatter)}");

        foreach (var gasCollection in component.AllowedGases)
        {
            Log.Debug($"General list: {gasCollection.Gas}, {gasCollection.Amount}, {gasCollection.Weight}");
        }

        SelectGas(component);
    }

    protected override void Started(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        BuildAnouncement(component);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        //build time when the event will end
        if (gameRule.Delay is {} startAfter)
            stationEvent.EndTime = _timing.CurTime + TimeSpan.FromSeconds(component.GasAmount / component.GasRate + startAfter.Next(RobustRandom));
        Log.Debug($"Event will end at {stationEvent.EndTime}. Current time is {_timing.CurTime}");
    }

    protected override void ActiveTick(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);

        //count down till next gas leak, do nothing if the cooldown hasn't hit yet, or continue and add more time for the next leak 'event'
        component.TimeTillNextLeak -= frameTime;
        if (component.TimeTillNextLeak > 0f)
            return;
        component.TimeTillNextLeak += LeakCooldown;

        Log.Debug($"Attempting to leak at {component.TargetTile}");

        var targetGrid = _transform.GetGrid(component.Supermatter);

        //if by somehow the grid or tile is invalid, or if the atmosphere simulation is disabled, end the event
        if (targetGrid == null ||
            component.TargetTile == default ||
            Deleted(component.StationUid) ||
            !_atmosphere.IsSimulatedGrid(targetGrid.Value))
        {
            Log.Debug($"SM offgas event {uid} canceled as the location is invalid. Target tile is:  {component.TargetTile}, on grid: {targetGrid} for station ID: {component.StationUid}");
            ForceEndSelf(uid, gameRule);

            if (component.TargetTile == default)
                Log.Debug("Target tile a default value");
            if (Deleted(component.StationUid))
                Log.Debug("Target station is deleted (Good job on that)");
            if (targetGrid == null)
            {
                Log.Debug("Target grid is null");
                return;
            }
            if (!_atmosphere.IsSimulatedGrid(targetGrid.Value))
                Log.Debug("Target grid is not simulated with atmos");
            return;
        }

        //stolen from GasLeakRule :)
        var environment = _atmosphere.GetTileMixture(targetGrid, null, component.TargetTile);
        environment?.AdjustMoles(component.SelectedGas, LeakCooldown * component.GasRate);
    }

    protected override void Ended(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        Log.Debug($"SM offgas event {uid} ended as expected");

        if (!_entityManager.TryGetComponent<SupermatterComponent>(component.Supermatter, out var supermatter))
            return;

        _superMatter.SendSupermatterAnnouncement(component.Supermatter, supermatter, Loc.GetString("sv-sm-off-gas-event-end"));
    }

    private void SelectGas(SMOffGasComponent component)
    {
        var weightedGasList = new List<GasSpawnEntry>();
        foreach (var gasCollection in component.AllowedGases)
        {
            for (int i = 0; i < gasCollection.Weight; i++)
            {
                weightedGasList.Add(gasCollection);
            }
        }

        foreach (var gasCollection in weightedGasList)
        {
            Log.Debug($"Weighted list: {gasCollection.Gas}, {gasCollection.Amount}, {gasCollection.Weight}");
        }

        var selectedGas = RobustRandom.Pick(weightedGasList);

        //Vary the gas and gas release amount
        component.GasAmount = selectedGas.Amount.Next(RobustRandom);
        component.GasRate = selectedGas.MolPerSecond.Next(RobustRandom);
        component.SelectedGas = selectedGas.Gas;

        Log.Debug($"selected gas is: {selectedGas.Gas}, with amount {component.GasAmount} at rate {component.GasRate}");
    }

    private void BuildAnouncement(SMOffGasComponent component)
    {
        var amountAnnouncment = component.GasAmount switch
        {
            <= 250 => Loc.GetString("off-gas-event-amount-small"),
            <= 500 => Loc.GetString("off-gas-event-amount-medium"),
            <= 750 => Loc.GetString("off-gas-event-amount-large"),
            <= 1000 => Loc.GetString("off-gas-event-amount-excessive"),
            _ => Loc.GetString("off-gas-event-amount-unknown"),
        };

        var rateAnnouncment = component.GasAmount switch
        {
            <= 10 => Loc.GetString("off-gas-event-rate-small"),
            <= 20 => Loc.GetString("off-gas-event-rate-medium"),
            <= 30 => Loc.GetString("off-gas-event-rate-large"),
            <= 50 => Loc.GetString("off-gas-event-rate-excessive"),
            _ => Loc.GetString("off-gas-event-rate-unknown"),
        };

        //Build the announcement for the SM to say over engineering comms
        var builtAnouncement = Loc.GetString("off-gas-event-announcement", ("amount", amountAnnouncment), ("rate", rateAnnouncment));

        Log.Debug($"built anouncement: {builtAnouncement}");

        if (!_entityManager.TryGetComponent<SupermatterComponent>(component.Supermatter, out var supermatter))
            return;

        _superMatter.SendSupermatterAnnouncement(component.Supermatter, supermatter, builtAnouncement);
    }
}
