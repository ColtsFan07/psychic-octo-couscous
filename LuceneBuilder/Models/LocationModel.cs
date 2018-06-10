using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneBuilder.Models
{
    public class LocationModel
    {
        public int ID { get; set; }
        public string PLC_ID { get; set; }
        public string FRMTD_ADDR { get; set; }
        public string ZIP_CD { get; set; }
        public string NELAT { get; set; }
        public string NELON { get; set; }
        public string SWLAT { get; set; }
        public string SWLON { get; set; }
        public string BIZ_NM { get; set; }
    }
}
