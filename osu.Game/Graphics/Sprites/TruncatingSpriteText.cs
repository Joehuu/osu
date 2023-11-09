// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;

namespace osu.Game.Graphics.Sprites
{
    /// <summary>
    /// A derived version of <see cref="OsuSpriteText"/> which automatically shows non-truncated text in tooltip when required.
    /// </summary>
    public sealed partial class TruncatingSpriteText : OsuSpriteText, IHasTooltip
    {
        /// <summary>
        /// Whether a tooltip should be shown with non-truncated text on hover.
        /// </summary>
        public bool ShowTooltip { get; init; } = true;

        public LocalisableString TooltipText => Text;

        public override bool HandlePositionalInput => IsTruncated && ShowTooltip;

        public TruncatingSpriteText()
        {
            ((SpriteText)this).Truncate = true;
        }

        protected override void Update()
        {
            base.Update();

            // find if this sprite text is a link (i.e. child of OsuHoverContainer)
            // and confine the auto-sized text to the parent of the hover container's child width
            // Parent => OsuHoverContainer.Content
            if (Parent!.Parent is OsuHoverContainer hoverContainer)
                MaxWidth = hoverContainer.Parent!.ChildSize.X;
            else
            {
                // else just use the parent's child width
                MaxWidth = Parent!.ChildSize.X;
            }
        }
    }
}
