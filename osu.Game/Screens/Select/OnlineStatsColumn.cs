// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSetV2;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class OnlineStatsColumn : CompositeDrawable
    {
        [Resolved]
        private IBindable<IBeatmapInfo?> beatmapInfo { get; set; } = null!;

        [Resolved]
        private IBindable<IBeatmapSetInfo?> beatmapSetInfo { get; set; } = null!;

        [Resolved]
        private IBindable<APIBeatmap?> apiBeatmap { get; set; } = null!;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        private readonly IBindable<APIState> apiState = new Bindable<APIState>();

        private SuccessRateV2 successRate = null!;
        private UserRatingsV2 ratings = null!;

        private LoadingLayer loading = null!;

        public OnlineStatsColumn()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        successRate = new SuccessRateV2(),
                        ratings = new UserRatingsV2(),
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(-5),
                    Child = new Container
                    {
                        CornerRadius = 5,
                        Masking = true,
                        RelativeSizeAxes = Axes.Both,
                        Child = loading = new LoadingLayer(dimBackground: true)
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmapInfo.BindValueChanged(_ => loading.Show());

            beatmapSetInfo.BindValueChanged(_ => updateRatings());

            apiState.BindTo(api.State);
            apiState.BindValueChanged(_ => loading.Show(), true);

            apiBeatmap.BindValueChanged(b =>
            {
                successRate.BeatmapInfo.Value = b.NewValue;

                successRate.FadeTo(b.NewValue != null ? 1 : 0.25f, 250, Easing.OutQuint);

                updateRatings();

                if (b.NewValue != null)
                    loading.Hide();
            }, true);
        }

        private void updateRatings()
        {
            if (beatmapSetInfo.Value is APIBeatmapSet set)
                ratings.Ratings = set.Ratings;
            else
                ratings.Ratings = apiBeatmap.Value?.BeatmapSet?.Ratings;

            ratings.FadeTo(apiBeatmap.Value != null ? 1 : 0.25f, 250, Easing.OutQuint);
        }
    }
}
