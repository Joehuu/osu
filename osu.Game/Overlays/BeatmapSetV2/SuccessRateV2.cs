// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class SuccessRateV2 : CompositeDrawable
    {
        public readonly Bindable<APIBeatmap?> BeatmapInfo = new Bindable<APIBeatmap?>();

        private SuccessRatePercentage successPercent = null!;
        private Bar successRate = null!;

        public SuccessRateV2()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, OverlayColourProvider colourProvider)
        {
            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new OsuSpriteText
                    {
                        Text = BeatmapsetsStrings.ShowInfoSuccessRate,
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                    },
                    successPercent = new SuccessRatePercentage
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                    },
                    successRate = new Bar
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 4,
                        CornerRadius = 2,
                        Masking = true,
                        AccentColour = colours.Lime1,
                        BackgroundColour = colours.Red3,
                    },
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            BeatmapInfo.BindValueChanged(b =>
            {
                int passCount = b.NewValue?.PassCount ?? 0;
                int playCount = b.NewValue?.PlayCount ?? 0;

                float rate = playCount != 0 ? (float)passCount / playCount : 0;
                successPercent.Text = rate.ToLocalisableString(@"0.#%");
                successPercent.TooltipText = $"{passCount} / {playCount}";
                successRate.Length = rate;
            }, true);
        }

        private partial class SuccessRatePercentage : OsuSpriteText, IHasTooltip
        {
            public LocalisableString TooltipText { get; set; }
        }
    }
}
