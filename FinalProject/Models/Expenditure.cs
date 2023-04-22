using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Expenditure
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bu xana boş ola bilmez")]
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Bu xana boş ola bilmez")]

        public float Money { get; set; }

        public DateTime ExpenditureTime { get; set; }

        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
    }
}
