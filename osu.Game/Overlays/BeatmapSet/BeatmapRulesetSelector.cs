// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets;
using System.Linq;
using osu.Framework.Allocation;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Overlays.BeatmapSet
{
    public partial class BeatmapRulesetSelector : OverlayRulesetSelector
    {
        [Resolved]
        private Bindable<APIBeatmapSet> beatmapSet { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmapSet.BindValueChanged(_ => SelectTab(TabContainer.TabItems.FirstOrDefault(t => t.Enabled.Value)));
        }

        protected override TabItem<RulesetInfo> CreateTabItem(RulesetInfo value) => new BeatmapRulesetTabItem(value);
    }
}
