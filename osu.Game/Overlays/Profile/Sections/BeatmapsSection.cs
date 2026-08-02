// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Online.API.Requests;
using osu.Game.Overlays.Profile.Sections.Beatmaps;
using osu.Game.Resources.Localisation.Web;

namespace osu.Game.Overlays.Profile.Sections
{
    public partial class BeatmapsSection : ProfileSection
    {
        public override LocalisableString Title => UsersStrings.ShowExtraBeatmapsTitle;

        public override string Identifier => @"beatmaps";

        [BackgroundDependencyLoader]
        private void load()
        {
            ChildrenEnumerable = createSubSections();
        }

        private IEnumerable<Drawable> createSubSections()
        {
            foreach (var type in Enum.GetValues<BeatmapSetType>())
            {
                var subSection = new PaginatedBeatmapContainer(type, User, type.GetLocalisableDescription());
                if (subSection.GetCount(User.Value!.User) == 0 && type != BeatmapSetType.Favourite)
                    continue;

                yield return subSection;
            }
        }
    }
}
