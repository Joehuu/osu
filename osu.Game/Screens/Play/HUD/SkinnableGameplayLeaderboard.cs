// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;
using osu.Game.Screens.OnlinePlay.Multiplayer.Spectate;
using osu.Game.Skinning;

namespace osu.Game.Screens.Play.HUD
{
    public partial class SkinnableGameplayLeaderboard : CompositeDrawable, ISerialisableDrawable
    {
        public bool UsesFixedAnchor { get; set; }
        public bool IsEditable => false;

        [SettingSource("Has shear")]
        public BindableBool HasShear { get; } = new BindableBool();

        public SkinnableGameplayLeaderboard()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(Player player)
        {
            LoadComponentAsync(getLeaderboardFor(player.GameplayLeaderboardType), leaderboard =>
            {
                if (!player.LoadedBeatmapSuccessfully)
                    return;

                InternalChild = leaderboard;
            });
        }

        private GameplayLeaderboard getLeaderboardFor(GameplayLeaderboardType type)
        {
            switch (type)
            {
                case GameplayLeaderboardType.Multiplayer:
                    return new MultiplayerGameplayLeaderboard();

                case GameplayLeaderboardType.MultiSpectator:
                    return new MultiSpectatorLeaderboard();

                case GameplayLeaderboardType.Solo:
                    return new SoloGameplayLeaderboard();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum GameplayLeaderboardType
    {
        Multiplayer,
        MultiplayerTeam,
        MultiSpectator,
        Solo,
    }
}
