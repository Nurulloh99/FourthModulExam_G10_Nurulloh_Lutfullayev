namespace Bll.Services;

public interface IFileService
{
    public Task<byte[]> GenerateCVAsync(long botUserId);
}
