using Microsoft.EntityFrameworkCore;
using QRGenerateTask.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
IronBarCode.License.LicenseKey = "IRONBARCODE.SABIRGAME3.12771-33B584BB7D-DRTNHBWOVQMMMKL-HAVOHPMVNGJW-R44AQLTVCE4V-FEYB63DAMFUG-CL36ATFJRMRJ-ONNUQR-TPXK3XODRBOIEA-DEPLOYMENT.TRIAL-PJYSYA.TRIAL.EXPIRES.23.OCT.2022";

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
