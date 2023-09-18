// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Beatmaps;
using osu.Framework.Bindables;
using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Extensions;
using osu.Framework.Threading;
using osu.Framework.Utils;
using osu.Game.Configuration;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Overlays.Mods;

namespace osu.Game.Screens.Select.Details
{
    public partial class AdvancedStats : Container, IHasCustomTooltip<AdjustedAttributesTooltip.Data>
    {
        [Resolved]
        private BeatmapDifficultyCache difficultyCache { get; set; } = null!;

        [Resolved]
        private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

        [Resolved]
        private OsuGameBase game { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        [Resolved]
        private IBindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        private IBindable<RulesetInfo>? gameRuleset;

        private IRulesetInfo? rulesetInfoWhenUpdated;

        private Ruleset ruleset = null!;

        private StatisticRow starDifficulty = null!;

        public ITooltip<AdjustedAttributesTooltip.Data> GetCustomTooltip() => new AdjustedAttributesTooltip();
        public AdjustedAttributesTooltip.Data TooltipContent { get; private set; } = null!;

        private IBeatmapInfo? beatmapInfo;

        public IBeatmapInfo? BeatmapInfo
        {
            get => beatmapInfo;
            set
            {
                if (value == beatmapInfo) return;

                beatmapInfo = value;

                updateStatistics();
            }
        }

        private readonly FillFlowContainer settingsFlow;

        public AdvancedStats(int columns = 1)
        {
            this.columns = columns;

            Child = settingsFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // the cached ruleset bindable might be a decoupled bindable provided by SongSelect,
            // which we can't rely on in combination with the game-wide selected mods list,
            // since mods could be updated to the new ruleset instances while the decoupled bindable is held behind,
            // therefore resulting in performing difficulty calculation with invalid states.
            gameRuleset = game.Ruleset.GetBoundCopy();
            gameRuleset.BindValueChanged(_ => updateStatistics());

            mods.BindValueChanged(modsChanged, true);
        }

        private ModSettingChangeTracker? modSettingChangeTracker;
        private ScheduledDelegate? debouncedStatisticsUpdate;

        private void modsChanged(ValueChangedEvent<IReadOnlyList<Mod>> mods)
        {
            modSettingChangeTracker?.Dispose();

            modSettingChangeTracker = new ModSettingChangeTracker(mods.NewValue);
            modSettingChangeTracker.SettingChanged += _ =>
            {
                debouncedStatisticsUpdate?.Cancel();
                debouncedStatisticsUpdate = Scheduler.AddDelayed(updateStatistics, 100);
            };

            updateStatistics();
        }

        private void updateStatistics() => Scheduler.AddOnce(() =>
        {
            if (BeatmapInfo == null || gameRuleset == null) return;

            IRulesetInfo rulesetInfo = BeatmapInfo is APIBeatmap ? BeatmapInfo.Ruleset : gameRuleset.Value;

            if (rulesetInfoWhenUpdated?.OnlineID != rulesetInfo.OnlineID)
                ruleset = rulesets.GetRuleset(rulesetInfo.OnlineID)!.CreateInstance();

            List<BeatmapDifficultySetting> difficultySettings;

            if (BeatmapInfo is APIBeatmap)
                difficultySettings = ruleset.GetDifficultySettings(BeatmapInfo.Difficulty).ToList();
            else
            {
                var localBeatmap = workingBeatmap.Value.Beatmap;

                BeatmapDifficulty difficulty = new BeatmapDifficulty(BeatmapInfo.Difficulty);

                foreach (var mod in mods.Value.OfType<IApplicableToDifficulty>())
                    mod.ApplyToDifficulty(difficulty);

                var adjustedConverter = ruleset.CreateBeatmapConverter(localBeatmap);

                foreach (var mod in mods.Value.OfType<IApplicableToBeatmapConverter>())
                    mod.ApplyToBeatmapConverter(adjustedConverter);

                difficultySettings = ruleset
                                     .GetDifficultySettings(localBeatmap.Difficulty, difficulty, adjustedConverter)
                                     .ToList();
            }

            if (rulesetInfoWhenUpdated?.OnlineID != rulesetInfo.OnlineID)
            {
                // only reconstruct difficulty settings if the ruleset changed
                settingsFlow.Children = difficultySettings.Select(s => new StatisticRow(s)
                {
                    Width = 1f / columns,
                    Padding = new MarginPadding { Horizontal = columns == 2 ? 5 : 0, Vertical = 2.5f },
                }).ToList();

                settingsFlow.Add(starDifficulty = new StatisticRow(new BeatmapDifficultySetting
                {
                    Name = BeatmapsetsStrings.ShowStatsStars,
                }, forceDecimalPlaces: true)
                {
                    Width = 1f / columns,
                    Padding = new MarginPadding { Horizontal = columns == 2 ? 5 : 0, Vertical = 2.5f },
                    AccentColour = colours.Yellow
                });
            }
            else
            {
                for (int i = 0; i < difficultySettings.Count; i++)
                {
                    ((StatisticRow)settingsFlow[i]).Value = difficultySettings[i].Value;
                }
            }

            updateStarDifficulty();

            rulesetInfoWhenUpdated = rulesetInfo;
        });

        private CancellationTokenSource? starDifficultyCancellationSource;
        private readonly int columns;

        /// <summary>
        /// Updates the displayed star difficulty statistics with the values provided by the currently-selected beatmap, ruleset, and selected mods.
        /// </summary>
        /// <remarks>
        /// This is scheduled to avoid scenarios wherein a ruleset changes first before selected mods do,
        /// potentially resulting in failure during difficulty calculation due to incomplete bindable state updates.
        /// </remarks>
        private void updateStarDifficulty() => Scheduler.AddOnce(() =>
        {
            starDifficultyCancellationSource?.Cancel();

            if (BeatmapInfo == null)
                return;

            starDifficultyCancellationSource = new CancellationTokenSource();

            var normalStarDifficultyTask = difficultyCache.GetDifficultyAsync(BeatmapInfo, gameRuleset?.Value, null, starDifficultyCancellationSource.Token);
            var moddedStarDifficultyTask = difficultyCache.GetDifficultyAsync(BeatmapInfo, gameRuleset?.Value, mods.Value, starDifficultyCancellationSource.Token);

            Task.WhenAll(normalStarDifficultyTask, moddedStarDifficultyTask).ContinueWith(_ => Schedule(() =>
            {
                var normalDifficulty = normalStarDifficultyTask.GetResultSafely();
                var moddedDifficulty = moddedStarDifficultyTask.GetResultSafely();

                if (normalDifficulty == null || moddedDifficulty == null)
                    return;

                starDifficulty.Value = ((float)normalDifficulty.Value.Stars, (float)moddedDifficulty.Value.Stars);
            }), starDifficultyCancellationSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            modSettingChangeTracker?.Dispose();
            starDifficultyCancellationSource?.Cancel();
        }

        public partial class StatisticRow : Container, IHasAccentColour
        {
            private const float value_width = 25;
            private const float name_width = 70;

            private readonly float maxValue;
            private readonly bool forceDecimalPlaces;
            private readonly OsuSpriteText valueText;
            private readonly Bar bar;
            public readonly Bar ModBar;

            [Resolved]
            private OsuColour colours { get; set; } = null!;

            private (float baseValue, float? adjustedValue)? value;
            private readonly BeatmapDifficultySetting statistic;

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

            public StatisticRow(BeatmapDifficultySetting statistic, float maxValue = 10, bool forceDecimalPlaces = false)
            {
                this.statistic = statistic;
                this.maxValue = maxValue;
                this.forceDecimalPlaces = forceDecimalPlaces;
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Padding = new MarginPadding { Vertical = 2.5f };

                Children = new Drawable[]
                {
                    new Container
                    {
                        Width = name_width,
                        AutoSizeAxes = Axes.Y,
                        // osu-web uses 1.25 line-height, which at 12px font size makes the element 14px tall - this compentates that difference
                        Padding = new MarginPadding { Vertical = 1 },
                        Child = new OsuSpriteText
                        {
                            Text = statistic.Name,
                            Font = OsuFont.GetFont(size: 12)
                        },
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Left = name_width + 10, Right = value_width + 10 },
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.X,
                                Height = 5,

                                CornerRadius = 2,
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    bar = new Bar
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        BackgroundColour = Color4.White.Opacity(0.5f),
                                    },
                                    ModBar = new Bar
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Alpha = 0.5f,
                                    },
                                }
                            },
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Width = value_width,
                        RelativeSizeAxes = Axes.Y,
                        Child = valueText = new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = OsuFont.GetFont(size: 12)
                        },
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Value = statistic.Value;
            }
        }
    }
}
