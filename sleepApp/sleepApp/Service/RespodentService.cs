using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sleepApp.Dto;
using sleepApp.ExceptionType;
using sleepApp.Model;
using sleepApp.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
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
            try
            {
                return _respondentRepository.GetAllRespondents();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<Respondent> GetRespondentByLastName(string name)
        {
            try
            {
                return _respondentRepository.GetRespondentByLastName(name);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public RespondentDto AddRespondent(string firstName,
                                                  string lastName,
                                                  string email,
                                                  string gender,
                                                  string country,
                                                  int age)
        {
            try
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
                    return _mapper.Map<RespondentDto>(_respondentRepository.AddRespondent(newResp));
                }
                else
                {
                    return null;
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        public bool RemoveRespondentById(int id)
        {
            var respondent = GetRespondentById(id);

            if (respondent == null)
            {
                return false;
            }
            _respondentRepository.RemoveRespondent(respondent);
            return true;

        }
        public Respondent? GetRespondentById(int id)
        {
            /* try
             {
                 var respondent = _respondentRepository.FindById(id);

                 if (respondent == null)
                 {
                     MessageBox.Show($"Пользователь с id={id} не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                     return null;
                 } else
                 {
                     return respondent;
                 }
             }
             catch (DbUpdateException ex)
             {
                 MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                 return null;
             }*/
            return _respondentRepository.FindById(id);
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

