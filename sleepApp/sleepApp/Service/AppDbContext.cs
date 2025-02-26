using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sleepApp.Model;

namespace sleepApp.Service
{
    public class AppDbContext : DbContext
    //Этот класс является основой для работы с базой данных в приложении, использующем Entity Framework Core и PostgreSQL.
    {
        public DbSet<Respondent> Respondents { get; set; }
        //DbSet<T> — это коллекция сущностей (записей) в контексте базы данных. Он представляет таблицу в базе данных.
        private readonly string _connectionString;
        public AppDbContext(string userName, string password)
        {
            _connectionString = $"Host = localhost; Port=5432; database = sleep_productivity; UserName = {userName}; password = {password}";
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            //Используется для настройки параметров подключения к базе данных. В данном случае, используется метод
            //UseNpgsql, который указывает, что приложение будет работать с базой данных PostgreSQL.
        }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder) //// Настройка первичного ключа
        {
           modelBuilder.Entity<Respondent>(entity =>
            {
                entity.ToTable("respondents", "sleep");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.Age).HasColumnName("age");
                entity.Property(r => r.FirstName).HasColumnName("first_name");
                entity.Property(r => r.LastName).HasColumnName("last_name");
                entity.Property(r => r.Email).HasColumnName("email");
                entity.Property(r => r.Email).HasColumnName("email");
                entity.Property(r => r.Gender).HasColumnName("gender");
                entity.Property(r => r.Country).HasColumnName("country");

            });
        
        }*/
    }
}

/*
 *  
 * 
 * 
 * 
 * 
 */

