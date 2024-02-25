// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;

namespace osu.Game.Screens.Select
{
    public abstract partial class BeatmapDetailArea : Container
    {
        private const float details_padding = 10;

        public virtual WorkingBeatmap Beatmap { get; set; }

        protected Bindable<BeatmapDetailAreaTabItem> CurrentTab => tabControl.Current;

        protected Bindable<bool> CurrentModsFilter => tabControl.CurrentModsFilter;

        private readonly Container content;
        protected override Container<Drawable> Content => content;

        private readonly BeatmapDetailAreaTabControl tabControl;

        protected BeatmapDetailArea()
        {
            AddRangeInternal(new Drawable[]
            {
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = BeatmapDetailAreaTabControl.HEIGHT },
                },
                tabControl = new BeatmapDetailAreaTabControl
                {
                    RelativeSizeAxes = Axes.X,
                    TabItems = CreateTabItems(),
                    OnFilter = OnTabChanged,
                },
            });
        }

        /// <summary>
        /// Refreshes the currently-displayed details.
        /// </summary>
        public virtual void Refresh()
        {
        }

        /// <summary>
        /// Invoked when a new tab is selected.
        /// </summary>
        /// <param name="tab">The tab that was selected.</param>
        /// <param name="selectedMods">Whether the currently-selected mods should be considered.</param>
        protected virtual void OnTabChanged(BeatmapDetailAreaTabItem tab, bool selectedMods)
        {
        }

        /// <summary>
        /// Creates the tabs to be displayed.
        /// </summary>
        /// <returns>The tabs.</returns>
        protected virtual BeatmapDetailAreaTabItem[] CreateTabItems() => new BeatmapDetailAreaTabItem[]
        {
        };
    }
}
