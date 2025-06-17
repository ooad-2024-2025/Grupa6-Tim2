namespace DressCode.Models
{
    public class EmailResult
    {
        public int TotalRecipients { get; set; }
        public int SuccessfulSends { get; set; }
        public int FailedSends { get; set; }
        public List<string> FailedEmails { get; set; } = new List<string>();
        public List<string> SuccessfulEmails { get; set; } = new List<string>();
        public bool IsSuccess => FailedSends == 0 && TotalRecipients > 0;
        public string Summary => $"Ukupno: {TotalRecipients}, Uspešno: {SuccessfulSends}, Neuspešno: {FailedSends}";
    }
}