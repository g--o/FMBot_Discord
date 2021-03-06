﻿using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Drawing;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;

namespace FMBot.Bot.Models
{
    public class LastFMModels
    {
        public class FMBotChart
        {
            public PageResponse<LastAlbum> albums;
            public string time;
            public string LastFMName;
            public int max;
            public int rows;
            public List<Bitmap> images;
            public IUser DiscordUser;
            public DiscordSocketClient disclient;
            public int mode;
            public bool titles;
        }
    }
}
