// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Resources.Localisation.Web;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public partial class UserRatingsV2 : CompositeDrawable
    {
        private Bar ratingsBar = null!;
        private OsuSpriteText negativeRatings = null!;
        private OsuSpriteText positiveRatings = null!;
        private BarGraph graph = null!;

        private int[]? ratings;

        public int[]? Ratings
        {
            get => ratings;
            set
            {
                if (value == ratings) return;

                ratings = value;

                const int rating_range = 10;

                if (ratings == null)
                {
                    negativeRatings.Text = 0.ToLocalisableString(@"N0");
                    positiveRatings.Text = 0.ToLocalisableString(@"N0");
                    ratingsBar.Length = 0;
                    graph.Values = new float[rating_range];
                }
                else
                {
                    var usableRange = ratings.Skip(1).Take(rating_range); // adjust for API returning weird empty data at 0.

                    int negativeCount = usableRange.Take(rating_range / 2).Sum();
                    int totalCount = usableRange.Sum();

                    negativeRatings.Text = negativeCount.ToLocalisableString(@"N0");
                    positiveRatings.Text = (totalCount - negativeCount).ToLocalisableString(@"N0");
                    ratingsBar.Length = totalCount == 0 ? 0 : (float)negativeCount / totalCount;
                    graph.Values = usableRange.Take(rating_range).Select(r => (float)r);
                }
            }
        }

        public UserRatingsV2()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Text = BeatmapsetsStrings.ShowStatsUserRating,
                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new[]
                            {
                                negativeRatings = new OsuSpriteText
                                {
                                    Text = 0.ToLocalisableString(@"N0"),
                                    Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                                },
                                positiveRatings = new OsuSpriteText
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    Text = 0.ToLocalisableString(@"N0"),
                                    Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                                },
                            },
                        },
                        ratingsBar = new Bar
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 4,
                            CornerRadius = 2,
                            Masking = true,
                            BackgroundColour = colours.Lime1,
                            AccentColour = colours.Red3,
                        },
                        new OsuSpriteText
                        {
                            Margin = new MarginPadding { Top = 10 },
                            Text = BeatmapsetsStrings.ShowStatsRatingSpread,
                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 30,
                            Child = graph = new BarGraph
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = colours.BlueDark,
                            },
                        },
                    },
                },
            };
        }
    }
}
