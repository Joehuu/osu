// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.BeatmapSet;
using osu.Game.Screens.Select;
using osuTK;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class MetadataColumn : FillFlowContainer
    {
        private MetadataSectionSource source = null!;
        private MetadataSectionTags tags = null!;

        public readonly Bindable<IBeatmapSetInfo?> BeatmapSetInfo = new Bindable<IBeatmapSetInfo?>();
        private MetadataSectionDateSubmitted dateSubmitted = null!;
        private MetadataSectionDateRanked dateRanked = null!;
        private MetadataSectionCreator creator = null!;
        private MetadataSectionGenre genre = null!;
        private MetadataSectionLanguage language = null!;
        private MetadataSectionNominators nominators = null!;

        public MetadataColumn()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            LayoutDuration = 250;
            LayoutEasing = Easing.OutQuad;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(3);
        }

        [BackgroundDependencyLoader]
        private void load(SongSelect? songSelect)
        {
            Children = new Drawable[]
            {
                creator = new MetadataSectionCreator(),
                source = new MetadataSectionSource(query => songSelect?.Search(query)),
                genre = new MetadataSectionGenre(),
                language = new MetadataSectionLanguage(),
                tags = new MetadataSectionTags(query => songSelect?.Search(query)),
                nominators = new MetadataSectionNominators(),
                dateSubmitted = new MetadataSectionDateSubmitted(),
                dateRanked = new MetadataSectionDateRanked(),
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            BeatmapSetInfo.BindValueChanged(b =>
            {
                if (b.NewValue == null) return;

                creator.Metadata = b.NewValue.Metadata.Author;
                source.Metadata = b.NewValue.Metadata.Source;
                tags.Metadata = b.NewValue.Metadata.Tags;

                switch (b.NewValue)
                {
                    case BeatmapSetInfo beatmap:
                        dateSubmitted.Metadata = beatmap.DateSubmitted;
                        dateRanked.Metadata = beatmap.DateRanked;
                        break;

                    case APIBeatmapSet apiBeatmap:
                        dateSubmitted.Metadata = apiBeatmap.Submitted;
                        dateRanked.Metadata = apiBeatmap.Ranked;

                        // genre and language is not stored yet
                        genre.Metadata = apiBeatmap.Genre;
                        language.Metadata = apiBeatmap.Language;
                        nominators.Metadata = (apiBeatmap.CurrentNominations ?? Array.Empty<BeatmapSetOnlineNomination>(), apiBeatmap.RelatedUsers ?? Array.Empty<APIUser>());
                        break;
                }
            }, true);
        }
    }
}
