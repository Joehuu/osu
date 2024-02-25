// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    public abstract partial class SongSelectComponentsTestScene : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Aquamarine);

        /// <summary>
        /// The local/online beatmap.
        /// </summary>
        /// <remarks>
        /// This is the same as <see cref="apiBeatmap"/> if online.
        /// </remarks>
        [Cached(typeof(IBindable<IBeatmapInfo?>))]
        protected readonly Bindable<IBeatmapInfo?> BeatmapInfo = new Bindable<IBeatmapInfo?>();

        /// <summary>
        /// The local/online beatmap set.
        /// </summary>
        [Cached(typeof(IBindable<IBeatmapSetInfo?>))]
        private readonly Bindable<IBeatmapSetInfo?> beatmapSetInfo = new Bindable<IBeatmapSetInfo?>();

        /// <summary>
        /// The online beatmap fetched from the api.
        /// </summary>
        [Cached(typeof(IBindable<APIBeatmap?>))]
        private readonly Bindable<APIBeatmap?> apiBeatmap = new Bindable<APIBeatmap?>();

        private int setID;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // mimics song select's `WorkingBeatmap` binding
            Beatmap.BindValueChanged(b =>
            {
                BeatmapInfo.Value = b.NewValue.BeatmapInfo;
                beatmapSetInfo.Value = b.NewValue?.BeatmapSetInfo;
            });

            // mimics beatmap set overlay's `APIBeatmap` binding
            // after selecting first beatmap from set response (done this way for simplicity)
            BeatmapInfo.BindValueChanged(b =>
            {
                beatmapSetInfo.Value = b.NewValue?.BeatmapSet as APIBeatmapSet;
                apiBeatmap.Value = b.NewValue as APIBeatmap;
            });
        }

        [SetUpSteps]
        public virtual void SetUpSteps()
        {
            AddStep("reset dependencies", () =>
            {
                Beatmap.Value = Beatmap.Default;
                SelectedMods.SetDefault();
                BeatmapInfo.Value = null;
                beatmapSetInfo.Value = null;
                apiBeatmap.Value = null;
            });
        }

        protected APIBeatmapSet CreateFigmaExampleBeatmapSet(BeatmapOnlineStatus status, bool hasExplicitContent)
        {
            var set = new APIBeatmapSet
            {
                OnlineID = ++setID,
                Title = "quaver",
                Artist = "dj TAKA",
                AuthorString = "Sotarks",
                RelatedUsers =
                [
                    new APIUser { Id = 1, Username = "mapper name" },
                ],
                Source = "REFLEC BEAT limelight",
                Tags = "risk junk beatmania iidx 19 lincle jubeat saucer jukebeat",
                Status = status,
                HasExplicitContent = hasExplicitContent,
                HasFavourited = true,
                FavouriteCount = 2593,
                Genre = new BeatmapSetOnlineGenre { Name = "Video Game (Instrumental)" },
                Language = new BeatmapSetOnlineLanguage { Name = "Instrumental" },
                Covers = new BeatmapSetOnlineCovers
                {
                    Cover =
                        "https://s3-alpha-sig.figma.com/img/d95c/56af/cc030fa23122fcb38db25d02288a977d?Expires=1636329600&Signature=bmpZcUhqCWXY8dqyEC5u5~Ev0L04BoxxYaBlyZiI9MyNJqj41S2869DOicjxLejNJtig5kuFaXpv31oB5qUAcve4B5xE0CVKnsKWZWHJag-N5zdIXvCciGYEPnG-YGqZJ-oICvS-Byhae6PcC0sAu0IwIHTEV-TJIhj3KTHWFEFy12fKMrElsas8BmlPF7niy7cbY3OQjcIXmeoiYUdGTBNxnLOIWhHmnQM-VzUQa~cJEATPmPAJFRw8Lolf~we754pDGwMP~joIiX0palLEbipEo~3iU4MfXoTjpYqMlE2QiH3IRwpgjD5~6rTZshv2XD8-Kkw~eka8jejpQ976xw__&Key-Pair-Id=APKAINTVSUGEWH5XD5UA"
                },
                Submitted = new DateTime(2018, 11, 4, 0, 0, 0),
                Ranked = new DateTime(2019, 1, 6, 0, 0, 0),
                Ratings = new[]
                {
                    0,
                    1320,
                    193,
                    157,
                    290,
                    359,
                    398,
                    773,
                    1477,
                    2792,
                    15922
                },
            };

            // set.Beatmaps = Enumerable.Range(0, 32).Select(i =>
            // {
            //     var beatmap = CreateFigmaExampleBeatmap(set, i);
            //     beatmap.RulesetID = i % 4;
            //     return beatmap;
            // }).ToArray();

            return set;
        }

        protected Beatmap CreateFigmaExampleBeatmap(int index)
        {
            var beatmapInfo = CreateFigmaExampleBeatmapInfo(index);

            beatmapInfo.BeatmapSet = new BeatmapSetInfo
            {
                OnlineID = ++setID,
                Beatmaps = { CreateFigmaExampleBeatmapInfo(index) }
            };

            return new Beatmap
            {
                BeatmapInfo = CreateFigmaExampleBeatmapInfo(index),
            };
        }

        public BeatmapInfo CreateFigmaExampleBeatmapInfo(int index) => new BeatmapInfo
        {
            OnlineID = 1000 + index,
            DifficultyName = "Very Long Difficulty Name",
            StarRating = 1.50 + index * 0.525,
            Length = 203000 + index * 1234,
            BPM = 140 + index * 5,
            Metadata = new BeatmapMetadata
            {
                Title = "quaver",
                Artist = "dj TAKA",
                Source = "REFLEC BEAT limelight",
                Tags = "risk junk beatmania iidx 19 lincle jubeat saucer jukebeat",
            },
        };
    }
}
