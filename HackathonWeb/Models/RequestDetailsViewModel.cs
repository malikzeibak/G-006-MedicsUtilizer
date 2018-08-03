using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackathonWeb.Models
{
    public class RequestDetailsViewModel
    {
        public string Type { get; set; }
        public string Comment { get; set; }
        public int VitalSignA { get; set; }
        public int VitalSignB { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ParamedicName { get; set; }
        public string GPS { get; set; }
        public string RequestStatus { get; set; }
        public int RequestID{ get; set; }
        public bool DiseaseA { get; set; }
        public bool DiseaseB { get; set; }
        public bool DiseaseC { get; set; }
        public string Duration { get; set; }
    }
}