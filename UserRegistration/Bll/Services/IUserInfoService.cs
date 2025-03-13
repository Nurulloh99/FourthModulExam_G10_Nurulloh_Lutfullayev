using Dal.Entities;

namespace Bll.Services;

public interface IUserInfoService
{
    Task<long> AddUserInfoAsync(UserInfo userInfo);
    Task DeleteUserInfoAsync(long userId);
    Task<long> GetUserInfoIdByBotUserIdAsync(long botUserId);
    Task<UserInfo> GetUserInfoByBotUserIdAsync(long botUserId);
}