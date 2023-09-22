// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.BeatmapSet;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class MetadataSectionDescription : MetadataSectionV2
    {
        public MetadataSectionDescription(Action<string>? searchAction = null)
            : base(MetadataType.Description, searchAction)
        {
        }

        protected override void AddMetadata(string metadata, LinkFlowContainer loaded)
        {
            loaded.AddText(metadata);
        }
    }
}
