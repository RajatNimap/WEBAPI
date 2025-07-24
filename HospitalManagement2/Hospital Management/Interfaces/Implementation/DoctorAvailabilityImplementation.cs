using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital_Management.Services
{
    public class DoctorAvailabilityService : IDoctorAvaliability
    {
        private readonly DataContext _database;

        public DoctorAvailabilityService(DataContext database)
        {
            _database = database;
        }

      
        public async Task<DoctorAvailability> GetByIdAsync(int doctorId)
        {
           var data= await _database.doctorAvailabilities
                .FirstOrDefaultAsync(x => x.DoctorId == doctorId);
            return data;    
        }

        public async Task<DoctorAvailibilityDTO> CreateAsync(DoctorAvailibilityDTO dto)
        {
            var entity = new DoctorAvailability
                {
                    DoctorId = dto.DoctorId,
                    DayOfWeek = dto.DayOfWeek,
                    MorningStartTime = dto.MorningStartTime,
                    MorningEndTime = dto.MorningEndTime,
                    EveningStartTime = dto.EveningStartTime,
                    EveningEndTime = dto.EveningEndTime,
                IsAvailable = dto.IsAvailable
            };

            _database.doctorAvailabilities.Add(entity);
            await _database.SaveChangesAsync();

            return new DoctorAvailibilityDTO
            {
                DoctorId = entity.DoctorId,
                DayOfWeek = entity.DayOfWeek,
                MorningStartTime = entity.MorningStartTime,
                MorningEndTime = entity.MorningEndTime,
                EveningStartTime = entity.EveningStartTime,
                EveningEndTime = entity.EveningEndTime,
                IsAvailable = entity.IsAvailable
            };

        }

        public async Task<DoctorAvailibilityDTO> UpdateAsync(int doctorId, DoctorAvailibilityDTO dto)
        {
            var data= await _database.doctorAvailabilities
                .FirstOrDefaultAsync(x => x.DoctorId == doctorId);
            if (data == null)
            {
                return null; // Doctor availability not found   
            }

            var entity = new DoctorAvailability
            {
                DoctorId = dto.DoctorId,
                DayOfWeek = dto.DayOfWeek,
                MorningStartTime = dto.MorningStartTime,
                MorningEndTime = dto.MorningEndTime,
                EveningStartTime = dto.EveningStartTime,
                EveningEndTime = dto.EveningEndTime,
                IsAvailable = dto.IsAvailable
            };
            _database.doctorAvailabilities.Add(entity);
            await _database.SaveChangesAsync();

            return new DoctorAvailibilityDTO
            {
                DoctorId = entity.DoctorId,
                DayOfWeek = entity.DayOfWeek,
                MorningStartTime = entity.MorningStartTime,
                MorningEndTime = entity.MorningEndTime,
                EveningStartTime = entity.EveningStartTime,
                EveningEndTime = entity.EveningEndTime,
                IsAvailable = entity.IsAvailable
            };
        }

        public async Task<IEnumerable<DoctorAvailability>> GetAllAsync()
        {
            var data = await _database.doctorAvailabilities.ToListAsync();
            return data;
        }

       
    }
}
