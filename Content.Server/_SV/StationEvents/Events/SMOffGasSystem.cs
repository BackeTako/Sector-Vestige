using Content.Server._EE.Supermatter.Systems;
using Content.Server._SV.StationEvents.Components;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Content.Shared._EE.Supermatter.Components;
using Content.Shared.Atmos;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server._SV.StationEvents.Events;

/// <summary>
/// This handles...
/// </summary>
public sealed class SMOffGasSystem : GameRuleSystem<SMOffGasComponent>
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SupermatterSystem _superMatter = default!;
    [Dependency] private readonly StationSystem _station = default!;
    /// <inheritdoc/>
    protected override void Started(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TryGetRandomStation(out var chosenStation))
            return;

        var target = RobustRandom.Pick(new List<Entity<SupermatterComponent>>());

        //End the event if there is no super matter
        if (target == null)
        {
            ForceEndSelf(uid, gameRule);
        }

        _superMatter.SendSupermatterAnnouncement(chosenStation.Value, target, "FUCK YOU");
        ForceEndSelf(uid, gameRule);
    }


}
