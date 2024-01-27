// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.BeatmapSetV2;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class ExtendedBeatmapDetailsContent : Container, ISerialisableDrawable
    {
        public readonly Bindable<IBeatmapInfo?> BeatmapInfo = new Bindable<IBeatmapInfo?>();
        public readonly Bindable<IBeatmapSetInfo?> BeatmapSetInfo = new Bindable<IBeatmapSetInfo?>();
        private OnlineStatsColumn onlineStats = null!;
        private readonly IBindable<APIState> apiState = new Bindable<APIState>();

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

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
                Children = new Drawable[]
                {
                    new MetadataColumn
                    {
                        Width = 0.3f,
                        Masking = true,
                        BeatmapSetInfo = { BindTarget = BeatmapSetInfo }
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
                                BeatmapInfo = { BindTarget = BeatmapInfo }
                            },
                            new DifficultySettingsContent
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                BeatmapInfo = { BindTarget = BeatmapInfo }
                            },
                        }
                    },
                    new ColumnDivider { X = (.65f + .7f) / 2 },
                    onlineStats = new OnlineStatsColumn
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Width = 0.3f,
                    },
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            BeatmapInfo.BindValueChanged(b =>
            {
                if (b.NewValue is APIBeatmap onlineInfo)
                    onlineStats.BeatmapInfo.Value = onlineInfo;

                fetchOnlineStats();
            }, true);

            BeatmapSetInfo.BindValueChanged(s =>
            {
                if (s.NewValue is APIBeatmapSet onlineSetInfo)
                    onlineStats.BeatmapSetInfo.Value = onlineSetInfo;
            }, true);

            apiState.BindTo(api.State);
            apiState.BindValueChanged(_ => fetchOnlineStats(), true);
        }

        private void fetchOnlineStats()
        {
            if (apiState.Value != APIState.Online || BeatmapInfo.Value is not BeatmapInfo beatmap) return;

            var requestedBeatmap = beatmap;

            var lookup = new GetBeatmapRequest(beatmap);

            lookup.Success += res =>
            {
                Schedule(() =>
                {
                    if (beatmap != requestedBeatmap)
                        // the beatmap has been changed since we started the lookup.
                        return;

                    onlineStats.BeatmapInfo.Value = res;
                    onlineStats.BeatmapSetInfo.Value = res.BeatmapSet;
                });
            };

            api.Queue(lookup);
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

        public bool UsesFixedAnchor { get; set; }
    }
}
