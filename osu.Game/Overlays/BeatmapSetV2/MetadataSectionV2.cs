// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays.BeatmapSet;

namespace osu.Game.Overlays.BeatmapSetV2
{
    public abstract partial class MetadataSectionV2 : MetadataSectionV2<string>
    {
        public override string Metadata
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.FadeOut(TRANSITION_DURATION);
                    return;
                }

                base.Metadata = value;
            }
        }

        protected MetadataSectionV2(MetadataType type, Action<string>? searchAction = null)
            : base(type, searchAction)
        {
        }
    }

    public abstract partial class MetadataSectionV2<T> : CompositeDrawable
    {
        private Container textContainer = null!;
        private TextFlowContainer? textFlow;

        private readonly MetadataType type;
        protected readonly Action<T>? SearchAction;

        protected const float TRANSITION_DURATION = 250;

        public virtual T Metadata
        {
            set
            {
                if (value == null)
                {
                    this.FadeOut(TRANSITION_DURATION);
                    return;
                }

                this.FadeIn(TRANSITION_DURATION);

                setTextFlowAsync(value);
            }
        }

        protected MetadataSectionV2(MetadataType type, Action<T>? searchAction = null)
        {
            this.type = type;
            SearchAction = searchAction;

            Alpha = 0;

            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = textContainer = new Container
            {
                Alpha = 0,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Child = new OsuSpriteText
                {
                    Width = 80,
                    Text = type.GetLocalisableDescription(),
                    Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                },
            };
        }

        private void setTextFlowAsync(T metadata)
        {
            LoadComponentAsync(new LinkFlowContainer(s => s.Font = OsuFont.GetFont(weight: FontWeight.Bold, size: 12))
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Left = 80 },
            }, loaded =>
            {
                textFlow?.Expire();

                AddMetadata(metadata, loaded);

                textContainer.Add(textFlow = loaded);

                // fade in if we haven't yet.
                textContainer.FadeIn(TRANSITION_DURATION);
            });
        }

        protected abstract void AddMetadata(T metadata, LinkFlowContainer loaded);
    }
}
