// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets;

namespace osu.Game.Screens.Select
{
    public partial class BeatmapInfoWedgeV2 : VisibilityContainer
    {
        public const float WEDGE_HEIGHT = 120;
        public const float SHEAR_WIDTH = SongSelect.SHEAR_X * WEDGE_HEIGHT;
        private const float transition_duration = 250;
        public const float COLOUR_BAR_WIDTH = 30;

        public readonly BindableBool ShowDetails = new BindableBool(true);

        [Resolved]
        private IBindable<RulesetInfo> ruleset { get; set; } = null!;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        [Resolved]
        private BeatmapDifficultyCache difficultyCache { get; set; } = null!;

        protected Container? DisplayedContent { get; private set; }

        protected WedgeInfoText? Info { get; private set; }

        private Container difficultyColourBar = null!;
        private StarCounter starCounter = null!;
        private StarRatingDisplay starRatingDisplay = null!;
        private BeatmapSetOnlineStatusPill statusPill = null!;
        private Container content = null!;

        private IBindable<StarDifficulty?>? starDifficulty;
        private CancellationTokenSource? cancellationSource;

        public BeatmapInfoWedgeV2()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = WEDGE_HEIGHT,
                        Shear = SongSelect.WEDGED_CONTAINER_SHEAR,
                        Masking = true,
                        Padding = new MarginPadding { Left = -SongSelect.WEDGE_CORNER_RADIUS },
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Colour = Colour4.Black.Opacity(0.2f),
                            Type = EdgeEffectType.Shadow,
                            Radius = 3,
                        },
                        CornerRadius = SongSelect.WEDGE_CORNER_RADIUS,

