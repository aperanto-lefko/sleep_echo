using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sleepApp.Model;
using System.Configuration;

namespace sleepApp.Service
{
    public class AppDbContext : DbContext
    //Этот класс является основой для работы с базой данных в приложении, использующем Entity Framework Core и PostgreSQL.
    {
        public DbSet<Respondent> Respondents { get; set; }
        public DbSet<SleepData> SleepData { get; set; }
        //DbSet<T> — это коллекция сущностей (записей) в контексте базы данных. Он представляет таблицу в базе данных.
        private readonly string _connectionString;
        public AppDbContext(string userName, string password, string port, string dataBase, string host)
        {
            var connectionStringTemp = ConfigurationManager.ConnectionStrings["SleepProductivityDB"].ConnectionString;
            _connectionString = connectionStringTemp
                .Replace("{username}", userName)
                .Replace("{password}", password)
                .Replace("{port}", port)
                .Replace("{dataBase}", dataBase)
                .Replace("{host}", host);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            //Используется для настройки параметров подключения к базе данных. В данном случае, используется метод
            //UseNpgsql, который указывает, что приложение будет работать с базой данных PostgreSQL.
        }
        
    }
}

