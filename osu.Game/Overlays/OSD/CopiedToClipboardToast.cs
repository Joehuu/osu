// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.LocalisationExtensions;
using osu.Game.Localisation;

namespace osu.Game.Overlays.OSD
{
    public partial class CopiedToClipboardToast : Toast
    {
        public CopiedToClipboardToast()
            : base(CommonStrings.General, Resources.Localisation.Web.CommonStrings.ButtonsClickToCopyCopied.ToSentence(), "")
        {
        }
    }
}
