// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.Delivery;

/// <summary>
/// Marks an item as player-addressable mail: it can be loaded with items via its storage,
/// activating the "Address" verb opens a UI to choose a recipient, and sending it spawns a real
/// delivery (carrying the loaded items) addressed to them through the mail system.
/// The chosen recipient is stored here until the item is sent.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AddressableDeliveryComponent : Component
{
    /// <summary>
    /// The delivery prototype spawned into the mail system when this is sent
    /// (e.g. a letter delivery for a blank letter, a package delivery for a blank package).
    /// </summary>
    [DataField]
    public EntProtoId DeliveryProto = "SVLetterDeliveryAddressed";

    /// <summary>
    /// Station record id of the chosen recipient, if the item has been addressed.
    /// </summary>
    [DataField]
    public uint? RecipientRecordKey;

    /// <summary>
    /// Name of the chosen recipient.
    /// </summary>
    [DataField]
    public string? RecipientName;

    /// <summary>
    /// Job title of the chosen recipient.
    /// </summary>
    [DataField]
    public string? RecipientJobTitle;

    /// <summary>
    /// Station the chosen recipient belongs to.
    /// </summary>
    [DataField]
    public EntityUid? RecipientStation;
}
