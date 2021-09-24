using System.ComponentModel.DataAnnotations;

namespace MET.Data.Models
{
    public class RenameManufacturerModel
    {
        [Key]
        public int Id { get; set; }

        public string From { get; set; }
        public string To { get; set; }
    }
}
