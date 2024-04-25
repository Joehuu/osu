// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Graphics.Containers;
using osu.Game.Online.Chat;
using osu.Game.Overlays.BeatmapSetV2;

namespace osu.Game.Overlays.BeatmapSet
{
    public partial class MetadataSectionSource : MetadataSectionV2
    {
        public MetadataSectionSource(Action<string>? searchAction = null)
            : base(MetadataType.Source, searchAction)
        {
        }

        protected override void AddMetadata(string metadata, LinkFlowContainer loaded)
        {
            if (SearchAction != null)
                loaded.AddLink(metadata, () => SearchAction(metadata));
            else
                loaded.AddLink(metadata, LinkAction.SearchBeatmapSet, metadata);
        }
    }
}
