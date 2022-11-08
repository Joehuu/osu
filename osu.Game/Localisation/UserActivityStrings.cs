// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class UserActivityStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.UserActivity";

        /// <summary>
        /// "Modding a map"
        /// </summary>
        public static LocalisableString ModdingAMap => new TranslatableString(getKey(@"modding_amap"), @"Modding a map");

        /// <summary>
        /// "Choosing a beatmap"
        /// </summary>
        public static LocalisableString ChoosingABeatmap => new TranslatableString(getKey(@"choosing_abeatmap"), @"Choosing a beatmap");

        /// <summary>
        /// "{0} with others"
        /// </summary>
        public static LocalisableString WithOthers(LocalisableString arg0) => new TranslatableString(getKey(@"with_others"), @"{0} with others", arg0);

        /// <summary>
        /// "Watching others {0}"
        /// </summary>
        public static LocalisableString WatchingOthers(CaseTransformableString arg0) => new TranslatableString(getKey(@"watching_others"), @"Watching others {0}", arg0);

        /// <summary>
        /// "Editing a beatmap"
        /// </summary>
        public static LocalisableString EditingABeatmap => new TranslatableString(getKey(@"editing_abeatmap"), @"Editing a beatmap");

        /// <summary>
        /// "Spectating a game"
        /// </summary>
        public static LocalisableString SpectatingAGame => new TranslatableString(getKey(@"spectating_agame"), @"Spectating a game");

        /// <summary>
        /// "Looking for a lobby"
        /// </summary>
        public static LocalisableString LookingForALobby => new TranslatableString(getKey(@"looking_for_alobby"), @"Looking for a lobby");

        /// <summary>
        /// "In a lobby"
        /// </summary>
        public static LocalisableString InALobby => new TranslatableString(getKey(@"in_alobby"), @"In a lobby");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}