// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.BeatmapSetV2;
using osu.Game.Users;

namespace osu.Game.Overlays.BeatmapSet
{
    public partial class MetadataSectionCreator : MetadataSectionV2<IUser>
    {
        public override IUser Metadata
        {
            set
            {
                if (string.IsNullOrEmpty(value.Username))
                {
                    this.FadeOut(TRANSITION_DURATION);
                    return;
                }

                base.Metadata = value;
            }
        }

        public MetadataSectionCreator(Action<IUser>? searchAction = null)
            : base(MetadataType.Creator, searchAction)
        {
        }

        protected override void AddMetadata(IUser metadata, LinkFlowContainer loaded)
        {
            loaded.AddUserLink(metadata, t => t.Font = OsuFont.GetFont(weight: FontWeight.Bold, size: 12));
        }
    }
}
