namespace FinalProject.Models
{
    public class Cash
    {
        public int Id { get; set; }

        public float Balance { get; set; }

        public float LastMotifiedMoney { get; set; }

        public string LastModified { get; set; }

        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
