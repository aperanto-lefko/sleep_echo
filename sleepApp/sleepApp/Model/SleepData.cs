using sleepApp.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sleepApp.Model
{
    [Table("sleep_data", Schema = "sleep")]
    public class SleepData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("date")]
        public DateOnly Date { get; set; }
        [Column("person_id")]
        public int PersonId { get; set; }
        [ForeignKey(nameof(PersonId)) ] //навигационное свойство, связывание с сущностью
        public Respondent respondent { get; set; }
        [Column("sleep_start_time")]
        public double SleepStartTime { get; set; }
        [Column("sleep_end_time")]
        public double SleepEndTime { get; set; }
        [Column("total_sleep_hours")]
        public double TotalSleepHours { get; set; }
        [Column("sleep_quality")]
        public int SleepQuality { get; set; }
        [Column("exercise_minutes")]
        public int ExerciseMinutes { get; set; }
        [Column("caffeine_intake_mg")]
        public int CaffeineIntakeMg { get; set; }
        [Column("screen_time_before_bed")]
        public int ScreenTime { get; set; }
        [Column("work_hours")]
        public double WorkHours { get; set; }
        [Column("productivity_score")]
        public int ProductivityScore { get; set; }
        [Column("mood_score")]
        public int MoodScore { get; set; }
        [Column("stress_level")]
        public int StressLevel { get; set; }

        
    }

}
