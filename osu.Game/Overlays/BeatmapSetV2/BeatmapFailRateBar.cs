// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Game.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class BeatmapFailRateBar : BeatmapMetricBar
    {
        [Resolved]
        private OsuColour colours { get; set; } = null!;

        protected override Color4 BackgroundColour => Color4Extensions.FromHex(@"aadd00");

        protected override IEnumerable<(float startingNumber, Color4 colour)> Levels => new[]
        {
            (0.25f, colours.YellowLight),
            (0.5f, colours.Yellow),
            (0.625f, colours.YellowDark),
            (0.75f, Color4Extensions.FromHex("eb5757")),
            (1f, Color4Extensions.FromHex("643232")),
        };

        protected override IReadOnlyList<float> GetValuesFrom(APIBeatmap beatmap) // todo: this should be done web-side or calculated better...
        {
            int[]? fails = beatmap.FailTimes?.Fails;

            if (fails == null) return new List<float>();

            return fails.Select(f => (float)f / fails.Max()).ToList();
        }
    }
}
