// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSetV2;
using osu.Game.Rulesets;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class ObjectCountContent : FillFlowContainer<BarStatisticRow>
    {
        public Bindable<IBeatmapInfo?> BeatmapInfo { get; set; } = new Bindable<IBeatmapInfo?>();

        [Resolved]
        private OsuGame? game { get; set; }

        [Resolved]
        private IRulesetStore rulesetStore { get; set; } = null!;

        [Resolved]
        private IBindable<WorkingBeatmap> workingBeatmap { get; set; } = null!;

        public ObjectCountContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(5);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            game?.Ruleset.BindValueChanged(_ => updateObjectCounts());

            BeatmapInfo.BindValueChanged(_ => updateObjectCounts(), true);

            workingBeatmap.BindValueChanged(_ => updateObjectCounts(), true);
        }

        private void updateObjectCounts()
        {
            if (workingBeatmap.Value.BeatmapInfo.OnlineID != BeatmapInfo.Value?.OnlineID) return;

            try
            {
                IRulesetInfo? ruleset = BeatmapInfo.Value is APIBeatmap onlineInfo ? rulesetStore.GetRuleset(onlineInfo.Ruleset.OnlineID)! : game?.Ruleset.Value;

                var localBeatmap = workingBeatmap.Value.GetPlayableBeatmap(ruleset);

                Children = localBeatmap.GetStatistics()
                                       .Select(s => new BarStatisticRow(1000)
                                       {
                                           Title = s.Name,
                                           Value = (int.Parse(s.Content), null),
                                       })
                                       .ToArray();
            }
            catch (Exception)
            {
                // TODO: explain this better or remove it
                // suppress BeatmapInvalidForRulesetException when ruleset is changing but beatmap is not
            }

            if (BeatmapInfo.Value is APIBeatmap beatmap)
            {
                // TODO: kinda hacky
                Children[0].Value = (beatmap.CircleCount, null);
                Children[1].Value = (beatmap.SliderCount, null);

                if (Children.ElementAtOrDefault(2) != null)
                    Children.ElementAtOrDefault(2)!.Value = (beatmap.SpinnerCount, null);
            }
        }
    }
}
