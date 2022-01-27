using MovieProDemo.Services.Interfaces;

namespace MovieProDemo.Services
{
    public class BasicImageService : IImageService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasicImageService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string DecodeImage(byte[] imagebytes, string contentType)
        {
            if(imagebytes == null)return null;
            var image = Convert.ToBase64String(imagebytes);
            return $"data:{contentType};base64,{image}";
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile image)
        {
            if (image == null) return null;

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeImageUrlAsync(string imageUrl)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(imageUrl);
            using Stream stream = await response.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await ms.CopyToAsync(stream);
            return ms.ToArray();
        }
    }
}
