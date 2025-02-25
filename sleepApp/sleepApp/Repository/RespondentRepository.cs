using sleepApp.Model;
using sleepApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sleepApp.Repository
{
    class RespondentRepository
    {
        private readonly string _login;
        private readonly string _password;

        public RespondentRepository(string login, string password)
        {
            _login = login;
            _password = password;
        }

        public List<Respondent> GetAllRespondents()
        {
            using (var context = new AppDbContext(_login, _password))
            {
                var respondents = context.Respondents.OrderBy(r => r.Id).ToList();
                MessageBox.Show($"Загружено {respondents.Count} пользователей."); // Отладочное сообщение
                return respondents;
            }
        }

        public List<Respondent> GetRespondentByLastName(string lastName)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                var respondents = context.Respondents
                    .Where(r => r.LastName.ToLower().StartsWith(lastName.ToLower()))
                    .ToList();
                MessageBox.Show($"Найдено {respondents.Count} пользователей."); // Отладочное сообщение
                return respondents;
            }
        }
    }
}

