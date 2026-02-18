using Content.Server._EE.Supermatter.Systems;
using Content.Server._SV.StationEvents.Components;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Content.Shared._EE.Supermatter.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Database;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._SV.StationEvents.Events;

/// <summary>
/// This handles...
/// </summary>
public sealed class SMOffGasSystem : GameRuleSystem<SMOffGasComponent>
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SupermatterSystem _superMatter = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    /// <inheritdoc/>
    protected override void Started(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TryGetRandomStation(out var chosenStation))
            return;

        //Find a list of all ACTIVE supermatters. Do not select supermatters that are not currently in a nominal status as it could cause some !FUN! problems.
        var possibleTargets = new List<Entity<SupermatterComponent>>();
        var query = EntityQueryEnumerator<SupermatterComponent, TransformComponent>();
        while (query.MoveNext(out var smUid, out var smComponent, out var xform))
        {
            if (smComponent.Status == SupermatterStatusType.Inactive && CompOrNull<StationMemberComponent>(xform.GridUid)?.Station == chosenStation) //TODO: set SupermatterStatusType.Inactive to SupermatterStatusType.Normal when starting testing
            {
                possibleTargets.Add((smUid, smComponent));
            }
        }
        _adminLogger.Add(LogType.EventStarted, LogImpact.Extreme, $"Possible SMoffgas targets for {ToPrettyString(uid)} are {possibleTargets.Count}");

        //End the event if there is no supermatters available
        if (possibleTargets.Count <= 0)
        {
            _chat.DispatchGlobalAnnouncement("Terminating self");
            ForceEndSelf(uid, gameRule);
            return;
        }

        var selectedTarget = RobustRandom.Pick(possibleTargets);

        ForceEndSelf(uid, gameRule);
        _chat.DispatchGlobalAnnouncement($"Selected supermatter for event {ToPrettyString(uid)} is {ToPrettyString(selectedTarget)}");
    }


}
