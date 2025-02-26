using AutoMapper;
using sleepApp.Dto;
using sleepApp.ExceptionType;
using sleepApp.Model;
using sleepApp.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace sleepApp.Service
{
    public class RespodentService
    {
        private readonly RespondentRepository _respondentRepository;
        private readonly IMapper _mapper;

        public RespodentService(string login, string password)
        {
            _respondentRepository = new RespondentRepository(login, password);
            var config = new MapperConfiguration(config => config.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        public List<Respondent> GetAllRespondents()
        {
            return _respondentRepository.GetAllRespondents();
        }

        public List<Respondent> GetRespondentByLastName(string name)
        {
            return _respondentRepository.GetRespondentByLastName(name);
        }

        public Respondent AddRespondent(string firstName,
                                                  string lastName,
                                                  string email,
                                                  string gender,
                                                  string country,
                                                  int age)
        {
            RespondentDto resp = GetNewRespondentDto(firstName,
                                                      lastName,
                                                      email,
                                                      gender,
                                                      country,
                                                      age);
            if (resp != null)
            {
                Respondent newResp = _mapper.Map<Respondent>(resp);
                return _respondentRepository.AddRespondent(newResp);

            }
            else
            {
                return null;
            }
        }
        public int RemoveRespondentById(int id)
        {

            var respondent = _respondentRepository.FindById(id);

            if (respondent == null)
            {
                throw new NotFoundException($"Пользователь с id={id} не найден");
            }
            return _respondentRepository.RemoveRespondent(respondent);
        }



        public RespondentDto GetNewRespondentDto(string firstName,
                                                  string lastName,
                                                  string email,
                                                  string gender,
                                                  string country,
                                                  int age)
        {
            try
            {
                ValidationTextFields(
                    firstName,
                    lastName,
                    country);

                return new RespondentDto(firstName,
                    lastName,
                    email,
                    gender,
                    country,
                    age);
            }
            catch (ValidationException e)
            {
                MessageBox.Show(e.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ValidationTextFields(params string[] fields)
        {
            foreach (var field in fields)

            {
                if (ContainsDigit(field))
                {
                    throw new ArgumentException("Текстовые поля не должны содержать цифр");
                }
            }
        }


        private bool ContainsDigit(string field)
        {
            return field.Any(char.IsDigit); //проверка содержит ли цифры
        }
    }
}

