// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Skinning;

namespace osu.Game.Screens.SelectV2.Wedge
{
    public partial class LocalObjectCountContent : ObjectCountContent, ISerialisableDrawable
    {
        private IRulesetInfo? lastRulesetWhenUpdated;

        [Resolved]
        private IBindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        [Resolved]
        private IBindable<RulesetInfo> ruleset { get; set; } = null!;

        [Resolved]
        private BeatmapManager beatmaps { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            workingBeatmap.BindValueChanged(_ => updateObjectCounts());

            ruleset.BindValueChanged(_ => updateObjectCounts(), true);
        }

        private void updateObjectCounts()
        {
            // needed for cases where ruleset and beatmapInfo change at the same time.
            Scheduler.AddOnce(() =>
            {
                if (ruleset.Value == null) return;

                var playableBeatmap = workingBeatmap.Value.GetPlayableBeatmap(ruleset.Value);
                var currentStatistics = playableBeatmap.GetStatistics().ToArray();
                List<BeatmapStatistic[]> allStatistics = new List<BeatmapStatistic[]>();

                foreach (var beatmap in workingBeatmap.Value.BeatmapSetInfo.Beatmaps)
                {
                    try
                    {
                        allStatistics.Add(beatmaps.GetWorkingBeatmap(beatmap).GetPlayableBeatmap(ruleset.Value).GetStatistics().ToArray());
                    }
                    catch
                    {
                    }
                }

                List<int> maxStatistics = allStatistics
                                          .SelectMany(list => list.Select((value, index) => new { value, index }))
                                          .GroupBy(item => item.index)
                                          .Select(group => group.Max(item => int.Parse(item.value.Content)))
                                          .ToList();

                // only reconstruct children when the ruleset changes
                // so that the existing `BarStatisticRow`s can animate to the new value.
                if (lastRulesetWhenUpdated?.OnlineID != ruleset.Value.OnlineID)
                {
                    // TODO: find way of getting the object counts from the hardest difficulty, set 1000 as max value for now
                    Flow.Children = currentStatistics.Select((s, i) =>
                                                         new BarStatisticRow
                                                         {
                                                             Title = s.Name,
                                                             Value = (int.Parse(s.Content), null),
                                                             MaxValue = Math.Max(maxStatistics[i], 1),
                                                         })
                                                     .ToArray();
                }
                else
                {
                    for (int i = 0; i < Flow.Children.Count; i++)
                    {
                        Flow.Children[i].Value = (int.Parse(currentStatistics[i].Content), null);
                        Flow.Children[i].MaxValue = Math.Max(maxStatistics[i], 1);
                    }
                }

                lastRulesetWhenUpdated = ruleset.Value;
            });
        }

        public bool UsesFixedAnchor { get; set; }
    }
}
