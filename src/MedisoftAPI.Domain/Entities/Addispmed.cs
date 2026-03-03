using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedisoftAPI.Domain.Entities
{
    [Table("Addispmed")] // Ajusta el nombre de la tabla según corresponda
    public class Addispmed
    {
        public string Addispcons { get; set; }
        public string Geespecodi { get; set; }
        public string Gemedicodi { get; set; }
        public string Faservcodi { get; set; }
        public string Adconscodi { get; set; }
        public DateTime? Addispfech { get; set; }
        public string Adhoraini { get; set; }
        public string Adhorafin { get; set; }
        public bool? Addispcita { get; set; }
        public bool? Addispplan { get; set; }
    }
}
