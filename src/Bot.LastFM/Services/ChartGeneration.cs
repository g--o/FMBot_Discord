using System;
using Bot.Domain.LastFM;
using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using IF.Lastfm.Core.Api;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bot.Domain.Enums;
using Bot.Logger.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Image = SixLabors.ImageSharp.Image;

namespace Bot.LastFM.Services
{
    public class ChartGeneration : IChartGeneration
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        private readonly ILogger _logger;

        public ChartGeneration(ILogger logger)
        {
            this._logger = logger;
        }

        public async Task<Image<Rgba32>> GenerateChartAsync(IReadOnlyList<Album> albums, LastFMTimespan timespan, int rows = 3, int columns = 3, bool showTitles = true)
        {
            var images = new List<Image<Rgba32>>();

            Parallel.ForEach(albums, async (album) =>
            {
                if (album.Images != null || album.Images?.Medium != null)
                {
                    images.Add(await FetchImageAsync(album.Images.Medium));
                }
            });

            int width;
            int height;

            double root = Math.Sqrt(rows * columns);

            using (var outputImage = new Image<Rgba32>(150 * columns, 150 * rows))
            {
                int offset = 0;
                int heightOffset = 0;

                for (var i = 1; i < images.Count + 1; i++)
                {
                    var image = images[i - 1];
                    var position = new Point(offset, heightOffset);
                    outputImage.Mutate(o => o
                        .DrawImage(image, position, 1f)
                    );

                    // next row
                    if ((i % root) == 0 && (i - 1) != 0)
                    {
                        offset = 0;
                        heightOffset += 150;
                    }
                }

                outputImage.Save("output.png");
            }

            return null;
        }

        private async Task<Image<Rgba32>> FetchImageAsync(Uri url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var inputStream = await response.Content.ReadAsStreamAsync();

            return Image.Load(inputStream);
        }
    }
}
