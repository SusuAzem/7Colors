﻿namespace _7Colors.Services
{
    public class FileManager : IFileManager
    {
        private readonly string? _imagePath;
        public FileManager(IConfiguration config)
        {
            _imagePath = config["Path:Images"];
        }
        public FileStream ImageStream(string image)
        {
            return new FileStream(Path.Combine(_imagePath!, image), FileMode.Open, FileAccess.Read);
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {
                var save_path = Path.Combine(_imagePath!);
                if (!Directory.Exists(_imagePath))
                {
                    Directory.CreateDirectory(save_path);
                }
                var mine = image.FileName[image.FileName.LastIndexOf('.')..];
                var fileName = $"img_{DateTime.Now:dd-MM-yyyy-HH-mm-ss}{mine}";

                using (var filestream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    await image.CopyToAsync(filestream);
                }
                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
        }     
    }    
}
