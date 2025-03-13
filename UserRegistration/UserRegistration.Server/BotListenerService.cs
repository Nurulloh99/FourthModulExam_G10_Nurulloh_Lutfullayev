using Bll.Services;
using Dal;
using Dal.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace UserRegistration.Server;

public class BotListenerService
{
    private static string botToken = "7982617349:AAGbMQVl7h-w_y9M0-PYZst36ntHZU1xlrw";
    private static TelegramBotClient botClient = new TelegramBotClient(botToken);
    private static IBotUserService botUserService;
    private static IUserInfoService userInfoService;

    public BotListenerService(IBotUserService _botUserService, IUserInfoService _userInfoService)
    {
        botUserService = _botUserService;
        userInfoService = _userInfoService;
    }



    public async Task StartBot()
    {
        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync
            );

        Console.WriteLine("Bot is runing");

        await SetBotCommands();

        Console.ReadKey();
    }


    static async Task SetBotCommands()
    {
        var commands = new[]
        {
        new BotCommand { Command = "start", Description = "Boshlash" }

    };

        await botClient.SetMyCommandsAsync(commands);
    }



    private static long currentUserId = 0;
    private static long currentUserId2 = 0;
    private static string currentStep = "";
    private static string firstName = "";
    private static string lastName = "";
    private static string age = "";
    private static string phone = "";
    private static string email = "";
    private static string address = "";


    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        var userObject = await botUserService.GetBotUserByTelegramUserIdAsync(update.Message.Chat.Id);


        if (update.Type == UpdateType.Message)
        {


            var message = update.Message;
            var user = message.Chat;

            Console.WriteLine($"{user.Id}, {user.Username}, {user.FirstName}, {message.Text}");



            if (message.Text == "/start")
            {

                if (userObject == null)
                {
                    userObject =new BotUser()
                    {
                        CreatedAt = DateTime.UtcNow,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsBlocked = false,
                        PhoneNumber = null,
                        TelegramUserId = user.Id,
                        UpdatedAt = DateTime.UtcNow,
                        Username = user.Username
                    };


                    await  botUserService.AddUserAsync(userObject);
                }
                else
                {
                    if (user.FirstName != userObject.FirstName || user.LastName != userObject.LastName || user.Username != userObject.Username)
                    {
                        userObject.UpdatedAt = DateTime.UtcNow;
                    };
                    userObject.FirstName = user.FirstName;
                    userObject.LastName = user.LastName;
                    userObject.Username = user.Username;
                    await botUserService.UpdateUserAsync(userObject);
                }



                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Register of User"),
                        new KeyboardButton("Get an instruction how to fill it")
                    },
                    new[]
                    {
                        new KeyboardButton("Remove User info")
                    }
                })
                { ResizeKeyboard = true };

                await bot.SendTextMessageAsync(user.Id, $"{user.FirstName} Welcome to Registration bot!\n {user.FirstName} Royxatdan otqazuvchi botga xush kelibsiz!", replyMarkup: keyboard, cancellationToken: cancellationToken);

            }

            if (message.Text == "Remove User info")
            {
                var userInformation = await botUserService.GetBotUserByTelegramUserIdAsync(user.Id);
                if (userInformation.UserInfo is null)
                {
                    await bot.SendTextMessageAsync(user.Id, "Informatsiyani O'chirish Uchun\nAvval Informatsiya Qo'shing", cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    await userInfoService.DeleteUserInfoAsync(userInformation.UserInfo.BotUserId);

                    await bot.SendTextMessageAsync(user.Id, "Informatsiya O'chirildi", cancellationToken: cancellationToken);
                }
            }






            if (message.Text == "Get an instruction how to fill it")
            {
                await bot.SendTextMessageAsync(user.Id,
                    $"❗️Ro'yxat quidagi ko'rinishda bo'lishi kerak❗️\n" +
                    $"\n First name: " +
                    $"\n Last name: " +
                    $"\n Age: " +
                    $"\n Phone number " +
                    $"\n Email " +
                    $"\n Address " +
                    $"\n\n ===================" +
                    $"\n\n Example:" +
                    $"\n\n First name:  Nick" +
                    $"\n Last name:  Nelson" +
                    $"\n Age:  25" +
                    $"\n Phone number:  +14242837985" +
                    $"\n Email:  nicknelsonfds@gmail.com" +
                    $"\n Address:  Avenel.NJ, Alber Ave St, USA", cancellationToken: cancellationToken);

            }


            if (message.Text == null) return;

            long userId = currentUserId;
            string userMessage = message.Text;

            if (userMessage == "Register of User")
            {
                currentUserId = update.Message.Chat.Id;
                currentUserId2 = userObject.BotUserId;
                currentStep = "askFirstName";
                firstName = "";
                lastName = "";
                age = "";
                phone = "";
                email = "";
                address = "";
                await bot.SendTextMessageAsync(userId, "Ismingizni kiriting:");
                return;
            }

            if (currentStep == "askFirstName")
            {
                firstName = userMessage;
                currentStep = "askLastName";
                await bot.SendTextMessageAsync(userId, "Familiyangizni kiriting:");
            }


            else if (currentStep == "askLastName")
            {
                lastName = userMessage;
                currentStep = "askAge";
                await bot.SendTextMessageAsync(userId, "Yoshingizni kiriting:");
            }


            else if (currentStep == "askAge")
            {
                age = userMessage;
                currentStep = "askPhoneNumber";
                await bot.SendTextMessageAsync(userId, "Tel raqamingizni kiriting:");
            }

            else if (currentStep == "askPhoneNumber")
            {
                phone = userMessage;
                currentStep = "askEmail";
                await bot.SendTextMessageAsync(userId, "Emailingizni kiriting:");
            }

            else if (currentStep == "askEmail")
            {
                email = userMessage;
                currentStep = "askAddress";
                await bot.SendTextMessageAsync(userId, "Yashash manzilingizni kiriting:");
            }

            else if (currentStep == "askAddress")
            {
                address = userMessage;
                currentStep = "completed";

                await bot.SendTextMessageAsync(userId, $"Ro‘yxatdan muvaffaqqiyatli o‘tdingiz ${user.FirstName}!\n" +
                    $"\nIsmingiz: {firstName}" +
                    $"\nFamiliyangiz: {lastName}" +
                    $"\nYoshingiz: {age}" +
                    $"\nTelefon raqamingiz: {phone}" +
                    $"\nEmailingiz: {email}" +
                    $"\nYashash manzilingiz: {address}");

                var userInfo = new UserInfo();
                userInfo.BotUserId = currentUserId2;
                userInfo.FirstName = firstName;
                userInfo.LastName = lastName;
                userInfo.Email = email;
                userInfo.PhoneNumber = phone;
                userInfo.Address = address;
                userInfo.Age = age;

                await userInfoService.AddUserInfoAsync(userInfo);

                currentUserId = 0;
                currentStep = "";
                firstName = "";
                lastName = "";
                age = "";
                phone = "";
                email = "";
                address = "";
            }

        }

    }




    static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) 
    {

    }
}
