// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Graphics.UserInterface
{
    public partial class Bar : CompositeDrawable, IHasAccentColour
    {
        private readonly Container background;
        private readonly Container bar;

        private const int resize_duration = 250;

        private const Easing easing = Easing.InOutCubic;

        private float length;

        /// <summary>
        /// Length of the bar, ranging from <see cref="MinLength"/> to <see cref="MaxLength"/>.
        /// </summary>
        public float Length
        {
            get => length;
            set
            {
                if (value == length)
                    return;

                length = value;

                updateBarLength();
            }
        }

        private float minLength;

        /// <summary>
        /// Minimum length of the bar.
        /// </summary>
        public float MinLength
        {
            get => minLength;
            set
            {
                if (value == minLength)
                    return;

                minLength = value;

                updateBarLength();
            }
        }

        private float maxLength = 1f;

        /// <summary>
        /// Maximum length of the bar.
        /// </summary>
        public float MaxLength
        {
            get => maxLength;
            set
            {
                if (value == maxLength)
                    return;

                maxLength = value;

                updateBarLength();
            }
        }

        public Color4 BackgroundColour
        {
            get => background.Colour;
            set => background.Colour = value;
        }

        public Color4 AccentColour
        {
            get => bar.Colour;
            set => bar.Colour = value;
        }

        private BarDirection direction = BarDirection.LeftToRight;

        public BarDirection Direction
        {
            get => direction;
            set
            {
                if (value == direction)
                    return;

                direction = value;

                updateBarDirection();
            }
        }

        public new MarginPadding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        /// <summary>
        /// Sets the background and bar's corner radius. Also implicitly turns on masking if the value is not zero.
        /// </summary>
        public new float CornerRadius
        {
            get => bar.CornerRadius;
            set
            {
                background.Masking = value != 0;
                background.CornerRadius = value;

                bar.Masking = value != 0;
                bar.CornerRadius = value;
            }
        }

        public Bar()
        {
            InternalChildren = new Drawable[]
            {
                background = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(0, 0, 0, 0),
                    Child = CreateBackground().With(d => d.RelativeSizeAxes = Axes.Both),
                },
                bar = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0,
                    Child = CreateBar().With(d => d.RelativeSizeAxes = Axes.Both),
                },
            };
        }

        /// <summary>
        /// Updates the bar display with the current length and its ranges.
        /// </summary>
        private void updateBarLength()
        {
            float drawLength = Math.Clamp((length - minLength) / (maxLength - minLength), 0f, 1f);

            switch (direction)
            {
                case BarDirection.LeftToRight:
                case BarDirection.RightToLeft:
                    bar.ResizeTo(new Vector2(drawLength, 1), resize_duration, easing);
                    break;

                case BarDirection.TopToBottom:
                case BarDirection.BottomToTop:
                    bar.ResizeTo(new Vector2(1, drawLength), resize_duration, easing);
                    break;
            }
        }

        /// <summary>
        /// Updates the bar with the new direction and refreshes the displayed length.
        /// </summary>
        private void updateBarDirection()
        {
            switch (direction)
            {
                case BarDirection.LeftToRight:
                case BarDirection.TopToBottom:
                    bar.Anchor = Anchor.TopLeft;
                    bar.Origin = Anchor.TopLeft;
                    break;

                case BarDirection.RightToLeft:
                case BarDirection.BottomToTop:
                    bar.Anchor = Anchor.BottomRight;
                    bar.Origin = Anchor.BottomRight;
                    break;
            }

            switch (direction)
            {
                case BarDirection.LeftToRight:
                case BarDirection.RightToLeft:
                    bar.Size = new Vector2((0 - minLength) / (maxLength - minLength), 1);
                    break;

                case BarDirection.TopToBottom:
                case BarDirection.BottomToTop:
                    bar.Size = new Vector2(1, (0 - minLength) / (maxLength - minLength));
                    break;
            }

            updateBarLength();
        }

        protected virtual Drawable CreateBackground() => new Box();
        protected virtual Drawable CreateBar() => new Box();
    }

    public enum BarDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop,
    }
}
