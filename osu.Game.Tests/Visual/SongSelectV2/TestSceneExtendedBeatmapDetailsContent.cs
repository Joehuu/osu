// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.Select;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    public partial class TestSceneExtendedBeatmapDetailsContent : SongSelectComponentsTestScene
    {
        private ExtendedBeatmapDetailsContent infoContent = null!;

        private DummyAPIAccess api => (DummyAPIAccess)API;

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = infoContent = new ExtendedBeatmapDetailsContent();

            infoContent.Show();

            AddSliderStep("change relative width", 0, 1f, 1f, v =>
            {
                infoContent.Width = v;
            });
        }

        [Test]
        public void TestAllMetrics()
        {
            AddStep("all metrics", () => BeatmapInfo.Value = new APIBeatmap
            {
                BeatmapSet = new APIBeatmapSet
                {
                    Source = "osu!",
                    Tags = "this beatmap has all the metrics",
                    Ratings = Enumerable.Range(0, 11).ToArray(),
                    Ranked = new DateTimeOffset(DateTime.Now),
                    OnlineID = 1,
                },
                CircleCount = 313,
                SliderCount = 232,
                DifficultyName = "All Metrics",
                CircleSize = 7,
                DrainRate = 1,
                OverallDifficulty = 5.7f,
                ApproachRate = 3.5f,
                StarRating = 5.3f,
                FailTimes = new APIFailTimes
                {
                    Fails = Enumerable.Range(1, 100).Select(i => i % 12 - 6).ToArray(),
                    Retries = Enumerable.Range(-2, 100).Select(i => i % 12 - 6).ToArray(),
                },
                PassCount = 1,
                PlayCount = 10,
                OnlineID = 1,
            });
        }

        [Test]
        public void TestAllMetricsExceptSource()
        {
            AddStep("all except source", () =>
            {
                BeatmapInfo.Value = new APIBeatmap
                {
                    BeatmapSet = new APIBeatmapSet
                    {
                        Tags = "this beatmap has all the metrics",
                        Ratings = Enumerable.Range(0, 11).ToArray(),
                        OnlineID = 2,
                    },
                    DifficultyName = "All Metrics",
                    CircleSize = 7,
                    DrainRate = 1,
                    OverallDifficulty = 5.7f,
                    ApproachRate = 3.5f,
                    StarRating = 5.3f,
                    FailTimes = new APIFailTimes
                    {
                        Fails = Enumerable.Range(1, 100).Select(i => i % 12 - 6).ToArray(),
                        Retries = Enumerable.Range(-2, 100).Select(i => i % 12 - 6).ToArray(),
                    },
                    OnlineID = 2,
                };
            });
        }

        [Test]
        public void TestOnlyRatings()
        {
            AddStep("ratings", () =>
            {
                BeatmapInfo.Value = new APIBeatmap
                {
                    BeatmapSet = new APIBeatmapSet
                    {
                        Ratings = Enumerable.Range(0, 11).ToArray(),
                        Source = "osu!",
                        Tags = "this beatmap has ratings metrics but not retries or fails",
                        OnlineID = 3,
                    },
                    DifficultyName = "Only Ratings",
                    CircleSize = 6,
                    DrainRate = 9,
                    OverallDifficulty = 6,
                    ApproachRate = 6,
                    StarRating = 4.8f,
                    OnlineID = 3,
                };
            });
        }

        [Test]
        public void TestOnlyFailsAndRetries()
        {
            AddStep("fails retries", () =>
            {
                BeatmapInfo.Value = new APIBeatmap
                {
                    DifficultyName = "Only Retries and Fails",
                    BeatmapSet = new APIBeatmapSet
                    {
                        Source = "osu!",
                        Tags = "this beatmap has retries and fails but no ratings",
                        OnlineID = 4,
                    },
                    CircleSize = 3.7f,
                    DrainRate = 6,
                    OverallDifficulty = 6,
                    ApproachRate = 7,
                    StarRating = 2.91f,
                    FailTimes = new APIFailTimes
                    {
                        Fails = Enumerable.Range(1, 100).Select(i => i % 12 - 6).ToArray(),
                        Retries = Enumerable.Range(-2, 100).Select(i => i % 12 - 6).ToArray(),
                    },
                    OnlineID = 4,
                };
            });
        }

        [Test]
        public void TestNoMetrics()
        {
            AddStep("no metrics", () =>
            {
                BeatmapInfo.Value = new APIBeatmap
                {
                    DifficultyName = "No Metrics",
                    BeatmapSet = new APIBeatmapSet
                    {
                        Source = "osu!",
                        Tags = "this beatmap has no metrics",
                        OnlineID = 5,
                    },
                    CircleSize = 5,
                    DrainRate = 5,
                    OverallDifficulty = 5.5f,
                    ApproachRate = 6.5f,
                    StarRating = 1.97f,
                    OnlineID = 5,
                };
            });
        }

        [Test]
        public void TestNullBeatmap()
        {
            AddStep("null beatmap", () => BeatmapInfo.Value = null!);
        }

        [Test]
        public void TestOnlineMetrics()
        {
            AddStep("online ratings/retries/fails", () => BeatmapInfo.Value = new APIBeatmap
            {
                OnlineID = 162,
            });
            AddStep("set online", () => api.SetState(APIState.Online));
            AddStep("set offline", () => api.SetState(APIState.Offline));
        }
    }
}
