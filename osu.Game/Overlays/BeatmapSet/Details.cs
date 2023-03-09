// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSet.Buttons;
using osu.Game.Screens.Select.Details;
using osuTK;

namespace osu.Game.Overlays.BeatmapSet
{
    public partial class Details : FillFlowContainer
    {
        protected readonly UserRatings Ratings;

        private readonly BasicStats basic;
        private readonly AdvancedStats advanced;
        private readonly DetailBox ratingBox;

        [Resolved]
        private Bindable<APIBeatmapSet> beatmapSet { get; set; } = null!;

        private IBeatmapInfo beatmapInfo;

        public IBeatmapInfo BeatmapInfo
        {
            get => beatmapInfo;
            set
            {
                if (value == beatmapInfo) return;

                basic.BeatmapInfo = advanced.BeatmapInfo = beatmapInfo = value;
            }
        }

        private void updateDisplay()
        {
            Ratings.Ratings = beatmapSet.Value?.Ratings;
            ratingBox.Alpha = beatmapSet.Value?.Status > 0 ? 1 : 0;
        }

        public Details()
        {
            Width = BeatmapSetOverlay.RIGHT_WIDTH;
            AutoSizeAxes = Axes.Y;
            Spacing = new Vector2(1f);

            Children = new Drawable[]
            {
                new PreviewButton
                {
                    RelativeSizeAxes = Axes.X,
                },
                new DetailBox
                {
                    Child = basic = new BasicStats
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Vertical = 10 }
                    },
                },
                new DetailBox
                {
                    Child = advanced = new AdvancedStats
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Margin = new MarginPadding { Vertical = 7.5f },
                    },
                },
                ratingBox = new DetailBox
                {
                    Child = Ratings = new UserRatings
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 95,
                        Margin = new MarginPadding { Top = 10 },
                    },
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmapSet.BindValueChanged(_ => updateDisplay(), true);
        }

        private partial class DetailBox : Container
        {
            private readonly Container content;
            private readonly Box background;

            protected override Container<Drawable> Content => content;

            public DetailBox()
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.5f
                    },
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Horizontal = 15 },
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
                background.Colour = colourProvider.Background6;
            }
        }
    }
}