                        Children = new Drawable[]
                        {
                            // These elements can't be grouped with the rest of the content, due to being present either outside or under the backgrounds area
                            difficultyColourBar = new Container
                            {
                                Colour = Colour4.Transparent,
                                Depth = float.MaxValue,
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                RelativeSizeAxes = Axes.Y,

                                // By limiting the width we avoid this box showing up as an outline around the drawables that are on top of it.
                                Width = COLOUR_BAR_WIDTH + SongSelect.WEDGE_CORNER_RADIUS,
                                Child = new Box { RelativeSizeAxes = Axes.Both }
                            },
                            new Container
                            {
                                // Applying the shear to this container and nesting the starCounter inside avoids
                                // the deformation that occurs if the shear is applied to the starCounter whilst rotated
                                Shear = -SongSelect.WEDGED_CONTAINER_SHEAR,
                                X = -COLOUR_BAR_WIDTH / 2,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Y,
                                Width = COLOUR_BAR_WIDTH,
                                Child = starCounter = new StarCounter
                                {
                                    Rotation = (float)(Math.Atan(SongSelect.SHEAR_X) * (180 / Math.PI)),
                                    Colour = Colour4.Transparent,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Scale = new Vector2(0.35f),
                                    Direction = FillDirection.Vertical
                                }
                            },
                            new FillFlowContainer
                            {
                                Name = "Topright-aligned metadata",
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Top = 3, Right = COLOUR_BAR_WIDTH + 8 },
                                AutoSizeAxes = Axes.Both,
                                Spacing = new Vector2(0, 5),
                                Depth = float.MinValue,
                                Children = new Drawable[]
                                {
                                    starRatingDisplay = new StarRatingDisplay(default, animated: true)
                                    {
                                        Anchor = Anchor.TopRight,
                                        Origin = Anchor.TopRight,
                                        Shear = -SongSelect.WEDGED_CONTAINER_SHEAR,
                                        Alpha = 0,
                                    },
                                    statusPill = new BeatmapSetOnlineStatusPill
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Anchor = Anchor.TopRight,
                                        Origin = Anchor.TopRight,
                                        Shear = -SongSelect.WEDGED_CONTAINER_SHEAR,
                                        TextSize = 11,
                                        TextPadding = new MarginPadding { Horizontal = 8, Vertical = 2 },
                                        Alpha = 0,
                                    }
                                }
                            },
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        AutoSizeDuration = 500,
                        AutoSizeEasing = Easing.OutQuint,
                        Child = basicDifficultyInfoWedge = new DetailsWedgeContainer
                        {
                            Padding = new MarginPadding
                            {
                                Top = 10, Left = -SongSelect.WEDGE_CORNER_RADIUS, Right = SHEAR_WIDTH + COLOUR_BAR_WIDTH
                            },
                            Child = new BasicDifficultyInfoContent
                            {
                                Padding = new MarginPadding { Left = SongSelect.TEXT_MARGIN, Right = 30, Vertical = 10 }
                            },
                        },
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        AutoSizeDuration = 500,
                        AutoSizeEasing = Easing.OutQuint,
                        Child = extendedDifficultyInfoWedge = new DetailsWedgeContainer
                        {
                            Padding = new MarginPadding
                            {
                                Top = 10, Left = -SongSelect.WEDGE_CORNER_RADIUS, Right = SHEAR_WIDTH + COLOUR_BAR_WIDTH + BasicDifficultyInfoContent.SHEAR_WIDTH
                            },
                            Child = new ExtendedBeatmapDetailsContent
                            {
                                Padding = new MarginPadding { Left = SongSelect.TEXT_MARGIN, Right = 50, Vertical = 10 }
                            },
                        },
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ruleset.BindValueChanged(_ => updateDisplay());

            starRatingDisplay.Current.BindValueChanged(s =>
            {
                // use actual stars as star counter has its own animation
                starCounter.Current = (float)s.NewValue.Stars;
            }, true);

            starRatingDisplay.DisplayedStars.BindValueChanged(s =>
            {
                // sync color with star rating display
                starCounter.Colour = s.NewValue >= 6.5 ? colours.Orange1 : Colour4.Black.Opacity(0.75f);
                difficultyColourBar.FadeColour(colours.ForStarDifficulty(s.NewValue));
            }, true);

            ShowDetails.BindValueChanged(e =>
            {
                if (e.NewValue)
                {
                    basicDifficultyInfoWedge.Show();
                    extendedDifficultyInfoWedge.Show();
                }
                else
                {
                    basicDifficultyInfoWedge.Hide();
                    extendedDifficultyInfoWedge.Hide();
                }
            }, true);

            beatmap.BindValueChanged(b =>
            {
                updateDisplay();
            }, true);

            Show();
        }

        private const double animation_duration = 600;

        protected override void PopIn()
        {
            this.MoveToX(0, animation_duration, Easing.OutQuint);
            this.FadeIn(200, Easing.In);
        }

        protected override void PopOut()
        {
            this.MoveToX(-150, animation_duration, Easing.OutQuint);
            this.FadeOut(200, Easing.OutQuint);
        }

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

        private Container? loadingInfo;
        private DetailsWedgeContainer basicDifficultyInfoWedge = null!;
        private DetailsWedgeContainer extendedDifficultyInfoWedge = null!;

        private void updateDisplay()
        {
            statusPill.Status = beatmap.Value.BeatmapInfo.Status;

            starDifficulty = difficultyCache.GetBindableDifficulty(beatmap.Value.BeatmapInfo, (cancellationSource = new CancellationTokenSource()).Token);

            starDifficulty.BindValueChanged(s =>
            {
                starRatingDisplay.Current.Value = s.NewValue ?? default;

                starRatingDisplay.FadeIn(transition_duration);
            });

            Scheduler.AddOnce(() =>
            {
                LoadComponentAsync(loadingInfo = new Container
                {
                    Padding = new MarginPadding { Right = COLOUR_BAR_WIDTH },
                    RelativeSizeAxes = Axes.Both,
                    Depth = DisplayedContent?.Depth + 1 ?? 0,
                    Child = new Container
                    {
                        Masking = true,
                        CornerRadius = SongSelect.WEDGE_CORNER_RADIUS,
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            // TODO: New wedge design uses a coloured horizontal gradient for its background, however this lacks implementation information in the figma draft.
                            // pending https://www.figma.com/file/DXKwqZhD5yyb1igc3mKo1P?node-id=2980:3361#340801912 being answered.
                            new BeatmapInfoWedgeBackground(beatmap.Value) { Shear = -SongSelect.WEDGED_CONTAINER_SHEAR },
                            Info = new WedgeInfoText(beatmap.Value) { Shear = -SongSelect.WEDGED_CONTAINER_SHEAR }
                        }
                    }
                }, d =>
                {
                    // Ensure we are the most recent loaded wedge.
                    if (d != loadingInfo) return;

                    removeOldInfo();
                    content.Add(DisplayedContent = d);
                });
            });

            void removeOldInfo()
            {
                DisplayedContent?.FadeOut(transition_duration);
                DisplayedContent?.Expire();
                DisplayedContent = null;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            cancellationSource?.Cancel();
        }

        public partial class WedgeInfoText : Container
        {
            public OsuSpriteText TitleLabel { get; private set; } = null!;
            public OsuSpriteText ArtistLabel { get; private set; } = null!;

            private readonly WorkingBeatmap working;

            public WedgeInfoText(WorkingBeatmap working)
            {
                this.working = working;

                RelativeSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(SongSelect? songSelect, LocalisationManager localisation)
            {
                var metadata = working.Metadata;

                var titleText = new RomanisableString(metadata.TitleUnicode, metadata.Title);
                var artistText = new RomanisableString(metadata.ArtistUnicode, metadata.Artist);

                Child = new FillFlowContainer
                {
                    Name = "Top-left aligned metadata",
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Left = SongSelect.TEXT_MARGIN, Vertical = 12 },
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new OsuHoverContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Action = () => songSelect?.Search(titleText.GetPreferred(localisation.CurrentParameters.Value.PreferOriginalScript)),
                            Child = TitleLabel = new TruncatingSpriteText
                            {
                                Shadow = true,
                                Text = titleText,
                                Font = OsuFont.TorusAlternate.With(size: 40, weight: FontWeight.SemiBold),
                            },
                        },
                        new OsuHoverContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Action = () => songSelect?.Search(artistText.GetPreferred(localisation.CurrentParameters.Value.PreferOriginalScript)),
                            Child = ArtistLabel = new TruncatingSpriteText
                            {
                                // TODO : figma design has a diffused shadow, instead of the solid one present here, not possible currently as far as i'm aware.
                                Shadow = true,
                                Text = artistText,
                                // Not sure if this should be semi bold or medium
                                Font = OsuFont.Torus.With(size: 20, weight: FontWeight.SemiBold),
                            },
                        },
                    }
                };
            }

            protected override void UpdateAfterChildren()
            {
                base.UpdateAfterChildren();

                // best effort to confine the auto-sized text to wedge bounds
                TitleLabel.MaxWidth = DrawWidth - SongSelect.TEXT_MARGIN * 2 - SongSelect.SHEAR_X * DrawHeight;
                ArtistLabel.MaxWidth = DrawWidth - SongSelect.TEXT_MARGIN - SongSelect.SHEAR_X * DrawHeight;
            }
        }
    }
}
