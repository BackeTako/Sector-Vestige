using System.Globalization;

namespace Content.Client.Lobby.UI;

/// <summary>
/// This handles custom height
/// </summary>
public sealed partial class HumanoidProfileEditor
{
        private void UpdateHeightControls()
        {
            if (Profile == null)
            {
                return;
            }

            var species = _species.Find(x => x.ID == Profile.Species);
            if (species != null)
            {
                _defaultHeight = species.DefaultHeight;
                // Sector Vestige - Display min/max range and default height for species
                CDHeightRangeLabel.Text = $"Range: {species.MinHeight:F2} - {species.MaxHeight:F2} | Default: {species.DefaultHeight:F2}";
            }

            CDHeight.Text = Profile.Height.ToString(CultureInfo.InvariantCulture);
        }

        private void SetProfileHeight(float height)
        {
            Profile = Profile?.WithHeight(height);
            SetDirty();
            ReloadProfilePreview();
        }
}
