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
using osu.Framework.Threading;
using osu.Game.Configuration;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osuTK;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class DifficultySettingsColumn : FillFlowContainer
    {
        [Resolved]
        private BeatmapDifficultyCache difficultyCache { get; set; } = null!;

        [Resolved]
        private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

        [Resolved]
        private OsuGameBase game { get; set; } = null!;

        public Bindable<IBeatmapInfo?> BeatmapInfo { get; set; } = new Bindable<IBeatmapInfo?>();

        private IBindable<RulesetInfo> gameRuleset = null!;

        private DifficultySettingRow firstValue = null!;
        private DifficultySettingRow hpDrain = null!;
        private DifficultySettingRow accuracy = null!;
        private DifficultySettingRow approachRate = null!;

        public DifficultySettingsColumn()
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
                firstValue = new DifficultySettingRow(), // circle size/key amount
                hpDrain = new DifficultySettingRow { Title = BeatmapsetsStrings.ShowStatsDrain },
                accuracy = new DifficultySettingRow { Title = BeatmapsetsStrings.ShowStatsAccuracy },
                approachRate = new DifficultySettingRow { Title = BeatmapsetsStrings.ShowStatsAr },
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

            BeatmapInfo.BindValueChanged(_ => updateStatistics(), true);

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
            IBeatmapDifficultyInfo? baseDifficulty = BeatmapInfo.Value?.Difficulty;
            BeatmapDifficulty? adjustedDifficulty = null;

            if (baseDifficulty != null && mods.Value.Any(m => m is IApplicableToDifficulty))
            {
                adjustedDifficulty = new BeatmapDifficulty(baseDifficulty);

                foreach (var mod in mods.Value.OfType<IApplicableToDifficulty>())
                    mod.ApplyToDifficulty(adjustedDifficulty);
            }

            switch (BeatmapInfo.Value?.Ruleset.OnlineID)
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
    }
}
