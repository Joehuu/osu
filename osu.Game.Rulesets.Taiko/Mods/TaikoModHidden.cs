﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModHidden : ModHidden
    {
        public override string Description => @"Less length, and beats fade out before you hit them!";
        public override double ScoreMultiplier => 1.06;
    }
}
