// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Screens.Play.HUD;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Osu.Skinning
{
    public class OsuSkinTransformer : SkinTransformer
    {
        public OsuSkinTransformer(ISkin skin)
            : base(skin)
        {
        }

        public override Drawable? GetDrawableComponent(ISkinComponentLookup lookup)
        {
            if (base.GetDrawableComponent(lookup) is Drawable c)
                return c;

            if (lookup is SkinComponentsContainerLookup containerLookup)
            {
                switch (containerLookup.Target)
                {
                    case SkinComponentsContainerLookup.TargetArea.MainHUDComponents:
                        return new DefaultSkinComponentsContainer(container =>
                        {
                            var leaderboard = container.OfType<SkinnableGameplayLeaderboard>().FirstOrDefault();

                            if (leaderboard != null)
                            {
                                leaderboard.Anchor = Anchor.CentreLeft;
                                leaderboard.Origin = Anchor.CentreLeft;
                                leaderboard.Position = new Vector2(44, 0);
                            }
                        })
                        {
                            Children = new Drawable[]
                            {
                                new SkinnableGameplayLeaderboard(),
                            }
                        };
                }

                return null;
            }

            return base.GetDrawableComponent(lookup);
        }
    }
}
