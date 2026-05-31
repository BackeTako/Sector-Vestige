// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 dffdff2423 <dffdff2423@gmail.com>
// SPDX-FileCopyrightText: 2026 lunarcomets (GitHub)
//
// SPDX-License-Identifier: MIT

using Content.Shared.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._CD.Prototypes;

public sealed partial class ALPrototypeSystem : EntitySystem
{
    [Dependency] private IComponentFactory _compFactory = default!;
    [Dependency] private IPrototypeManager _prototype = default!;

    public IEnumerable<(EntityPrototype Prototype, T Component)> EnumerateComponents<T>() where T : IComponent, new()
    {
        foreach (var entity in _prototype.EnumeratePrototypes<EntityPrototype>())
        {
            if (entity.TryGetComponent(out T? comp, _compFactory))
                yield return (entity, comp);
        }
    }

    public IEnumerable<EntityPrototype> EnumerateEntities<T>() where T : IComponent, new()
    {
        foreach (var entity in _prototype.EnumeratePrototypes<EntityPrototype>())
        {
            if (entity.HasComponent<T>(_compFactory))
                yield return entity;
        }
    }
}
