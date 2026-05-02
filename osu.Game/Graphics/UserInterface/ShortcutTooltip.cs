// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Graphics.UserInterface
{
    public partial class ShortcutTooltip : CompositeDrawable, ITooltip<Shortcut>
    {
        private OsuSpriteText shortcutText = null!;
        private HotkeyDisplay hotkeyDisplay = null!;

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AutoSizeAxes = Axes.Both;
            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colours.GreySeaFoamDarker
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5),
                    Padding = new MarginPadding(10),
                    Children = new Drawable[]
                    {
                        shortcutText = new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        hotkeyDisplay = new HotkeyDisplay
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                        }
                    }
                }
            };
        }

        public void Move(Vector2 pos) => Position = pos;

        public void SetContent(Shortcut shortcut)
        {
            shortcutText.Text = shortcut.Name;
            hotkeyDisplay.Hotkey = shortcut.Hotkey;
        }
    }

    public class Shortcut
    {
        public LocalisableString Name { get; init; }
        public Hotkey Hotkey { get; init; }
    }
}
