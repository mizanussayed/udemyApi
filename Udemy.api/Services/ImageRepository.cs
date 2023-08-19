namespace Udemy.api.Services;
public class ImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment env;
    private readonly IHttpContextAccessor _contextAccessor;

    public ImageRepository(IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
    {
        this.env = env;
        _contextAccessor = contextAccessor;
    }
    public bool IsValid(string fileName)
    {
        var allowExtension = new string []{".jpg", ".png", ".jpeg"};
        if(allowExtension.Contains(fileName))
           return true;
        return false;
    }

    public async Task<string> UpladImageToDir(IFormFile img)
    {
        var fileExt = Path.GetExtension(img.FileName);
        var fileName = img.FileName;
        if(IsValid(fileExt)){
        var saveToUrl = Path.Combine(env.ContentRootPath, "Images", fileName);
        using var stream = new FileStream(saveToUrl, FileMode.Create, FileAccess.Write);
        await img.CopyToAsync(stream);

        var req = _contextAccessor.HttpContext?.Request;
        //https://localhost:5000/images/abc.png;
        var fileUrl = $"{req?.Scheme}://{req?.Host}{req?.PathBase}/images/{fileName}";
        return fileUrl;
        }else{
            return string.Empty;
        }
    }
}