using System;

namespace API.Services;

public class FileUpload
{
    public static async Task<string> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is null or empty", nameof(file));

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadFolder, fileName);

        if(!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
}
