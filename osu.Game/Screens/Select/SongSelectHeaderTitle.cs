// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class SongSelectHeaderTitle : CompositeDrawable
    {
        private const float spacing = 6;

        private readonly OsuSpriteText dot;
        private readonly OsuSpriteText pageTitle;

        public SongSelectHeaderTitle()
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Spacing = new Vector2(spacing, 0),
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = OsuFont.TorusAlternate.With(size: 24),
                            Text = "Solo Play"
                        },
                        dot = new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = OsuFont.TorusAlternate.With(size: 24),
                            Text = "·"
                        },
                        pageTitle = new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = OsuFont.TorusAlternate.With(size: 24),
                            Text = "Song Select"
                        }
                    }
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            pageTitle.Colour = dot.Colour = colours.Yellow;
        }
    }
}
