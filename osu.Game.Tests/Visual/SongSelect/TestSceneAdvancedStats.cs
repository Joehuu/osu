// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Mods;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Screens.Select.Details;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual.SongSelect
{
    [System.ComponentModel.Description("Advanced beatmap statistics display")]
    public partial class TestSceneAdvancedStats : OsuTestScene
    {
        private AdvancedStats advancedStats;

        [Resolved]
        private RulesetStore rulesets { get; set; }

        [Resolved]
        private OsuColour colours { get; set; }

        [SetUp]
        public void Setup()
        {
            SelectedMods.Value = Array.Empty<Mod>();
            Schedule(() => Child = advancedStats = new AdvancedStats
            {
                Width = 500
            });
        }

        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("reset game ruleset", () => Ruleset.Value = new OsuRuleset().RulesetInfo);
        }

        private BeatmapInfo exampleBeatmapInfo => new BeatmapInfo
        {
            Ruleset = rulesets.AvailableRulesets.First(),
            Difficulty = new BeatmapDifficulty
            {
                CircleSize = 7.2f,
                DrainRate = 3,
                OverallDifficulty = 5.7f,
                ApproachRate = 3.5f
            },
            StarRating = 4.5f
        };

        [Test]
        public void TestNoMod()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);
            AddStep("select osu ruleset", () => Ruleset.Value = rulesets.GetRuleset(0));

            AddStep("no mods selected", () => SelectedMods.Value = Array.Empty<Mod>());

            AddAssert("circle size bar is white", () => barIsWhite(barAtElement(0)));
            AddAssert("HP drain bar is white", () => barIsWhite(barAtElement(1)));
            AddAssert("accuracy bar is white", () => barIsWhite(barAtElement(2)));
            AddAssert("approach rate bar is white", () => barIsWhite(barAtElement(3)));
        }

        [Test]
        public void TestEasyMod()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);
            AddStep("select osu ruleset", () => Ruleset.Value = rulesets.GetRuleset(0));

            AddStep("select EZ mod", () =>
            {
                var ruleset = Ruleset.Value.CreateInstance().AsNonNull();
                SelectedMods.Value = new[] { ruleset.CreateMod<ModEasy>() };
            });

            AddAssert("circle size bar is blue", () => barIsBlue(barAtElement(0)));
            AddAssert("HP drain bar is blue", () => barIsBlue(barAtElement(1)));
            AddAssert("accuracy bar is blue", () => barIsBlue(barAtElement(2)));
            AddAssert("approach rate bar is blue", () => barIsBlue(barAtElement(3)));
        }

        [Test]
        public void TestHardRockMod()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);
            AddStep("select osu ruleset", () => Ruleset.Value = rulesets.GetRuleset(0));

            AddStep("select HR mod", () =>
            {
                var ruleset = Ruleset.Value.CreateInstance().AsNonNull();
                SelectedMods.Value = new[] { ruleset.CreateMod<ModHardRock>() };
            });

            AddAssert("circle size bar is red", () => barIsRed(barAtElement(0)));
            AddAssert("HP drain bar is red", () => barIsRed(barAtElement(1)));
            AddAssert("accuracy bar is red", () => barIsRed(barAtElement(2)));
            AddAssert("approach rate bar is red", () => barIsRed(barAtElement(3)));
        }

        [Test]
        public void TestUnchangedDifficultyAdjustMod()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);
            AddStep("select osu ruleset", () => Ruleset.Value = rulesets.GetRuleset(0));

            AddStep("select unchanged Difficulty Adjust mod", () =>
            {
                var ruleset = Ruleset.Value.CreateInstance().AsNonNull();
                var difficultyAdjustMod = ruleset.CreateMod<ModDifficultyAdjust>().AsNonNull();
                difficultyAdjustMod.ReadFromDifficulty(advancedStats.BeatmapInfo!.Difficulty);
                SelectedMods.Value = new[] { difficultyAdjustMod };
            });

            AddAssert("circle size bar is white", () => barIsWhite(barAtElement(0)));
            AddAssert("HP drain bar is white", () => barIsWhite(barAtElement(1)));
            AddAssert("accuracy bar is white", () => barIsWhite(barAtElement(2)));
            AddAssert("approach rate bar is white", () => barIsWhite(barAtElement(3)));
        }

        [Test]
        public void TestChangedDifficultyAdjustMod()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);
            AddStep("select osu ruleset", () => Ruleset.Value = rulesets.GetRuleset(0));

            AddStep("select changed Difficulty Adjust mod", () =>
            {
                var ruleset = Ruleset.Value.CreateInstance().AsNonNull();
                var difficultyAdjustMod = ruleset.CreateMod<OsuModDifficultyAdjust>().AsNonNull();
                var originalDifficulty = advancedStats.BeatmapInfo!.Difficulty;

                difficultyAdjustMod.ReadFromDifficulty(originalDifficulty);
                difficultyAdjustMod.DrainRate.Value = originalDifficulty.DrainRate - 0.5f;
                difficultyAdjustMod.ApproachRate.Value = originalDifficulty.ApproachRate + 2.2f;
                SelectedMods.Value = new[] { difficultyAdjustMod };
            });

            AddAssert("circle size bar is white", () => barIsWhite(barAtElement(0)));
            AddAssert("drain rate bar is blue", () => barIsBlue(barAtElement(1)));
            AddAssert("accuracy bar is white", () => barIsWhite(barAtElement(2)));
            AddAssert("approach rate bar is red", () => barIsRed(barAtElement(3)));
        }

        [Test]
        public void TestRulesetDifficultySettings()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);

            AddStep("select osu ruleset", () => Ruleset.Value = rulesets.GetRuleset(0));
            AddAssert("first bar text is correct", () => advancedStats.ChildrenOfType<SpriteText>().First().Text == BeatmapsetsStrings.ShowStatsCs);
            AddAssert("bar count is 5", () => advancedStats.ChildrenOfType<AdvancedStats.StatisticRow>().Count() == 5);

            AddStep("select taiko ruleset", () => Ruleset.Value = rulesets.GetRuleset(1));
            AddAssert("first bar text is correct", () => advancedStats.ChildrenOfType<SpriteText>().First().Text == BeatmapsetsStrings.ShowStatsDrain);
            AddAssert("bar count is 3", () => advancedStats.ChildrenOfType<AdvancedStats.StatisticRow>().Count() == 3);

            AddStep("select catch ruleset", () => Ruleset.Value = rulesets.GetRuleset(2));
            AddAssert("first bar text is correct", () => advancedStats.ChildrenOfType<SpriteText>().First().Text == BeatmapsetsStrings.ShowStatsCs);
            AddAssert("bar count is 5", () => advancedStats.ChildrenOfType<AdvancedStats.StatisticRow>().Count() == 5);

            AddStep("select mania ruleset", () => Ruleset.Value = rulesets.GetRuleset(3));
            AddAssert("first bar text is correct", () => advancedStats.ChildrenOfType<SpriteText>().First().Text == BeatmapsetsStrings.ShowStatsCsMania);
            AddAssert("bar count is 4", () => advancedStats.ChildrenOfType<AdvancedStats.StatisticRow>().Count() == 4);
        }

        [Test]
        public void TestManiaKeyCountConversion()
        {
            AddStep("set beatmap", () => advancedStats.BeatmapInfo = exampleBeatmapInfo);

            AddStep("select mania ruleset", () => Ruleset.Value = rulesets.GetRuleset(3));
            AddStep("select 1K mod", () =>
            {
                var ruleset = Ruleset.Value.CreateInstance().AsNonNull();
                SelectedMods.Value = new[] { ruleset.CreateMod<ManiaModKey1>() };
            });
            AddAssert("key count bar is blue", () => barIsBlue(barAtElement(0)));
        }

        private bool barIsWhite(AdvancedStats.StatisticRow row) => row.ModBar.AccentColour == Color4.White;
        private bool barIsBlue(AdvancedStats.StatisticRow row) => row.ModBar.AccentColour == colours.BlueDark;
        private bool barIsRed(AdvancedStats.StatisticRow row) => row.ModBar.AccentColour == colours.Red;

        private AdvancedStats.StatisticRow barAtElement(int index) => advancedStats.ChildrenOfType<AdvancedStats.StatisticRow>().ElementAt(index);
    }
}
