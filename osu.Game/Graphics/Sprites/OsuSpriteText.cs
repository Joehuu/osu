// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;

namespace osu.Game.Graphics.Sprites
{
    public class OsuSpriteText : SpriteText, IHasTooltip
    {
        public string TooltipText { get; set; }

        public OsuSpriteText()
        {
            Shadow = true;
            Font = OsuFont.Default;
        }
    }
}
