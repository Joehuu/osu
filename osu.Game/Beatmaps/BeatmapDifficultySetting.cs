// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Beatmaps
{
    public class BeatmapDifficultySetting
    {
        public LocalisableString Name;
        public (float baseValue, float? adjustedValue) Value;
        public LocalisableString Description;
        public LocalisableString Acronym;
    }
}
