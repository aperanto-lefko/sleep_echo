using sleepApp.Model;
using sleepApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sleepApp.Repository
{
    public class SleepDataRepository
    {
        private readonly string _login;
        private readonly string _password;

        public SleepDataRepository(string login, string password)
        {
            _login = login;
            _password = password;
        }
        public SleepData AddSleepData(SleepData data)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                context.SleepData.Add(data);
                context.SaveChanges();
                return data;
            }
        }
    }
}
