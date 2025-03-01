using AutoMapper;
using sleepApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sleepApp.Dto;


using System.ComponentModel.DataAnnotations;
using System.Windows;
using sleepApp.Model;
using Microsoft.EntityFrameworkCore;
using sleepApp.ExceptionType;

namespace sleepApp.Service
{
    public class SleepDataService
    {
        private readonly SleepDataRepository _sleepDataRepository;
        private readonly RespodentService _rService;
        private readonly IMapper _mapper;

        public SleepDataService(string login, string password)
        {
            _sleepDataRepository = new SleepDataRepository(login, password);
            var config = new MapperConfiguration(config => config.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _rService = new RespodentService(login, password);
        }

        public SleepDataDto AddSleepData(int personId,
                                      double sleepStartTime,
                                      double sleepEndTime,
                                      double totalSleepHours,
                                      int sleepQuality,
                                      int exerciseMinutes,
                                      int caffeineIntakeMg,
                                      int screenTime,
                                      double workHours,
                                      int productivityScore,
                                      int moodScore,
                                      int stressLevel)
        {
          
                _rService.GetRespondentById(personId);
                SleepDataDto sleepDataDto = new SleepDataDto(personId,
                                                             sleepStartTime,
                                                             sleepEndTime,
                                                             totalSleepHours,
                                                             sleepQuality,
                                                             exerciseMinutes,
                                                             caffeineIntakeMg,
                                                             screenTime,
                                                             workHours,
                                                             productivityScore,
                                                             moodScore,
                                                             stressLevel);
                SleepData newSleepData = _mapper.Map<SleepData>(sleepDataDto);
                return _mapper.Map<SleepDataDto>(_sleepDataRepository.AddSleepData(newSleepData));
       
        }
        public bool UpdateSleepData(int dataId,
                                      int personId,
                                      double sleepStartTime,
                                      double sleepEndTime,
                                      double totalSleepHours,
                                      int sleepQuality,
                                      int exerciseMinutes,
                                      int caffeineIntakeMg,
                                      int screenTime,
                                      double workHours,
                                      int productivityScore,
                                      int moodScore,
                                      int stressLevel)
        {
            SleepData oldData = GetSleepDataById(dataId);
            UpdateSleepDataRequest request = GetNewDataRequest(dataId,
                                                             oldData.Date, //оставляем дату неизменной
                                                             personId,
                                                             sleepStartTime,
                                                             sleepEndTime,
                                                             totalSleepHours,
                                                             sleepQuality,
                                                             exerciseMinutes,
                                                             caffeineIntakeMg,
                                                             screenTime,
                                                             workHours,
                                                             productivityScore,
                                                             moodScore,
                                                             stressLevel);
            SleepData updatedSleepData = _mapper.Map<SleepData>(request);

            return _sleepDataRepository.UpdateSleepData(updatedSleepData);
        }
            

        public bool RemoveSleepDataById(int id)
        {
            var data = GetSleepDataById(id);
            return _sleepDataRepository.RemoveSleepData(data);
         
        }

        public SleepData GetSleepDataById(int id)
        {
            return _sleepDataRepository.FindById(id) ??
                throw new NotFoundException($"Данные с ID: {id} не найдены");
        }

        public List<SleepDataDto> GetSleepDataWithParameters(int respondentId,
                                                            double slStartTimeStart,
                                                            double slStartTimeEnd,
                                                            double slEndTimeStart,
                                                            double slEndTimeEnd,
                                                            double slTotalTimeStart,
                                                            double slTotalTimeEnd,
                                                            int slQualityStart,
                                                            int slQualityEnd,
                                                            int exerciseStart,
                                                            int exerciseEnd,
                                                            int coffeeStart,
                                                            int coffeeEnd,
                                                            int screenTimeStart,
                                                            int screenTimeEnd,
                                                            double workTimeStart,
                                                            double workTimeEnd,
                                                            int productivityStart,
                                                            int productivityEnd,
                                                            int moodStart,
                                                            int moodEnd,
                                                            int stressStart,
                                                            int stressEnd)
        {
            return _mapper.Map<List<SleepDataDto>>(_sleepDataRepository.GetSleepDataWithParameters(respondentId,
                                                                    slStartTimeStart,
                                                                    slStartTimeEnd,
                                                                    slEndTimeStart,
                                                                    slEndTimeEnd,
                                                                    slTotalTimeStart,
                                                                    slTotalTimeEnd,
                                                                    slQualityStart,
                                                                    slQualityEnd,
                                                                    exerciseStart,
                                                                    exerciseEnd,
                                                                    coffeeStart,
                                                                    coffeeEnd,
                                                                    screenTimeStart,
                                                                    screenTimeEnd,
                                                                    workTimeStart,
                                                                    workTimeEnd,
                                                                    productivityStart,
                                                                    productivityEnd,
                                                                    moodStart,
                                                                    moodEnd,
                                                                    stressStart,
                                                                    stressEnd));
        }
        private UpdateSleepDataRequest GetNewDataRequest(int dataId,
                                      DateOnly date,
                                      int personId,
                                      double sleepStartTime,
                                      double sleepEndTime,
                                      double totalSleepHours,
                                      int sleepQuality,
                                      int exerciseMinutes,
                                      int caffeineIntakeMg,
                                      int screenTime,
                                      double workHours,
                                      int productivityScore,
                                      int moodScore,
                                      int stressLevel)
        {
            
            return new UpdateSleepDataRequest(dataId, 
                                              date,
                                              personId,
                                              sleepStartTime,
                                              sleepEndTime,
                                              totalSleepHours,
                                              sleepQuality,
                                              exerciseMinutes,
                                              caffeineIntakeMg,
                                              screenTime,
                                              workHours,
                                              productivityScore,
                                              moodScore,
                                              stressLevel);
        }

    }
}
