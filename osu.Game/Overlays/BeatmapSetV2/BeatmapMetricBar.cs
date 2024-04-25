// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public abstract partial class BeatmapMetricBar : CompositeDrawable, IHasCurrentValue<APIBeatmap?>
    {
        private readonly BindableWithCurrent<APIBeatmap?> current = new BindableWithCurrent<APIBeatmap?>();

        public Bindable<APIBeatmap?> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        protected abstract Color4 BackgroundColour { get; }

        protected abstract IEnumerable<(float startingNumber, Color4 colour)> Levels { get; }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Masking = true;
            CornerRadius = 5f;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindValueChanged(b =>
            {
                ClearInternal();
                InternalChild = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = BackgroundColour,
                };

                if (b.NewValue == null)
                    return;

                foreach (var level in Levels)
                {
                    foreach (var section in getSectionsFrom(GetValuesFrom(b.NewValue), level.startingNumber))
                    {
                        AddInternal(new Box
                        {
                            RelativePositionAxes = Axes.X,
                            RelativeSizeAxes = Axes.Both,
                            X = section.start,
                            Width = section.length,
                            Colour = level.colour,
                        });
                    }
                }
            }, true);
        }

        protected abstract IReadOnlyList<float> GetValuesFrom(APIBeatmap beatmap);

        private static IEnumerable<(float start, float length)> getSectionsFrom(IReadOnlyList<float> source, float startingNumber)
        {
            float offset = 0;
            float length = 0;

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] >= startingNumber)
                {
                    // if this is the first value matching the "starting number", then set up offset and reset length for incrementing.
                    if (i == 0 || source[i - 1] < startingNumber)
                    {
                        offset = i;
                        length = 0;
                    }

                    length++;

                    // if this is the last value matching the "starting number", then return the calculated section and proceed forward.
                    if (i == source.Count - 1 || source[i + 1] < startingNumber)
                        yield return (offset / source.Count, length / source.Count);
                }
            }
        }
    }
}
