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
                TempData["Upload"] = "Succesfully Uploaded";
            }
            catch (HttpRequestException ex)
            {
                TempData["Upload"] = "Host is not available,please try again later";
            }
            catch (Exception ex)
            {
                TempData["Upload"] = ex.Message.ToString();
            }
            
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Generate(string search=null)
        {
            List<VCard> vCards =new List<VCard>();
            if (search==null)
            {
                vCards = await _context.VCards.ToListAsync();
            }
            else
            {
                vCards = await _context.VCards.Where(x=>x.Firstname.ToLower().Contains(search.ToLower()) 
                || x.Lastname.ToLower().Contains(search.ToLower())
                || x.Email.Contains(search.ToLower()) || x.Phone.Contains(search)|| x.Country.ToLower().Contains(search.ToLower())
                || x.City.ToLower().Contains(search.ToLower())).ToListAsync();
            }
            
            if (vCards == null) return NotFound();
            foreach (var card in vCards)
            {
                if (card.ExistQRPath == null)
                {
                    GeneratedBarcode Qrcode = QRCodeWriter.CreateQrCode($"" +
                        $"BEGIN:VCARD" +
                        $"\nN: {card.Lastname}; {card.Firstname}; ; Mr." +
                        $"\nFN:{card.Lastname} {card.Firstname}" +
                        $"\nTEL: {card.Phone}" +
                        $"\nCOUNTRY:{card.Country}" +
                        $"\nCITY:{card.City}" +
                        $"\nEMAIL:{card.Email}" +
                        $"\nEND:VCARD",500, QRCodeWriter.QrErrorCorrectionLevel.Low);

                    Qrcode.SaveAsPng($"{_env.WebRootPath}/img/{card.Id}.png");
                    card.ExistQRPath = $"/img/{card.Id}.png";
                 }
        }
            await _context.SaveChangesAsync();
            return View(vCards);
        }
    }
}