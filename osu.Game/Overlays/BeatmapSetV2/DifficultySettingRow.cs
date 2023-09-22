// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class DifficultySettingRow : GridContainer, IHasAccentColour
    {
        private const float value_width = 25;
        private const float name_width = 70;

        private readonly float maxValue;
        private readonly bool forceDecimalPlaces;
        private OsuTextFlowContainer name = null!;
        private OsuSpriteText valueText = null!;
        private Bar bar = null!;
        public Bar ModBar = null!;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        private LocalisableString title;

        public LocalisableString Title
        {
            get => title;
            set
            {
                title = value;
                if (name != null) name.Text = value;
            }
        }

        private (float baseValue, float? adjustedValue)? value;

        public (float baseValue, float? adjustedValue) Value
        {
            get => value ?? (0, null);
            set
            {
                if (value == this.value)
                    return;

                this.value = value;

                bar.Length = value.baseValue / maxValue;

                valueText.Text = (value.adjustedValue ?? value.baseValue).ToString(forceDecimalPlaces ? "0.00" : "0.##");
                ModBar.Length = (value.adjustedValue ?? 0) / maxValue;

                if (Precision.AlmostEquals(value.baseValue, value.adjustedValue ?? value.baseValue, 0.05f))
                    ModBar.AccentColour = valueText.Colour = Color4.White;
                else if (value.adjustedValue > value.baseValue)
                    ModBar.AccentColour = valueText.Colour = colours.Red;
                else if (value.adjustedValue < value.baseValue)
                    ModBar.AccentColour = valueText.Colour = colours.BlueDark;
            }
        }

        public Color4 AccentColour
        {
            get => bar.AccentColour;
            set => bar.AccentColour = value;
        }

        public DifficultySettingRow(float maxValue = 10, bool forceDecimalPlaces = false)
        {
            this.maxValue = maxValue;
            this.forceDecimalPlaces = forceDecimalPlaces;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            ColumnDimensions = new[]
            {
                new Dimension(GridSizeMode.Absolute, name_width),
                new Dimension(GridSizeMode.Absolute, value_width),
            };
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize),
            };
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            Content = new[]
            {
                new Drawable[]
                {
                    name = new OsuTextFlowContainer(t => t.Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12))
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Text = title,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Child = valueText = new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = OsuFont.GetFont(weight: FontWeight.Bold, size: 12),
                        },
                    },
                    new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            bar = new Bar
                            {
                                Masking = true,
                                CornerRadius = 2,
                                RelativeSizeAxes = Axes.X,
                                Height = 4,
                                BackgroundColour = colourProvider.Background6,
                                AccentColour = colourProvider.Highlight1,
                            },
                            ModBar = new Bar
                            {
                                Masking = true,
                                CornerRadius = 2,
                                Alpha = 0.5f,
                                RelativeSizeAxes = Axes.X,
                                Height = 4,
                            },
                        }
                    }
                }
            };
        }
    }
}
