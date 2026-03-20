// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 OnyxTheBrave <131422822+OnyxTheBrave@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.BarSign;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client.BarSign.Ui;

[UsedImplicitly]
public sealed class BarSignBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private BarSignMenu? _menu;

    protected override void Open()
    {
        base.Open();

        var allSigns = BarSignSystem.GetAllBarSigns(_prototype)
            .OrderBy(p => Loc.GetString(p.Name))
            .ToList();

        _menu = this.CreateWindow<BarSignMenu>();
        _menu.LoadSigns(allSigns);

        _menu.OnSignSelected += id =>
        {
            SendPredictedMessage(new SetBarSignMessage(id));
        };

        _menu.OnClose += Close;
        _menu.OpenToLeft();
    }

    public override void Update()
    {
        if (!EntMan.TryGetComponent<BarSignComponent>(Owner, out var signComp)
            || !_prototype.Resolve(signComp.Current, out var signPrototype))
            return;

        _menu?.UpdateState(signPrototype);
    }

}

