﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Framework.Testing;
using osu.Game.Graphics;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Tests.Visual.UserInterface
{
    [TestFixture]
    public partial class TestSceneOsuTooltip : OsuManualInputManagerTestScene
    {
        private TestTooltipContainer container = null!;

        [SetUp]
        public void SetUp() => Schedule(() =>
        {
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(100),
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = Colour4.Red.Opacity(0.5f),
                        RelativeSizeAxes = Axes.Both,
                    },
                    container = new TestTooltipContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = new OsuSpriteText
                        {
                            Text = "Hover me!",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = OsuFont.GetFont(size: 50)
                        }
                    },
                },
            };
        });

        private static readonly string[] test_case_tooltip_string =
        [
            "Hello!!",
            string.Concat(Enumerable.Repeat("Hello ", 100)),
            $"H{new string('e', 500)}llo",
        ];

        //TODO: o!f issue: https://github.com/ppy/osu-framework/issues/5007
        //Enable after o!f fixed
        [Ignore("o!f issue https://github.com/ppy/osu-framework/issues/5007")]
        [Test]
        public void TestTooltipBasic([Values(0, 1, 2)] int index)
        {
            AddStep("Set tooltip content", () =>
            {
                container.TooltipText = test_case_tooltip_string[index];
            });

            AddStep("Move to container", () =>
            {
                InputManager.MoveMouseTo(new Vector2(InputManager.ScreenSpaceDrawQuad.Centre.X, InputManager.ScreenSpaceDrawQuad.Centre.Y));
            });

            OsuTooltipContainer.OsuTooltip? tooltip = null!;

            AddUntilStep("Wait for the tooltip shown", () =>
            {
                tooltip = container.FindClosestParent<OsuTooltipContainer>().ChildrenOfType<OsuTooltipContainer.OsuTooltip>().FirstOrDefault();
                return tooltip != null && tooltip.Alpha == 1;
            });

            AddAssert("Is tooltip obey 500 width limit", () => tooltip != null && tooltip.Width <= 500);
        }

        internal sealed partial class TestTooltipContainer : Container, IHasTooltip
        {
            public LocalisableString TooltipText { get; set; }
        }
    }
}
