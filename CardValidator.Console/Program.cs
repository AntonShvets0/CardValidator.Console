using System.Net.Http.Headers;
using CardValidator.Console.Models.Entities;
using CardValidator.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

// ВНИМАНИЕ: appsettings.json содержит ТЕСТОВЫЕ ключи API и адреса.
// В реальном проекте этот файл должен быть добавлен в .gitignore,
// а чувствительные данные должны храниться в переменных окружения,
// секретах пользователя или другом защищенном хранилище.
// Это сделано для упрощения проверки тестового задания.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.Configure<PaymentServiceOptions>(builder.Configuration.GetSection("PaymentService"));
builder.Services.AddHttpClient<CardsValidationService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<PaymentServiceOptions>>().Value;
    client.BaseAddress = new Uri(options.Host);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("kid", options.KeyId);
});

var app = builder.Build();
var cardValidationService = app.Services.GetRequiredService<CardsValidationService>();

while (true)
{
    Console.WriteLine("Enter card number to verify (or 'exit' to exit):");
    string? input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
        break;
    
    try
    {
        bool result = await cardValidationService.ValidateCardAsync(input);
        Console.WriteLine(result ? "Successfully" : "Unsuccessfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error while validation card: {ex.Message}");
    }
    
    Console.WriteLine();
}