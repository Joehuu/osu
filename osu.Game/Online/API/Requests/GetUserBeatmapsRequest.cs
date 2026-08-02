// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Localisation;
using osu.Game.Extensions;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;

namespace osu.Game.Online.API.Requests
{
    public class GetUserBeatmapsRequest : PaginatedAPIRequest<List<APIBeatmapSet>>
    {
        private readonly long userId;

        private readonly BeatmapSetType type;

        public GetUserBeatmapsRequest(long userId, BeatmapSetType type, PaginationParameters pagination)
            : base(pagination)
        {
            this.userId = userId;
            this.type = type;
        }

        protected override string Target => $@"users/{userId}/beatmapsets/{type.ToString().ToSnakeCase()}";
    }

    public enum BeatmapSetType
    {
        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsFavouriteTitle))]
        Favourite,

        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsRankedTitle))]
        Ranked,

        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsLovedTitle))]
        Loved,

        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsGuestTitle))]
        Guest,

        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsPendingTitle))]
        Pending,

        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsGraveyardTitle))]
        Graveyard,

        [LocalisableDescription(typeof(UsersStrings), nameof(UsersStrings.ShowExtraBeatmapsNominatedTitle))]
        Nominated,
    }
}
