using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string AuthorName { get; set; } = string.Empty;

        [Required]
        [StringLength(13, MinimumLength = 10)]
        public string IsbnNumber { get; set; } = string.Empty;

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime? PublicationDate { get; set; }

    }
}