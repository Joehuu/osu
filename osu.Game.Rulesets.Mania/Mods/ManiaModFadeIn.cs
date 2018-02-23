﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Mania.Mods
{
    public class ManiaModFadeIn : Mod
    {
        public override string Name => "FadeIn";
        public override string ShortenedName => "FI";
        public override FontAwesome Icon => FontAwesome.fa_osu_mod_hidden;
        public override ModType Type => ModType.DifficultyIncrease;
        public override string Description => "The notes appear out of nowhere!" //edit if bad
        public override double ScoreMultiplier => 1;
        public override bool Ranked => true;
        public override Type[] IncompatibleMods => new[] { typeof(ModFlashlight) };
    }
}
