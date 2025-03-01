using sleepApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace sleepApp.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RespondentDto, Respondent>();
            CreateMap<Respondent, RespondentDto>();
            CreateMap<UpdateRespondentRequest, Respondent>()
              .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SleepDataDto, SleepData>()
                .ForMember(dest => dest.respondent, opt => opt.Ignore()); //не маппим respondent
            CreateMap<SleepData, SleepDataDto>();

            CreateMap<UpdateSleepDataRequest, SleepData>()
                .ForMember(dest => dest.respondent, opt => opt.Ignore()) //не маппим respondent
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); /*
                                                                                                    * src: исходный объект
                                                                                                    dest: целевой объект
                                                                                                    srcMember: значение поля в исходном объекте.
                                                                                                    Условие srcMember != null означает, что поле будет обновлено только если 
                                                                                                    значение в исходном объекте не равно null.
                                                                                                    srcMember != default - для числовых 0, для ссылочный null
                                                                                                    */
        }
    }
}
