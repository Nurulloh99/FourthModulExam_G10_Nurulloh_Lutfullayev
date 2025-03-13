using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bll.Services;

public class UserInfoService : IUserInfoService
{
    private readonly MainContext mainContext;

    public UserInfoService(MainContext mainContext)
    {
        this.mainContext = mainContext;
    }

    public async Task<long> AddUserInfoAsync(UserInfo userInfo)
    {
        try
        {
            await mainContext.UserInfos.AddAsync(userInfo);
            await mainContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 0l;
        }

        return userInfo.UserInfoId;
    }

    public async Task<UserInfo> GetUserInfoByBotUserIdAsync(long botUserId)
    {
        var userInfo = await mainContext.UserInfos.FirstOrDefaultAsync(ui => ui.BotUserId == botUserId);
        return userInfo;
    }

    public async Task<long> GetUserInfoIdByBotUserIdAsync(long botUserId)
    {
        var userInfo = await mainContext.UserInfos.FirstOrDefaultAsync(ui => ui.BotUserId == botUserId);

        if (userInfo == null) return 0l;

        return userInfo.UserInfoId;
    }

    public async Task DeleteUserInfoAsync(long userId)
    {
        var userInfo = await GetUserInfoByBotUserIdAsync(userId);
        mainContext.UserInfos.Remove(userInfo);
        mainContext.SaveChanges();
    }
}
