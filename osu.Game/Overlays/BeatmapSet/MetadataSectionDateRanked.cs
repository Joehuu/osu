// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.BeatmapSetV2;

namespace osu.Game.Overlays.BeatmapSet
{
    public partial class MetadataSectionDateRanked : MetadataSectionV2<DateTimeOffset?>
    {
        public MetadataSectionDateRanked(Action<DateTimeOffset?>? searchAction = null)
            : base(MetadataType.DateRanked, searchAction)
        {
        }

        protected override void AddMetadata(DateTimeOffset? metadata, LinkFlowContainer loaded)
        {
            if (metadata != null)
            {
                loaded.AddArbitraryDrawable(new DrawableDate(metadata.Value)
                {
                    Font = OsuFont.GetFont(weight: FontWeight.Bold, size: 12)
                });
            }
        }
    }
}
