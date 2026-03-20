using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using NoticeBoard_frontend.Handlers;
using NoticeBoard_frontend.Models;
using NoticeBoard_frontend.Services;

namespace NoticeBoard_frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = async context =>
                {
                    string? idToken = null;
                    if (context.TokenResponse?.Response != null &&
                        context.TokenResponse.Response.RootElement.TryGetProperty("id_token", out var tokenEl))
                    {
                        idToken = tokenEl.GetString();
                    }

                    if (string.IsNullOrEmpty(idToken)) return;
                    context.Properties.StoreTokens(
                    [
                        new AuthenticationToken
                        {
                            Name = "id_token",
                            Value = idToken
                        }
                    ]);

                    var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                    var baseUrl = config["ApiSettings:BaseUrl"];
                    if (string.IsNullOrEmpty(baseUrl)) return;

                    try
                    {
                        using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
                        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

                        var user = await http.GetFromJsonAsync<UserInfo>("api/users/me");
                        if (user != null)
                        {
                            context.Identity?.AddClaim(new Claim("internal_user_id", user.Id.ToString()));
                        }
                    }
                    catch
                    {
                        // API unavailable during login — user can still authenticate
                    }
                };
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<AuthTokenHandler>();

            builder.Services.AddHttpClient<IAnnouncementApiService, AnnouncementApiService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
            }).AddHttpMessageHandler<AuthTokenHandler>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Announcements/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Announcements}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }

}
