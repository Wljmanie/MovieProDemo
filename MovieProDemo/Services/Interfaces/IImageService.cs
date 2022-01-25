namespace MovieProDemo.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile image);
        Task<byte[]> EncodeImageUrlAsync(string imageUrl);
        string DecodeImage(byte[] image, string contentType);
    }
}
