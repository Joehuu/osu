﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapSet
{
    public partial class BeatmapAvailability : Container
    {
        private bool downloadDisabled => beatmapSet.Value?.Availability.DownloadDisabled ?? false;
        private bool hasExternalLink => !string.IsNullOrEmpty(beatmapSet.Value?.Availability.ExternalLink);

        private readonly LinkFlowContainer textContainer;

        public BeatmapAvailability()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Padding = new MarginPadding { Top = 10, Right = 20 };

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black.Opacity(0.6f),
                },
                textContainer = new LinkFlowContainer(t => t.Font = OsuFont.GetFont(size: 14))
                {
                    Direction = FillDirection.Full,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding(10),
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmapSet.BindValueChanged(_ =>
            {
                if (downloadDisabled || hasExternalLink)
                {
                    Show();
                    updateText();
                }
                else
                    Hide();
            }, true);
        }

        [Resolved]
        private Bindable<APIBeatmapSet> beatmapSet { get; set; } = null!;

        private void updateText()
        {
            textContainer.Clear();
            textContainer.AddParagraph(downloadDisabled
                ? BeatmapsetsStrings.AvailabilityDisabled
                : BeatmapsetsStrings.AvailabilityPartsRemoved, t => t.Colour = Color4.Orange);

            if (hasExternalLink)
            {
                textContainer.NewParagraph();
                textContainer.NewParagraph();
                textContainer.AddLink(BeatmapsetsStrings.AvailabilityMoreInfo, beatmapSet.Value.Availability.ExternalLink, creationParameters: t => t.Font = OsuFont.GetFont(size: 10));
            }
        }
    }
}
