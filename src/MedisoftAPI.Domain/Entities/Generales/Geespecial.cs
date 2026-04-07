using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedisoftAPI.Domain.Entities.Generales
{
    [Table("Geespecial")]
    public class Geespecial
    {
        public string Geespecodi { get; set; }
        public string Geespenomb { get; set; }
        public int? Geespesv18 { get; set; }
        public int? Geespeodon { get; set; }
        public int? Hcrevartip { get; set; }
        public int? geespechbx { get; set; }
    }
}
