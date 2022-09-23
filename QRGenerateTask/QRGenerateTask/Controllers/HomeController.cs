using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRGenerateTask.DAL;
using QRGenerateTask.Models;
using System.Diagnostics;

namespace QRGenerateTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger,AppDbContext context,IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public async Task<IActionResult> Upload()
        {
        Request:
            HttpClient http = new HttpClient();
            try
            {
                var response = await http.GetAsync("https://randomuser.me/api?results=50");
                List<VCard> vcards = new List<VCard>();
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var roots = JsonConvert.DeserializeObject<Root>(json);
                    foreach (var item in roots.results)
                    {
                        VCard card = new VCard
                        {
                            Firstname = item.name.first,
                            Lastname = item.name.last,
                            Country = item.location.country,
                            City = item.location.city,
                            Email = item.email,
                            Phone = item.phone,
                            ExistQRPath = null
                        };
                        vcards.Add(card);
                    }
                    await _context.VCards.AddRangeAsync(vcards);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Respones are not successfull,Request has been sent again");
                    goto Request;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Host is not available,please try again later");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok("Successfully Uploaded");
        }
        public async Task<IActionResult> Generate()
        {
            List<VCard> vCards =await _context.VCards.ToListAsync();
            if (vCards == null) return NotFound();
            foreach (var card in vCards)
            {
                if (card.ExistQRPath==null)
                {
                    GeneratedBarcode Qrcode = IronBarCode.QRCodeWriter.CreateQrCode($"{_env.WebRootPath}/img/");
                    Qrcode.SaveAsPng($"{card.Id}.png");
                    card.ExistQRPath = $"~/img/{card.Id}.png";
                }
            }
            await _context.SaveChangesAsync();
            return View(vCards);
        }
    }
}