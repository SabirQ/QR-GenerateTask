namespace QRGenerateTask.Models
{
    public class VCard
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ExistQRPath { get; set; }=null;
    }
}
