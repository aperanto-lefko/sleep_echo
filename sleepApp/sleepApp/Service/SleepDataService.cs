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
    }
}
