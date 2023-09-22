// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSetV2;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class OnlineStatsColumn : FillFlowContainer
    {
        public readonly Bindable<APIBeatmap?> BeatmapInfo = new Bindable<APIBeatmap?>();
        public readonly Bindable<APIBeatmapSet?> BeatmapSetInfo = new Bindable<APIBeatmapSet?>();

        private SuccessRateV2 successRate = null!;
        private UserRatingsV2 ratings = null!;

        public OnlineStatsColumn()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(10);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                successRate = new SuccessRateV2(),
                ratings = new UserRatingsV2(),
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            BeatmapInfo.BindValueChanged(b =>
            {
                successRate.BeatmapInfo.Value = b.NewValue;
            });

            BeatmapSetInfo.BindValueChanged(b =>
            {
                ratings.Ratings = b.NewValue?.Ratings ?? Array.Empty<int>();
            });
        }
    }
}
