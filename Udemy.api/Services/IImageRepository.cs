namespace Udemy.api.Services;
public interface IImageRepository
{
    bool IsValid(string fileName);
    Task<string> UpladImageToDir(IFormFile img);
}