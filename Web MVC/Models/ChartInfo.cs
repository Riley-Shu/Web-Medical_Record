using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample05_Web.Models
{
    //參考自MSSQL Table
    [Table("PersonalInformation")]
    public class ChartInfo
    {
        [Column(name: "ChartNo")]
        [Required]
        [Key]
        public String chartNo { get; set; }

        [Column(name: "Name")]
        [Required]
        public String name { get; set; }

        [Column(name: "Sex")]
        public String? sex { get; set; }

        [Column(name: "Birthday")]
        public DateTime? birthday { get; set; }

        [Column(name: "VisitDate")]
        [Required]
        public DateTime? visitDate { get; set; }
    }
}
