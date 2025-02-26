using sleepApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace sleepApp.Dto
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {
            CreateMap<RespondentDto, Respondent>();
            CreateMap<Respondent, RespondentDto>();
        }
    }
}
