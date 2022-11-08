// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets;
using osuTK.Graphics;
using osu.Game.Localisation;

namespace osu.Game.Users
{
    public abstract class UserActivity
    {
        public abstract LocalisableString Status { get; }
        public virtual Color4 GetAppropriateColour(OsuColour colours) => colours.GreenDarker;

        public class Modding : UserActivity
        {
            public override LocalisableString Status => UserActivityStrings.ModdingAMap;
            public override Color4 GetAppropriateColour(OsuColour colours) => colours.PurpleDark;
        }

        public class ChoosingBeatmap : UserActivity
        {
            public override LocalisableString Status => UserActivityStrings.ChoosingABeatmap;
        }

        public abstract class InGame : UserActivity
        {
            public IBeatmapInfo BeatmapInfo { get; }

            public IRulesetInfo Ruleset { get; }

            protected InGame(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
            {
                BeatmapInfo = beatmapInfo;
                Ruleset = ruleset;
            }

            public override LocalisableString Status => Ruleset.CreateInstance().PlayingVerb;
        }

        public class InMultiplayerGame : InGame
        {
            public InMultiplayerGame(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
                : base(beatmapInfo, ruleset)
            {
            }

            public override LocalisableString Status => UserActivityStrings.WithOthers(base.Status);
        }

        public class SpectatingMultiplayerGame : InGame
        {
            public SpectatingMultiplayerGame(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
                : base(beatmapInfo, ruleset)
            {
            }

            public override LocalisableString Status => UserActivityStrings.WatchingOthers(base.Status.ToLower());
        }

        public class InPlaylistGame : InGame
        {
            public InPlaylistGame(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
                : base(beatmapInfo, ruleset)
            {
            }
        }

        public class InSoloGame : InGame
        {
            public InSoloGame(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
                : base(beatmapInfo, ruleset)
            {
            }
        }

        public class Editing : UserActivity
        {
            public IBeatmapInfo BeatmapInfo { get; }

            public Editing(IBeatmapInfo info)
            {
                BeatmapInfo = info;
            }

            public override LocalisableString Status => UserActivityStrings.EditingABeatmap;
        }

        public class Spectating : UserActivity
        {
            public override LocalisableString Status => UserActivityStrings.SpectatingAGame;
        }

        public class SearchingForLobby : UserActivity
        {
            public override LocalisableString Status => UserActivityStrings.LookingForALobby;
        }

        public class InLobby : UserActivity
        {
            public override LocalisableString Status => UserActivityStrings.InALobby;

            public readonly Room Room;

            public InLobby(Room room)
            {
                Room = room;
            }
        }
    }
}
