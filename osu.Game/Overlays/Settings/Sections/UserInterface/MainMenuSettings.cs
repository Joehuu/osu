// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Online.API;
using osu.Game.Users;

namespace osu.Game.Overlays.Settings.Sections.UserInterface
{
    public class MainMenuSettings : SettingsSubsection
    {
        protected override string Header => "Main Menu";

        private IBindable<User> user;

        private SettingsEnumDropdown<BackgroundSource> backgroundSourceDropdown;

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config, IAPIProvider api)
        {
            user = api.LocalUser.GetBoundCopy();

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Interface voices",
                    Current = config.GetBindable<bool>(OsuSetting.MenuVoice)
                },
                new SettingsCheckbox
                {
                    LabelText = "osu! music theme",
                    Current = config.GetBindable<bool>(OsuSetting.MenuMusic)
                },
                new SettingsEnumDropdown<IntroSequence>
                {
                    LabelText = "Intro sequence",
                    Current = config.GetBindable<IntroSequence>(OsuSetting.IntroSequence),
                },
                backgroundSourceDropdown = new SettingsEnumDropdown<BackgroundSource>
                {
                    LabelText = "Background source",
                    Current = config.GetBindable<BackgroundSource>(OsuSetting.MenuBackgroundSource),
                },
                new SettingsEnumDropdown<SeasonalBackgroundMode>
                {
                    LabelText = "Seasonal backgrounds",
                    Current = config.GetBindable<SeasonalBackgroundMode>(OsuSetting.SeasonalBackgroundMode),
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            backgroundSourceDropdown.Current.BindValueChanged(_ => updateBackgroundSourceWarning(), true);

            user.BindValueChanged(_ => updateBackgroundSourceWarning());
        }

        private void updateBackgroundSourceWarning()
        {
            const string not_supporter_note = "Changes to this setting will only apply with an active osu!supporter tag.";
            const string epilepsy_warning = "Storyboard / video may contain scenes with rapidly flashing colours. Please take caution if you are affected by epilepsy.";

            string warningText = string.Empty;

            if (user.Value?.IsSupporter != true)
                warningText = not_supporter_note;
            else if (backgroundSourceDropdown.Current.Value == BackgroundSource.BeatmapWithStoryboard)
                warningText = epilepsy_warning;

            backgroundSourceDropdown.WarningText = warningText;
        }
    }
}
