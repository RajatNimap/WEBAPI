using System.Data;
using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Interfaces.Implementation
{
    public class DashboardImplementation : IReportDashboard
    {
        private readonly IConfiguration configuration;

        private readonly AppointmentRepository _appointmentRepository;
        private readonly DataContext _database; 
        public DashboardImplementation(IConfiguration configuration,
            AvailabilityRepository availabilityRepository,
            AppointmentRepository appointmentRepository,DataContext database)
        {
            this.configuration = configuration;
            _appointmentRepository = appointmentRepository;
            _database = database;   
        }
        public Task<DataTable> getDailyCount(DateOnly date)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("DailyCountbyDoctorAndDepartment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@givendate", date);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return Task.FromResult(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while fetching daily count data.", ex);

            }
        }

        public async Task<GetdailyCountdto> getPercentage(int doctorId,DateOnly date)
        {
            // This method is not implemented yet.  
           var Convertdate=date.ToDateTime(TimeOnly.MinValue);
            var bookings = await _appointmentRepository.GetBookedSlot(doctorId,Convertdate);
           
            var dayofWeek = (int)Convertdate.DayOfWeek;
            var totalslot = from avail in _database.doctorAvailabilities
                            where avail.DoctorId == doctorId && avail.IsAvailable && avail.DayOfWeek == dayofWeek
                            select new
                            {
                                avail.MorningStartTime,
                                avail.MorningEndTime,
                                avail.EveningStartTime,
                                avail.EveningEndTime
                            };

            var morningStart = TimeSpan.MinValue;
            var morningEnd = TimeSpan.MinValue;
            var eveningStart = TimeSpan.MinValue;
            var eveningEnd = TimeSpan.MinValue;

            foreach (var total in totalslot)
            {
                 morningStart = total.MorningStartTime;
                 morningEnd = total.MorningEndTime;
                 eveningStart = total.EveningStartTime;
                 eveningEnd = total.EveningEndTime;
               
            }
            var total1=morningEnd - morningStart;
            var total2 = eveningEnd - eveningStart;

           double totalhour1 = (double)total1.TotalHours;
            double totalhour2 = (double)total2.TotalHours; 

            totalhour1= totalhour1 * 2; // Morning and Evening slots
            totalhour1 = totalhour1 * 2; // 30 minutes slots    
            double count1 = totalhour1+totalhour2; // Total available slots in a day

            double count2 = bookings.Count;
            double percentage = (count2/ count1) * 100;
           
            var datafromlinq = from dept in _database.department 
                               join doc in _database.doctors on dept.Id equals doc.DepartmentId
                               where doc.Id == doctorId
                               select new GetdailyCountdto
                               {
                                   DepartmentName = dept.DepartmentName,
                                   DoctorName = doc.Name,
                                   percentage = percentage
                               };

          
           return await datafromlinq.FirstOrDefaultAsync();
        }

        public async Task<List<PatientFrequencyDto>> PatientFrequencyDto(int month)
        {
            //var data = await _database.appointments.FromSqlRaw($@"select p.Name,p.Age,p.Email,
            //                count(a.Id) as appoint
            //                from patients p join appointments a on p.Id =a.PatientId
            //                where MONTH(a.DateofAppointment) ={month}
            //                group by p.Name,p.Age ,p.Email order by appoint desc").ToListAsync();

            var data = (from p in _database.patients
                       join a in _database.appointments on p.Id equals a.PatientId
                       where a.DateofAppointment.Month == month
                       group new { p, a } by new { p.Name, p.Age, p.Email } into g
                        orderby g.Count() descending

                        select new PatientFrequencyDto
                       {
                           Name = g.Key.Name,
                            age=g.Key.Age,
                           Email = g.Key.Email,
                           frequency = g.Count()
                       }).ToListAsync();

            if (data == null)
            {
                return null; // No data found for the specified month
            }
            return await data;

          
        }
    }
}
