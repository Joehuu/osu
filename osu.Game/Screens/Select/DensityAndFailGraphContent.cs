// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSetV2;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Screens.Play.HUD;

namespace osu.Game.Screens.Select
{
    public partial class DensityAndFailGraphContent : CompositeDrawable
    {
        private const float bar_height = 10;

        private ArgonSongProgressGraph graph = null!;
        private BeatmapFailRateBar failRate = null!;
        private Container densityGraphContainer = null!;
        private OsuSpriteText graphNotAvailableText = null!;
        private BarLoadingLayer loading = null!;

        [Resolved]
        private IBindable<IBeatmapInfo?> beatmapInfo { get; set; } = null!;

        [Resolved]
        private IBindable<APIBeatmap?> apiBeatmap { get; set; } = null!;

        [Resolved]
        private Bindable<RulesetInfo> ruleset { get; set; } = null!;

        [Resolved]
        private Bindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        private readonly IBindable<APIState> apiState = new Bindable<APIState>();

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        public DensityAndFailGraphContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChild = new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize, minSize: 70),
                    new Dimension(GridSizeMode.Absolute, bar_height),
                },
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension(GridSizeMode.Absolute, bar_height),
                    new Dimension(GridSizeMode.AutoSize),
                },
                Content = new[]
                {
                    new[]
                    {
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = "Density",
                            Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold),
                        },
                        Empty(),
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                graphNotAvailableText = new OsuSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = "N/A",
                                    Alpha = 0,
                                    Font = OsuFont.GetFont(size: 12)
                                },
                                densityGraphContainer = new Container
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    RelativeSizeAxes = Axes.X,
                                    Height = bar_height,
                                    Masking = true,
                                    CornerRadius = 5,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = colours.Blue,
                                        },
                                        graph = new ArgonSongProgressGraph
                                        {
                                            Name = @"Difficulty graph",
                                            RelativeSizeAxes = Axes.Both,
                                        },
                                    }
                                }
                            }
                        }
                    },
                    null,
                    new[]
                    {
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = BeatmapsetsStrings.ShowInfoPointsOfFailure,
                            Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold),
                        },
                        Empty(),
                        new Container
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.X,
                            Height = bar_height,
                            Children = new Drawable[]
                            {
                                failRate = new BeatmapFailRateBar
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                loading = new BarLoadingLayer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                            }
                        }
                    },
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmapInfo.BindValueChanged(_ => updateInfo(), true);

            apiState.BindTo(api.State);
            apiState.BindValueChanged(_ => loading.Show(), true);

            apiBeatmap.BindValueChanged(b =>
            {
                failRate.Current.Value = b.NewValue;

                failRate.FadeTo(b.NewValue != null ? 1 : 0.25f, 250, Easing.OutQuint);

                if (b.NewValue != null)
                    Schedule(() => loading.Hide());
            }, true);
        }

        private void updateInfo()
        {
            // needed for cases where `ruleset` and `beatmapInfo` change at the same time.
            Scheduler.AddOnce(() =>
            {
                loading.Show();

                if (beatmapInfo.Value is APIBeatmap beatmap)
                {
                    failRate.Current.Value = beatmap;

                    // TODO: pending web support
                    densityGraphContainer.Hide();
                    graphNotAvailableText.Show();
                    return;
                }

                graphNotAvailableText.Hide();
                densityGraphContainer.Show();

                var playableBeatmap = workingBeatmap.Value.GetPlayableBeatmap(ruleset.Value);

                // TODO: not sure if it should be done like this
                graph.Objects = ruleset.Value.CreateInstance().CreateDrawableRulesetWith(playableBeatmap).Objects;
                densityGraphContainer.FadeTo(graph.Values.Any() ? 1 : 0.25f, 250, Easing.OutQuint);
            });
        }

        private partial class BarLoadingLayer : LoadingLayer
        {
            public BarLoadingLayer()
                : base(dimBackground: true)
            {
                BackgroundDimLayer!.Masking = true;
                BackgroundDimLayer.CornerRadius = bar_height / 2;
            }
        }
    }
}
