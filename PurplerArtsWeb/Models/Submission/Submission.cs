using System.ComponentModel.DataAnnotations;

namespace PurplerArtsWeb.Models.Submission
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Annoymous";

        [Required]
        public string Decription { get; set; } = "Default string";

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmittedDate { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "Default string";

        //1. how do i implement "include images" in .net application
        //2. "include links" for references
        //

        //
        // I will come up with user related detials like shipping address later
        //
        // [Required]
        // public string SSN { get; set; } = "Default string";
        // public string Address { get; set; } = "Default string";
        // public string City { get; set; } = "Default string";
        // public string State { get; set; } = "Default string";
        // [Required]
        // public int PostalCode { get; set; }





    }
}