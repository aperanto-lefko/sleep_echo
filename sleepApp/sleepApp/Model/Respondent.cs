using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sleepApp.Model
{
    [Table("respondents", Schema = "sleep")]
    public class Respondent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("gender")]
        public string Gender { get; set; }
        [Column("country")]
        public string Country { get; set; }
        [Column("age")]
        public int Age { get; set; }

       
    }


}
