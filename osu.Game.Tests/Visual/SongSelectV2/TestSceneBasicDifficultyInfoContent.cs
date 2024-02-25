// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Models;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.Select;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    public partial class TestSceneBasicDifficultyInfoContent : SongSelectComponentsTestScene
    {
        private BasicDifficultyInfoContent? infoContent;
        private float relativeWidth;

        [BackgroundDependencyLoader]
        private void load()
        {
            AddSliderStep("change relative width", 0, 1f, 1f, v =>
            {
                if (infoContent != null) infoContent.Width = v;

                relativeWidth = v;
            });
        }

        [SetUpSteps]
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("set content", () =>
            {
                Child = infoContent = new BasicDifficultyInfoContent
                {
                    Width = relativeWidth,
                };
            });
        }

        [Test]
        public void TestLocalBeatmap()
        {
            AddStep("set beatmap", () => Beatmap.Value = CreateWorkingBeatmap(new Beatmap
            {
                BeatmapInfo = new BeatmapInfo
                {
                    Length = 83000,
                    BPM = 123,
                    OnlineID = 1,
                    Metadata = new BeatmapMetadata
                    {
                        Author = new RealmUser { Username = "really long username" }
                    },
                    DifficultyName = "really long difficulty"
                },
            }));
        }

        [Test]
        public void TestAPIBeatmap()
        {
            AddStep("set beatmap", () => BeatmapInfo.Value = new APIBeatmap
            {
                DifficultyName = "user 1's difficulty name",
                AuthorID = 1,
                Length = 83000,
                HitLength = 62000,
                BPM = 321,
                OnlineID = 2,
                BeatmapSet = new APIBeatmapSet
                {
                    RelatedUsers =
                    [
                        new APIUser { Id = 1, Username = "user 1" },
                        new APIUser { Id = 2, Username = "user 2" }
                    ],
                }
            });
        }

        [Test]
        public void TestNullBeatmap()
        {
        }
    }
}
