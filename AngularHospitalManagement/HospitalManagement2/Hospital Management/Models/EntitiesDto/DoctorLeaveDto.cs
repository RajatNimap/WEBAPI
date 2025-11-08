using Hospital_Management.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management.Models.EntitiesDto
{
    public class DoctorLeaveDto
    {

    
        public int DoctorsId { get; set; }
        public DateOnly Leave { get; set; }
        public string Reason { get; set; }
    }
}
