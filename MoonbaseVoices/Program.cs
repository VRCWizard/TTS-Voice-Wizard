using Microsoft.AspNetCore.Mvc;


/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MoonbaseAlpha;
using System.Runtime.InteropServices;



public class Program
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;
    const int SW_SHOWMINIMIZED = 2;
    public static void Main(string[] args)
    {
        Console.WriteLine("Moonbase Alpha Voice Synthesizer (x86)");
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_SHOWMINIMIZED);
        CreateHostBuilder(args).Build().Run();

       
        


        // Hide
        //ShowWindow(handle, SW_HIDE);

        // Show
        //  ShowWindow(handle, SW_SHOW);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://localhost:54027");
                webBuilder.UseStartup< MoonbaseAlpha.Startup>();
            });
  
}
