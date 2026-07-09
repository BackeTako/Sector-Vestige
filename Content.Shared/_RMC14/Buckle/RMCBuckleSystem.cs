// SPDX-FileCopyrightText: 2026 RMC14 contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 Anzuneth <malachigene@gmail.com>
//
// SPDX-License-Identifier: MIT

// Taken from RMC
using System.Numerics;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Interaction;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Physics.Events;

namespace Content.Shared._RMC14.Buckle;

public sealed partial class RMCBuckleSystem : EntitySystem
{
   // [Dependency] private readonly SharedBuckleSystem _buckle = default!;
    [Dependency] private EntityLookupSystem _entityLookup = default!;
    [Dependency] private EntityWhitelistSystem _entityWhitelist = default!;
   // [Dependency] private readonly SharedPopupSystem _popup = default!;

    private readonly HashSet<EntityUid> _intersecting = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<BuckleClimbableComponent, StrappedEvent>(OnBuckleClimbableStrapped);
        SubscribeLocalEvent<ActiveBuckleClimbingComponent, PreventCollideEvent>(OnBuckleClimbablePreventCollide);
        SubscribeLocalEvent<BuckleWhitelistComponent, BuckleAttemptEvent>(OnBuckleWhitelistAttempt);
    }

    private void OnBuckleClimbableStrapped(Entity<BuckleClimbableComponent> ent, ref StrappedEvent args)
    {
        var active = EnsureComp<ActiveBuckleClimbingComponent>(args.Buckle);
        active.Strap = ent;
        Dirty(args.Buckle, active);
    }

    private void OnBuckleClimbablePreventCollide(Entity<ActiveBuckleClimbingComponent> ent, ref PreventCollideEvent args)
    {
        if (args.Cancelled)
            return;

        if (ent.Comp.Strap == args.OtherEntity)
            args.Cancelled = true;
    }

    private void OnBuckleWhitelistAttempt(Entity<BuckleWhitelistComponent> ent, ref BuckleAttemptEvent args)
    {
        if (!_entityWhitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Strap))
            args.Cancelled = true;
    }

    private void OnBuckleAttemptMobTargetCollide(Entity<BuckleComponent> ent, ref AttemptMobTargetCollideEvent args)
    {
        if (args.Cancelled)
            return;

        if (ent.Comp.Buckled)
            args.Cancelled = true;
    }

    public Vector2 GetOffset(Entity<RMCBuckleOffsetComponent?> offset)
    {
        if (!Resolve(offset, ref offset.Comp, false))
            return Vector2.Zero;

        return offset.Comp.Offset;
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<ActiveBuckleClimbingComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.Strap is not { } strap)
            {
                RemCompDeferred<ActiveBuckleClimbingComponent>(uid);
                continue;
            }

            _intersecting.Clear();
            _entityLookup.GetEntitiesIntersecting(uid, _intersecting);

            if (!_intersecting.Contains(strap))
                RemCompDeferred<ActiveBuckleClimbingComponent>(uid);
        }
    }
}
