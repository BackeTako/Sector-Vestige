// SPDX-FileCopyrightText: 2026 RMC14 contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 Anzuneth <malachigene@gmail.com>
//
// SPDX-License-Identifier: MIT

// Taken from RMC
using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Wheelchair;

[RegisterComponent, NetworkedComponent]
[Access(typeof(WheelchairSystem))]
public sealed partial class ActiveWheelchairPilotComponent : Component
{
    [DataField]
    public EntityUid? BellActionEntity;
}
