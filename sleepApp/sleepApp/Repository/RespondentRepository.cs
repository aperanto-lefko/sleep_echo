using sleepApp.Model;
using sleepApp.Service;
using sleepApp.ExceptionType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sleepApp.Repository
{
    public class RespondentRepository
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
                return respondents;
            }
        }

        public Respondent AddRespondent(Respondent respondent)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                context.Respondents.Add(respondent);
                context.SaveChanges();
                return respondent;
            }
        }
        public int RemoveRespondent(Respondent respondent)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                context.Respondents.Remove(respondent);
                return context.SaveChanges();
            }
        }
        public Respondent? FindById(int id)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                return context.Respondents.Find(id);
            }
        }
    }
}

