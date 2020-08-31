// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;
using osu.Game.Graphics;
using osu.Game.Input.Bindings;
using osu.Game.Online.API;
using osu.Game.Users;
using osu.Game.Users.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Overlays.Toolbar
{
    public class ToolbarUserButton : ToolbarOverlayToggleButton, IOnlineComponent
    {
        private readonly UpdateableAvatar avatar;

        [Resolved(CanBeNull = true)]
        private UserProfileOverlay profileOverlay { get; set; }

        [Resolved]
        private IAPIProvider api { get; set; }

        public ToolbarUserButton()
        {
            TooltipMain = "Profile";
            TooltipSub = "See your stats";

            Hotkey = GlobalAction.ToggleUserProfile;

            AutoSizeAxes = Axes.X;

            DrawableText.Font = OsuFont.GetFont(italics: true);

            Add(new OpaqueBackground { Depth = 1 });

            Flow.Add(avatar = new UpdateableAvatar
            {
                Masking = true,
                Size = new Vector2(32),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 4,
                OpenOnClick = { Value = false },
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 4,
                    Colour = Color4.Black.Opacity(0.1f),
                }
            });
        }

        [BackgroundDependencyLoader(true)]
        private void load(LoginOverlay login)
        {
            api.Register(this);

            StateContainer = login;
        }

        public void APIStateChanged(IAPIProvider api, APIState state)
        {
            switch (state)
            {
                default:
                    Text = @"Guest";
                    avatar.User = new User();
                    break;

                case APIState.Online:
                    Text = api.LocalUser.Value.Username;
                    avatar.User = api.LocalUser.Value;
                    break;
            }
        }

        public override bool OnPressed(GlobalAction action)
        {
            if (action == Hotkey && api.IsLoggedIn)
                profileOverlay?.ShowUser(api.LocalUser.Value);

            return false;
        }
    }
}
