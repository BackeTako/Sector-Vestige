// SPDX-FileCopyrightText: 2026 RMC14 contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 Anzuneth <malachigene@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Buckle;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(RMCBuckleSystem))]
public sealed partial class RMCBuckleDrawDepthComponent : Component
{
    [DataField, AutoNetworkedField]
    public DrawDepth.DrawDepth? BuckleDepth;

    [DataField, AutoNetworkedField]
    public DrawDepth.DrawDepth UnbuckleDepth = DrawDepth.DrawDepth.Mobs;
}
