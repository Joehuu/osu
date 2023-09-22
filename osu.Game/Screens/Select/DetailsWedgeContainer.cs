// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;

namespace osu.Game.Screens.Select
{
    public partial class DetailsWedgeContainer : VisibilityContainer
    {
        private readonly Container content = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
        };

        protected override Container<Drawable> Content => content;

        public DetailsWedgeContainer()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = SongSelect.WEDGE_CORNER_RADIUS,
                    Shear = SongSelect.WEDGED_CONTAINER_SHEAR,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourProvider.Background5,
                        },
                    },
                },
                content
            };
        }

        protected override void PopIn()
        {
            this.MoveToX(0, 600, Easing.OutQuint);
            this.FadeIn(200, Easing.In);
        }

        protected override void PopOut()
        {
            this.MoveToX(-150, 600, Easing.OutQuint);
            this.FadeOut(200, Easing.OutQuint);
        }
    }
}
