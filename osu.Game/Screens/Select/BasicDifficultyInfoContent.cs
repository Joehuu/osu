// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.BeatmapSetV2;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.Play.HUD;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class BasicDifficultyInfoContent : Container
    {
        public const float WEDGE_HEIGHT = 100;
        public const float SHEAR_WIDTH = SongSelect.SHEAR_X * WEDGE_HEIGHT;

        private OsuSpriteText difficultyName = null!;
        private LinkFlowContainer creatorName = null!;
        private PillStatistic lengthStatistic = null!;
        private PillStatistic bpmStatistic = null!;

        public readonly Bindable<IBeatmapInfo> BeatmapInfo = new Bindable<IBeatmapInfo>();
        public readonly Bindable<IBeatmapSetInfo?> BeatmapSetInfo = new Bindable<IBeatmapSetInfo?>();
        private ArgonSongProgressGraph graph = null!;
        private BeatmapFailRateBar failRate = null!;
        private Container densityGraphContainer = null!;
        private OsuSpriteText densityText = null!;

        [Resolved]
        private Bindable<RulesetInfo> ruleset { get; set; } = null!;

        [Resolved]
        private Bindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

        public BasicDifficultyInfoContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider, OsuColour colours)
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
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            ColumnDimensions = new[]
                            {
                                new Dimension(),
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.Absolute, 10),
                                new Dimension(GridSizeMode.AutoSize),
                            },
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new[]
                                {
                                    difficultyName = new TruncatingSpriteText
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold),
                                    },
                                    new OsuSpriteText
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        // todo: better null display
                                        Text = " mapped by ",
                                        Font = OsuFont.GetFont(size: 14),
                                    },
                                    creatorName = new LinkFlowContainer(t => t.Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14))
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        AutoSizeAxes = Axes.Both,
                                    },
                                    Empty(),
                                    new Container
                                    {
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        AutoSizeAxes = Axes.X,
                                        Height = 20,
                                        CornerRadius = 10,
                                        Masking = true,
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = colourProvider.Background4
                                            },
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.X,
                                                RelativeSizeAxes = Axes.Y,
                                                Direction = FillDirection.Horizontal,
                                                Spacing = new Vector2(10),
                                                Padding = new MarginPadding { Horizontal = 20 },
                                                Children = new Drawable[]
                                                {
                                                    lengthStatistic = new PillStatistic(new BeatmapStatistic { Name = "Length" })
                                                    {
                                                        Value = "-"
                                                    },
                                                    bpmStatistic = new PillStatistic(new BeatmapStatistic { Name = BeatmapsetsStrings.ShowStatsBpm })
                                                    {
                                                        Value = "-"
                                                    },
                                                }
                                            },
                                        }
                                    }
                                },
                            }
                        },
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            ColumnDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize, minSize: 70),
                                new Dimension(GridSizeMode.Absolute, 10),
                            },
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.Absolute, 10),
                                new Dimension(GridSizeMode.AutoSize),
                            },
                            Content = new[]
                            {
                                new[]
                                {
                                    densityText = new OsuSpriteText
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Text = "Density",
                                        Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold),
                                    },
                                    Empty(),
                                    densityGraphContainer = new Container
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.X,
                                        Height = 10,
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
                                    failRate = new BeatmapFailRateBar
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.X,
                                        Height = 10f,
                                    },
                                }
                            }
                        }
                    },
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            BeatmapInfo.BindValueChanged(b =>
            {
                if (b.NewValue == null) return;

                difficultyName.Text = b.NewValue.DifficultyName;

                lengthStatistic.Value = b.NewValue.Length.ToFormattedDuration();

                if (b.NewValue is APIBeatmap beatmap)
                {
                    failRate.Current.Value = beatmap;

                    // TODO: pending web
                    densityText.Hide();
                    densityGraphContainer.Hide();
                    lengthStatistic.TooltipText = BeatmapsetsStrings.ShowStatsTotalLength(beatmap.HitLength.ToFormattedDuration());
                    bpmStatistic.Value = b.NewValue.BPM.ToLocalisableString(@"0.##");
                    return;
                }

                updateBPM();

                var playableBeatmap = workingBeatmap.Value.GetPlayableBeatmap(ruleset.Value);
                lengthStatistic.TooltipText = BeatmapsetsStrings.ShowStatsTotalLength(playableBeatmap.CalculateDrainLength().ToFormattedDuration());

                // local only at this point

                // TODO: not sure if it should be done like this
                graph.Objects = ruleset.Value.CreateInstance().CreateDrawableRulesetWith(playableBeatmap).Objects;

                fetchPointsOfFailure();
            }, true);

            BeatmapSetInfo.BindValueChanged(s =>
            {
                creatorName.Clear();
                if (s.NewValue != null) creatorName.AddUserLink(s.NewValue.Metadata.Author);
            }, true);

            mods.BindValueChanged(_ => updateBPM());
        }

        private void updateBPM()
        {
            if (BeatmapInfo.Value is APIBeatmap) return;

            // this doesn't consider mods which apply variable rates, yet.
            double rate = 1;
            foreach (var mod in mods.Value.OfType<IApplicableToRate>())
                rate = mod.ApplyToRate(0, rate);

            var beatmap = workingBeatmap.Value.Beatmap;

            int bpmMax = (int)Math.Round(Math.Round(beatmap.ControlPointInfo.BPMMaximum) * rate);
            int bpmMin = (int)Math.Round(Math.Round(beatmap.ControlPointInfo.BPMMinimum) * rate);
            int mostCommonBPM = (int)Math.Round(Math.Round(60000 / beatmap.GetMostCommonBeatLength()) * rate);

            string labelText = bpmMin == bpmMax
                ? $"{bpmMin}"
                : $"{bpmMin}-{bpmMax} (mostly {mostCommonBPM})";

            bpmStatistic.Value = labelText;
        }

        private void fetchPointsOfFailure()
        {
            var requestedBeatmap = BeatmapInfo.Value;

            var lookup = new GetBeatmapRequest(requestedBeatmap);

            lookup.Success += res =>
            {
                Schedule(() =>
                {
                    if (BeatmapInfo.Value != requestedBeatmap)
                        // the beatmap has been changed since we started the lookup.
                        return;

                    failRate.Current.Value = res;
                });
            };

            api.Queue(lookup);
        }

        private partial class PillStatistic : FillFlowContainer, IHasTooltip
        {
            private readonly OsuSpriteText valueText;

            public LocalisableString Value
            {
                set => valueText.Text = value;
            }

            public PillStatistic(BeatmapStatistic statistic)
            {
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.CentreLeft;
                AutoSizeAxes = Axes.Both;
                Direction = FillDirection.Horizontal;
                Spacing = new Vector2(5);

                Children = new Drawable[]
                {
                    new OsuSpriteText
                    {
                        Text = statistic.Name,
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                    },
                    valueText = new OsuSpriteText
                    {
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                        Text = statistic.Content
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
                valueText.Colour = colourProvider.Content2;
            }

            public LocalisableString TooltipText { get; set; }
        }
    }
}
