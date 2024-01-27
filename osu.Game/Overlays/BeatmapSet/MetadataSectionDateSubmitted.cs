// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.BeatmapSet;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class MetadataSectionDateSubmitted : MetadataSectionV2<DateTimeOffset?>
    {
        public MetadataSectionDateSubmitted(Action<DateTimeOffset?>? searchAction = null)
            : base(MetadataType.DateSubmitted, searchAction)
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
