using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Hospital_Management.Interfaces.Implementation
{
    public class PatientsImplementation : IPatients
    {
        private readonly DataContext Database;
        private readonly string Connectionstring;
      
        public PatientsImplementation(DataContext database,IConfiguration configuration) {
        
                this.Database = database;
            Connectionstring = configuration.GetConnectionString("DefaultConnection");
        } 
        public async Task<List<PatientsModel>> GetPatientsModelsDetails()
        {
            var PatientDetail = await Database.patients.Include(x => x.Appointment).ToListAsync();

            if (PatientDetail == null) {
                return null;
            }

            return PatientDetail;
        }
       
        public async Task<PatientsModel> GetPatientsById(int id)
        {
            var PatientDetailById = await Database.patients.Include(x=>x.Appointment).FirstOrDefaultAsync(x => x.Id == id);
            if (PatientDetailById == null) { }

            if (PatientDetailById == null) { return null; }

            return PatientDetailById;   
        }

        public async Task<PatientsModel> InsertPatientDetail(PatientsDto patients)
        {
                var Data=new PatientsModel
                {
                        Name = patients.Name,   
                        Age = patients.Age,
                        Gender = patients.Gender,   
                        Address = patients.Address,
                        Disease = patients.Disease,
                    PhoneNumber = patients.PhoneNumber,
                    Email = patients.Email, 
                };   
            if(Data==null) { return null; }
            await Database.AddAsync(Data);  
            await Database.SaveChangesAsync();
            return Data;
        }

        public async Task<PatientsModel> UpdatePatientDetail(int id, PatientsDto patients)
        {
            var Data=await Database.patients.FirstOrDefaultAsync(x=>x.Id == id);
           

            if (Data == null)
            {
                return null;
            }
            Data.Name = patients.Name;  
            Data.Age = patients.Age;
            Data.Gender = patients.Gender;  
            Data.Address = patients.Address;    
            Data.Disease = patients.Disease;
            Data.PhoneNumber = patients.PhoneNumber;
            Data.Email = patients.Email;

            await Database.SaveChangesAsync();

            return Data;


        }
        public async Task<bool> DeletePatientDetail(int id)
        {
            var Data = await Database.patients.FirstOrDefaultAsync(x => x.Id == id);
            if (Data == null) {
                return false;
            }
             Database.patients.Remove(Data);
            await Database.SaveChangesAsync();
            return true;
        }

        public async Task<List<Appointment>> GetAppointmentDetail(int id)
        {

            var Data =await Database.appointments.Where(x=>x.PatientId==id).ToListAsync();
            if (Data == null) {

                return null;
            }

            return Data;    
        }
        
        public async Task<List<PatientsModel>> Searching([FromQuery]string value)
        {
            var Query= Database.patients.AsQueryable();
            // var Data= await Database.patients.FirstOrDefaultAsync(x=>x.)
            if (!string.IsNullOrEmpty(value))
            {
                value = value.ToLower();
                Query=Query.Where(x=>x.Name.ToLower() == value || x.Email.ToLower()==value || x.PhoneNumber == value);
               
            }
            var list = await Query.ToListAsync();   
            return list;
        }

        public Task<List<MedicalRecord>> GettingAllmedicalRecord(int patientId)
        {
            //var data = Database.medicalRecords.Where(x => x.PatientsModelID == patientId).ToListAsync();
            //return data;
            var data = (from medical in Database.medicalRecords
                       join appoint in Database.appointments
                           on medical.AppointmentId equals appoint.Id
                       join patient in Database.patients
                           on appoint.PatientId equals patient.Id
                       where appoint.PatientId == patientId
                       select medical).ToListAsync();

            return data;
        }

   
        public async Task<DataTable> GetAllRecordofPatientLinked(int patientId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(Connectionstring))
            {
                using (SqlCommand cmd = new SqlCommand("GetallpatientData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", patientId);

                    await conn.OpenAsync();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

    }
    
}
