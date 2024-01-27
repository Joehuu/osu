// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.Select
{
    public partial class RankingsHeader : Container
    {
        private DetailsWedgeContainer content = null!;
        private Bindable<PlayBeatmapDetailArea.TabType> selectedTab = null!;
        private RankingsTabControl tabControl = null!;

        public RankingsHeader()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider, OsuConfigManager config)
        {
            selectedTab = config.GetBindable<PlayBeatmapDetailArea.TabType>(OsuSetting.BeatmapDetailTab);

            Child = content = new DetailsWedgeContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Child = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Left = SongSelect.TEXT_MARGIN, Vertical = 10, Right = 20 },
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                        },
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(GridSizeMode.Absolute, 10),
                            new Dimension(),
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(6),
                                    Children = new Drawable[]
                                    {
                                        new OsuSpriteText
                                        {
                                            Text = "Rankings",
                                            Font = OsuFont.GetFont(Typeface.TorusAlternate, 18)
                                        },
                                        new Box
                                        {
                                            Size = new Vector2(28, 2),
                                            Colour = colourProvider.Highlight1,
                                        }
                                    }
                                },
                                Empty(), // padding
                                tabControl = new RankingsTabControl
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                },
                            }
                        }
                    }
                }
            };

            content.Show();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            selectedTab.BindValueChanged(tab => tabControl.Current.Value = tab.NewValue, true);
            tabControl.Current.BindValueChanged(tab => selectedTab.Value = tab.NewValue);
        }

        private partial class RankingsTabControl : OsuTabControl<PlayBeatmapDetailArea.TabType>
        {
            public RankingsTabControl()
            {
                TabContainer.Spacing = new Vector2(5);
            }

            protected override TabFillFlowContainer CreateTabFlow() => new RankingsTabFillFlowContainer
            {
                Direction = FillDirection.Full,
                RelativeSizeAxes = Axes.Both,
                Depth = -1,
                Masking = true
            };

            private partial class RankingsTabFillFlowContainer : TabFillFlowContainer
            {
                // TODO: fix overflowing buttons starting from the left (i.e. local button hiding first)
                // reverse as tab items are right anchored
                public override IEnumerable<Drawable> GetFlowingTabs(IEnumerable<Drawable> tabs) => base.GetFlowingTabs(tabs).Reverse();
            }

            protected override TabItem<PlayBeatmapDetailArea.TabType> CreateTabItem(PlayBeatmapDetailArea.TabType value) => new RankingsTabItem(value);

            private partial class RankingsTabItem : OsuTabItem
            {
                private readonly RankingsToggleButton button;

                public RankingsTabItem(PlayBeatmapDetailArea.TabType value)
                    : base(value)
                {
                    Anchor = Anchor.TopRight;
                    Origin = Anchor.TopRight;

                    Child = button = new RankingsToggleButton(value)
                    {
                        Text = value.GetLocalisableDescription(),
                    };
                }

                protected override void LoadComplete()
                {
                    base.LoadComplete();

                    button.Active.BindTo(Active);
                }

                protected override void OnActivated()
                {
                    // handled by button.Active state changing
                }

                protected override void OnDeactivated()
                {
                    // handled by button.Active state changing
                }
            }
        }

        private partial class RankingsToggleButton : ShearedToggleButton
        {
            private readonly PlayBeatmapDetailArea.TabType tabType;

            public RankingsToggleButton(PlayBeatmapDetailArea.TabType tabType)
                : base(120)
            {
                this.tabType = tabType;

                Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14);
                Padding = new MarginPadding();
                Height = 25;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                HasGradientBorder = false;
            }

            protected override void UpdateActiveState()
            {
                DarkerColour = Active.Value ? getButtonColourForRanking(tabType) : ColourProvider.Background3;
                LighterColour = Active.Value ? Colour4.White : ColourProvider.Background1;
                TextColour = Active.Value ? ColourProvider.Background6 : ColourProvider.Content1;
            }

            private Colour4 getButtonColourForRanking(PlayBeatmapDetailArea.TabType value)
            {
                switch (value)
                {
                    case PlayBeatmapDetailArea.TabType.Local:
                        return Colour4.FromHex("CAFF5A");

                    case PlayBeatmapDetailArea.TabType.Country:
                        return Colour4.Red;

                    case PlayBeatmapDetailArea.TabType.Global:
                        return Colour4.FromHex("FFA95A");

                    case PlayBeatmapDetailArea.TabType.Friends:
                        return Colour4.FromHex("5AFFF5");

                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }

            protected override bool OnClick(ClickEvent e)
            {
                base.OnClick(e);
                // handled by RankingsTabItem
                // TODO: fix double samples playing
                return false;
            }
        }
    }
}
