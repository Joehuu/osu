// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Framework.Bindables;
using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using System.Linq;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Threading;
using osu.Game.Configuration;
using osu.Game.Overlays.Mods;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osuTK;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class DifficultySettingsContent : FillFlowContainer, IHasCustomTooltip<AdjustedAttributesTooltip.Data>
    {
        [Resolved]
        private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

        [Resolved]
        private OsuGameBase game { get; set; } = null!;

        [Resolved]
        private IBindable<IBeatmapInfo?> beatmapInfo { get; set; } = null!;

        private IBindable<RulesetInfo> gameRuleset = null!;

        private BarStatisticRow firstValue = null!;
        private BarStatisticRow hpDrain = null!;
        private BarStatisticRow accuracy = null!;
        private BarStatisticRow approachRate = null!;

        public DifficultySettingsContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(3);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new[]
            {
                firstValue = new BarStatisticRow(), // circle size/key amount
                hpDrain = new BarStatisticRow { Title = BeatmapsetsStrings.ShowStatsDrain },
                accuracy = new BarStatisticRow { Title = BeatmapsetsStrings.ShowStatsAccuracy },
                approachRate = new BarStatisticRow { Title = BeatmapsetsStrings.ShowStatsAr },
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

            beatmapInfo.BindValueChanged(_ => updateStatistics(), true);

            mods.BindValueChanged(modsChanged);
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

        private void updateStatistics()
        {
            IBeatmapDifficultyInfo? baseDifficulty = beatmapInfo.Value?.Difficulty;
            BeatmapDifficulty? adjustedDifficulty = null;

            IRulesetInfo ruleset = gameRuleset.Value ?? beatmapInfo.Value!.Ruleset;

            if (baseDifficulty != null)
            {
                BeatmapDifficulty originalDifficulty = new BeatmapDifficulty(baseDifficulty);

                foreach (var mod in mods.Value.OfType<IApplicableToDifficulty>())
                    mod.ApplyToDifficulty(originalDifficulty);

                double rate = 1;
                foreach (var mod in mods.Value.OfType<IApplicableToRate>())
                    rate = mod.ApplyToRate(0, rate);

                adjustedDifficulty = ruleset.CreateInstance().GetRateAdjustedDisplayDifficulty(originalDifficulty, rate);

                TooltipContent = new AdjustedAttributesTooltip.Data(originalDifficulty, adjustedDifficulty);
            }

            switch (ruleset.OnlineID)
            {
                case 3:
                    // Account for mania differences locally for now
                    // Eventually this should be handled in a more modular way, allowing rulesets to return arbitrary difficulty attributes
                    firstValue.Title = BeatmapsetsStrings.ShowStatsCsMania;
                    firstValue.Value = (baseDifficulty?.CircleSize ?? 0, null);
                    break;

                default:
                    firstValue.Title = BeatmapsetsStrings.ShowStatsCs;
                    firstValue.Value = (baseDifficulty?.CircleSize ?? 0, adjustedDifficulty?.CircleSize);
                    break;
            }

            hpDrain.Value = (baseDifficulty?.DrainRate ?? 0, adjustedDifficulty?.DrainRate);
            accuracy.Value = (baseDifficulty?.OverallDifficulty ?? 0, adjustedDifficulty?.OverallDifficulty);
            approachRate.Value = (baseDifficulty?.ApproachRate ?? 0, adjustedDifficulty?.ApproachRate);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            modSettingChangeTracker?.Dispose();
        }

        public ITooltip<AdjustedAttributesTooltip.Data> GetCustomTooltip() => new AdjustedAttributesTooltip();

        public AdjustedAttributesTooltip.Data TooltipContent { get; private set; } = null!;
    }
}
