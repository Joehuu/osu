// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Chat;
using osu.Game.Users.Drawables;

namespace osu.Game.Graphics.Containers
{
    public partial class UserLinkCompiler : DrawableLinkCompiler
    {
        private readonly APIUser user;

        public UserLinkCompiler(ITextPart textPart, APIUser user)
            : base(textPart)
        {
            this.user = user;
        }

        public override ITooltip GetCustomTooltip() => new UserCardTooltip();

        public override object TooltipContent => user;
    }
}
