using WebSocketApp.App_Source;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseWebSockets(wsConfig.GetOptions());
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        if (context.Request.Path == wsConfig.wschatPath)
        {
            wsFunction wsf = new wsFunction();
            await wsf.ListenAcceptAsync(context);
        }
    }
    else
    {
        await next();
    }
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
