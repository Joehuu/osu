// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class BasicDifficultyInfoContent : Container
    {
        private const float wedge_height = 100;
        public const float SHEAR_WIDTH = SongSelect.SHEAR_X * wedge_height;

        private LengthAndBPMStatisticPill lengthAndBPMStatistic = null!;

        private DifficultyNameContent difficultyNameContent = null!;

        public BasicDifficultyInfoContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                difficultyNameContent = new DifficultyNameContent
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                },
                                lengthAndBPMStatistic = new LengthAndBPMStatisticPill
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                }
                            }
                        },
                        new DensityAndFailGraphContent(),
                    },
                },
            };
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            // special logic
            difficultyNameContent.Padding = new MarginPadding { Right = lengthAndBPMStatistic.DrawWidth + 10 };
        }
    }
}
