// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Screens.Select;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    public partial class TestSceneBasicDifficultyInfoContent : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Aquamarine);

        [Cached]
        private readonly Bindable<IBeatmapInfo?> beatmapInfo = new Bindable<IBeatmapInfo?>();

        [Cached]
        private readonly Bindable<IBeatmapSetInfo> beatmapSetInfo = new Bindable<IBeatmapSetInfo>();

        [BackgroundDependencyLoader]
        private void load()
        {
            BasicDifficultyInfoContent infoContent;

            Child = infoContent = new BasicDifficultyInfoContent();

            AddSliderStep("change relative width", 0, 1f, 1f, v =>
            {
                infoContent.Width = v;
            });
        }

        [Test]
        public void TestLocalBeatmap()
        {
            AddStep("set beatmap", () => beatmapInfo.Value = new BeatmapInfo
            {
                Length = 83000,
            });
        }

        [Test]
        public void TestAPIBeatmap()
        {
            AddStep("set beatmap", () => beatmapInfo.Value = new APIBeatmap
            {
                Length = 83000,
                HitLength = 62000
            });
        }

        [Test]
        public void TestNullBeatmap()
        {
            AddStep("set beatmap", () => beatmapInfo.Value = null);
        }
    }
}
