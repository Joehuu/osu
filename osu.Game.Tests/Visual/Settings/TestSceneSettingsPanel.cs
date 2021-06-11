// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Users;

namespace osu.Game.Tests.Visual.Settings
{
    [TestFixture]
    public class TestSceneSettingsPanel : OsuTestScene
    {
        private readonly SettingsPanel settings;
        private readonly DialogOverlay dialogOverlay;

        public TestSceneSettingsPanel()
        {
            settings = new SettingsOverlay();

            Add(dialogOverlay = new DialogOverlay
            {
                Depth = -1
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Dependencies.Cache(dialogOverlay);

            Add(settings);

            AddStep("toggle settings overlay", () => settings.ToggleVisibility());

            AddStep("toggle supporter", () =>
            {
                ((DummyAPIAccess)API).LocalUser.Value = new User
                {
                    Username = API.LocalUser.Value.Username,
                    Id = API.LocalUser.Value.Id + 1,
                    IsSupporter = !API.LocalUser.Value.IsSupporter,
                };
            });
        }
    }
}
