// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Taiko;
using osu.Game.Screens.SelectV2.Wedge;

namespace osu.Game.Tests.Visual.SongSelectV2
{
    public partial class TestSceneObjectCountContent : SongSelectComponentsTestScene
    {
        private ObjectCountContent? objectCountContent;

        [Test]
        public void TestLocalBeatmap()
        {
            const int circle_count = 100;
            const int slider_count = 50;
            const int spinner_count = 2;

            AddStep("set content", () =>
            {
                Child = objectCountContent = new LocalObjectCountContent();
            });

            AddStep("set empty beatmap", () => Beatmap.Value = CreateWorkingBeatmap(new Beatmap()));

            AddStep("set ruleset to taiko", () => Ruleset.Value = new TaikoRuleset().RulesetInfo);
            AddAssert("first bar text is correct", () => objectCountContent.ChildrenOfType<SpriteText>().First().Text.ToString(), () => Is.EqualTo("Hit "));

            AddStep("set ruleset to catch", () => Ruleset.Value = new CatchRuleset().RulesetInfo);
            AddAssert("first bar text is correct", () => objectCountContent.ChildrenOfType<SpriteText>().First().Text.ToString(), () => Is.EqualTo("Fruit "));

            AddStep("set ruleset to mania", () => Ruleset.Value = new ManiaRuleset().RulesetInfo);
            AddAssert("first bar text is correct", () => objectCountContent.ChildrenOfType<SpriteText>().First().Text.ToString(), () => Is.EqualTo("Note "));

            AddStep("set ruleset to osu", () => Ruleset.Value = new OsuRuleset().RulesetInfo);
            AddAssert("first bar text is correct", () => objectCountContent.ChildrenOfType<SpriteText>().First().Text.ToString(), () => Is.EqualTo("Circle "));

            AddStep("set beatmap with hit objects", () =>
            {
                var beatmap = new Beatmap();

                for (int i = 0; i < circle_count; i++)
                    beatmap.HitObjects.Add(new HitCircle());

                for (int i = 0; i < slider_count; i++)
                    beatmap.HitObjects.Add(new Slider());

                for (int i = 0; i < spinner_count; i++)
                    beatmap.HitObjects.Add(new Spinner());

                Beatmap.Value = CreateWorkingBeatmap(beatmap);
            });

            AddAssert("first value is set", () => getBarValueAt(0), () => Is.EqualTo(circle_count));
            AddAssert("second value is set", () => getBarValueAt(1), () => Is.EqualTo(slider_count));
            AddAssert("third value is set", () => getBarValueAt(2), () => Is.EqualTo(spinner_count));
        }

        private float getBarValueAt(int index) => objectCountContent.ChildrenOfType<BarStatisticRow>().ElementAt(index).Value.baseValue;
    }
}
