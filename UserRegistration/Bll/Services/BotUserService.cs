using Dal.Entities;
using Dal;
using Microsoft.EntityFrameworkCore;

namespace Bll.Services;

public class BotUserService : IBotUserService
{
    private readonly MainContext mainContext;

    public BotUserService(MainContext mainContext)
    {
        this.mainContext = mainContext;
    }

    public async Task AddUserAsync(BotUser user)
    {
        var dbUser = await mainContext.Users.FirstOrDefaultAsync(x => x.TelegramUserId == user.TelegramUserId);

        if (dbUser != null)
        {
            Console.WriteLine($"User with id : {user.TelegramUserId} already exists");
            return;
        }

        try
        {
            await mainContext.Users.AddAsync(user);
            await mainContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public Task<List<BotUser>> GetAllUsersAsync()
    {
        var users = mainContext.Users.ToListAsync();
        return users;
    }

    public async Task<BotUser> GetBotUserByTelegramUserIdAsync(long telegramUserId)
    {
        var botUser = await mainContext.Users.FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);

        if (botUser == null)
        {
            Console.WriteLine($"User with telegram id is not fount : {telegramUserId}");
        }

        return botUser;
    }

    public async Task<long> GetBotUserIdByTelegramUserIdAsync(long telegramUserId)
    {
        var botUserId = await mainContext.Users
        .Where(u => u.TelegramUserId == telegramUserId)
        .Select(u => (long?)u.BotUserId)
        .FirstOrDefaultAsync();

        if (botUserId == null)
        {
            Console.WriteLine($"User with Telegram ID {telegramUserId} not found.");
        }

        return botUserId ?? 0;
    }

    public async Task UpdateUserAsync(BotUser user)
    {
        var dbUser = await mainContext.Users.FirstOrDefaultAsync(x => x.TelegramUserId == user.TelegramUserId);
        dbUser = user;
        mainContext.Users.Update(dbUser);
        await mainContext.SaveChangesAsync();
    }
}
