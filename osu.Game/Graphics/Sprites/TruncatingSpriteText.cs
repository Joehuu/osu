// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

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

            // MaxWidth is used instead of RelativeSizeAxes for cases where we need to centre the text.
            MaxWidth = Parent.DrawWidth - Parent.Padding.Left - Parent.Padding.Right - X;
        }

        public override Axes RelativeSizeAxes
        {
            // For consistency, disable RelativeSizeAxes as normal left-anchored text also works with MaxWidth.
            set => throw new InvalidOperationException($"{nameof(RelativeSizeAxes)} is replaced by MaxWidth logic in {nameof(Update)}.");
        }
    }
}
