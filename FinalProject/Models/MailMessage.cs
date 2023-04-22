namespace FinalProject.Models
{
    public class MailMessage
    {
        public int Id { get; set; }
        public string MailTo { get; set; }
        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }
    }
}
