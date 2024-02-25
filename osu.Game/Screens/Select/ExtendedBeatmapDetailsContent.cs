// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osu.Game.Overlays.BeatmapSetV2;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class ExtendedBeatmapDetailsContent : Container
    {
        public ExtendedBeatmapDetailsContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                // best effort to not use a grid container and
                // do fancy resizing when needed (i.e. hide metadata column and expand second column)
                Children = new Drawable[]
                {
                    new MetadataColumn
                    {
                        Width = 0.3f,
                        Masking = true,
                    },
                    new ColumnDivider { X = (.3f + .35f) / 2 },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Width = 0.3f,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new ObjectCountContent
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                            },
                            new DifficultySettingsContent
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                            },
                        }
                    },
                    new ColumnDivider { X = (.65f + .7f) / 2 },
                    new OnlineStatsColumn
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Width = 0.3f,
                    },
                }
            };
        }

        private partial class ColumnDivider : CompositeDrawable
        {
            public ColumnDivider()
            {
                Origin = Anchor.TopCentre;
                RelativePositionAxes = Axes.X;
                Width = 2;
                RelativeSizeAxes = Axes.Y;
                Masking = true;
                CornerRadius = 1;
            }

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
                InternalChild = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background4,
                };
            }
        }
    }
}
