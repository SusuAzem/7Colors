namespace _7Colors.Services
{
    public interface IFileManager
    {
        FileStream ImageStream(string image);
        Task<string> SaveImage(IFormFile image);
    }
}
