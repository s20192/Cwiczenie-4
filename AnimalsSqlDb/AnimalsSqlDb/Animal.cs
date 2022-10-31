using System.ComponentModel.DataAnnotations;

namespace AnimalsSqlDb
{
    public class Animal
    {
        [Required]
        public int IdAnimal { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kategoria jest wymagana")]
        [MaxLength(200)]
        public string Category { get; set; }

        [Required(ErrorMessage = "Miejsce wystepowania jest wymagane")]
        [MaxLength(200)]
        public string Area { get; set; }
    }
}
