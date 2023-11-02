// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.BeatmapSetV2;
using osu.Game.Rulesets;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class ExtendedBeatmapDetailsContent : Container
    {
        public readonly Bindable<IBeatmapInfo?> BeatmapInfo = new Bindable<IBeatmapInfo?>();
        public readonly Bindable<IBeatmapSetInfo?> BeatmapSetInfo = new Bindable<IBeatmapSetInfo?>();
        private FillFlowContainer<PillCountStatistic> objectCountFlow = null!;
        private OnlineStatsColumn onlineStats = null!;
        private readonly IBindable<APIState> apiState = new Bindable<APIState>();

        [Resolved]
        private IBindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private IRulesetStore rulesetStore { get; set; } = null!;

        [Resolved]
        private OsuGameBase game { get; set; } = null!;

        public ExtendedBeatmapDetailsContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 50),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 25),
                    new Dimension(),
                },
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new[]
                    {
                        new MetadataColumn
                        {
                            Masking = true,
                            BeatmapSetInfo = { BindTarget = BeatmapSetInfo }
                        },
                        new ColumnDivider(),
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(10),
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    CornerRadius = 10,
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = colourProvider.Background4
                                        },
                                        objectCountFlow = new FillFlowContainer<PillCountStatistic>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(5),
                                            Padding = new MarginPadding { Horizontal = 20, Vertical = 5 },
                                        },
                                    }
                                },
                                new DifficultySettingsColumn
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    BeatmapInfo = { BindTarget = BeatmapInfo }
                                },
                            }
                        },
                        Empty(),
                        onlineStats = new OnlineStatsColumn(),
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

                updateObjectCounts();
                fetchOnlineStats();
            }, true);

            BeatmapSetInfo.BindValueChanged(s =>
            {
                if (s.NewValue is APIBeatmapSet onlineSetInfo)
                    onlineStats.BeatmapSetInfo.Value = onlineSetInfo;
            }, true);

            apiState.BindTo(api.State);
            apiState.BindValueChanged(state => fetchOnlineStats(), true);

            game.Ruleset.BindValueChanged(_ => updateObjectCounts());
        }

        private void updateObjectCounts()
        {
            IRulesetInfo ruleset = BeatmapInfo.Value is APIBeatmap onlineInfo ? rulesetStore.GetRuleset(onlineInfo.Ruleset.OnlineID)! : game.Ruleset.Value;

            try
            {
                var localBeatmap = workingBeatmap.Value.GetPlayableBeatmap(ruleset);

                objectCountFlow.Children = ruleset.CreateInstance()
                                                  .GetObjectCount(localBeatmap.HitObjects)
                                                  .Select(s => new PillCountStatistic(s))
                                                  .ToArray();
            }
            catch (BeatmapInvalidForRulesetException)
            {
                // TODO: explain this better or remove it
                // suppress BeatmapInvalidForRulesetException when ruleset is changing but beatmap is not
            }

            if (BeatmapInfo.Value is APIBeatmap beatmap)
            {
                // TODO: kinda hacky
                objectCountFlow.Children[0].Value = beatmap.CircleCount.ToString();
                objectCountFlow.Children[1].Value = beatmap.SliderCount.ToString();

                if (objectCountFlow.Children.ElementAtOrDefault(2) != null)
                    objectCountFlow.Children.ElementAtOrDefault(2)!.Value = beatmap.SpinnerCount.ToString();
            }
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

        private partial class PillCountStatistic : GridContainer
        {
            private readonly OsuSpriteText valueText;

            public LocalisableString Value
            {
                set => valueText.Text = value;
            }

            public PillCountStatistic(BeatmapStatistic statistic)
            {
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.CentreLeft;
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 10),
                    new Dimension(GridSizeMode.AutoSize),
                };
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                };

                Content = new[]
                {
                    new[]
                    {
                        new OsuTextFlowContainer(t => t.Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12))
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Text = statistic.Name,
                        },
                        Empty(),
                        valueText = new OsuSpriteText
                        {
                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                            Text = statistic.Content,
                        }
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
                valueText.Colour = colourProvider.Content2;
            }
        }

        private partial class ColumnDivider : CompositeDrawable
        {
            public ColumnDivider()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
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
