﻿using sleepApp.Dto;
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
        public int RemoveSleepData(SleepData data)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                context.SleepData.Remove(data);
                return context.SaveChanges();
            }
        }
        public SleepData? FindById(int id)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                return context.SleepData.Find(id);
            }
        }
        public List<SleepData> GetSleepDataWithParameters(int respondentId,
                                                           double slStartTimeStart,
                                                           double slStartTimeEnd,
                                                           double slEndTimeStart,
                                                           double slEndTimeEnd,
                                                           double slTotalTimeStart,
                                                           double slTotalTimeEnd,
                                                           int slQualityStart,
                                                           int slQualityEnd,
                                                           int exerciseStart,
                                                           int exerciseEnd,
                                                           int coffeeStart,
                                                           int coffeeEnd,
                                                           int screenTimeStart,
                                                           int screenTimeEnd,
                                                           double workTimeStart,
                                                           double workTimeEnd,
                                                           int productivityStart,
                                                           int productivityEnd,
                                                           int moodStart,
                                                           int moodEnd,
                                                           int stressStart,
                                                           int stressEnd)
        {
            using (var context = new AppDbContext(_login, _password))
            {
                bool allParametersEmpty = respondentId == 0 &&
                            slStartTimeStart == 0 && slStartTimeEnd == 0 &&
                            slEndTimeStart == 0 && slEndTimeEnd == 0 &&
                            slTotalTimeStart == 0 && slTotalTimeEnd == 0 &&
                            slQualityStart == 0 && slQualityEnd == 0 &&
                            exerciseStart == 0 && exerciseEnd == 0 &&
                            coffeeStart == 0 && coffeeEnd == 0 &&
                            screenTimeStart == 0 && screenTimeEnd == 0 &&
                            workTimeStart == 0 && workTimeEnd == 0 &&
                            productivityStart == 0 && productivityEnd == 0 &&
                            moodStart == 0 && moodEnd == 0 &&
                            stressStart == 0 && stressEnd == 0;
                if (!allParametersEmpty)
                {
                    var query = context.SleepData.AsQueryable(); //AsQueryable() — преобразует DbSet<SleepData> в IQueryable<SleepData>.
                                                                 //Это позволяет строить запросы с использованием LINQ, которые будут
                                                                 //преобразованы в SQL и выполнены на стороне базы данных.

                    if (respondentId > 0)
                        query = query.Where(x => x.PersonId == respondentId);

                    if (slStartTimeStart > 0 && slStartTimeEnd > 0)
                        query = query.Where(x => x.SleepStartTime >= slStartTimeStart && x.SleepStartTime <= slStartTimeEnd);
                    else if (slStartTimeStart > 0)
                        query = query.Where(x => x.SleepStartTime >= slStartTimeStart);
                    else if (slStartTimeEnd > 0)
                        query = query.Where(x => x.SleepStartTime <= slStartTimeEnd);

                    if (slEndTimeStart > 0 && slEndTimeEnd > 0)
                        query = query.Where(x => x.SleepEndTime >= slEndTimeStart && x.SleepEndTime <= slEndTimeEnd);
                    else if (slEndTimeStart > 0)
                        query = query.Where(x => x.SleepEndTime >= slEndTimeStart);
                    else if (slEndTimeEnd > 0)
                        query = query.Where(x => x.SleepEndTime <= slEndTimeEnd);

                    if (slTotalTimeStart > 0 && slTotalTimeEnd > 0)
                        query = query.Where(x => x.TotalSleepHours >= slTotalTimeStart && x.TotalSleepHours <= slTotalTimeEnd);
                    else if (slTotalTimeStart > 0)
                        query = query.Where(x => x.TotalSleepHours >= slTotalTimeStart);
                    else if (slTotalTimeEnd > 0)
                        query = query.Where(x => x.TotalSleepHours <= slTotalTimeEnd);

                    if (slQualityStart > 0 && slQualityEnd > 0)
                        query = query.Where(x => x.SleepQuality >= slQualityStart && x.SleepQuality <= slQualityEnd);
                    else if (slQualityStart > 0)
                        query = query.Where(x => x.SleepQuality >= slQualityStart);
                    else if (slQualityEnd > 0)
                        query = query.Where(x => x.SleepQuality <= slQualityEnd);

                    if (exerciseStart > 0 && exerciseEnd > 0)
                        query = query.Where(x => x.ExerciseMinutes >= exerciseStart && x.ExerciseMinutes <= exerciseEnd);
                    else if (exerciseStart > 0)
                        query = query.Where(x => x.ExerciseMinutes >= exerciseStart);
                    else if (exerciseEnd > 0)
                        query = query.Where(x => x.ExerciseMinutes <= exerciseEnd);

                    if (coffeeStart > 0 && coffeeEnd > 0)
                        query = query.Where(x => x.CaffeineIntakeMg >= coffeeStart && x.CaffeineIntakeMg <= coffeeEnd);
                    else if (coffeeStart > 0)
                        query = query.Where(x => x.CaffeineIntakeMg >= coffeeStart);
                    else if (coffeeEnd > 0)
                        query = query.Where(x => x.CaffeineIntakeMg <= coffeeEnd);

                    if (screenTimeStart > 0 && screenTimeEnd > 0)
                        query = query.Where(x => x.ScreenTime >= screenTimeStart && x.ScreenTime <= screenTimeEnd);
                    else if (screenTimeStart > 0)
                        query = query.Where(x => x.ScreenTime >= screenTimeStart);
                    else if (screenTimeEnd > 0)
                        query = query.Where(x => x.ScreenTime <= screenTimeEnd);

                    if (workTimeStart > 0 && workTimeEnd > 0)
                        query = query.Where(x => x.WorkHours >= workTimeStart && x.WorkHours <= workTimeEnd);
                    else if (workTimeStart > 0)
                        query = query.Where(x => x.WorkHours >= workTimeStart);
                    else if (workTimeEnd > 0)
                        query = query.Where(x => x.WorkHours <= workTimeEnd);

                    if (productivityStart > 0 && productivityEnd > 0)
                        query = query.Where(x => x.ProductivityScore >= productivityStart && x.ProductivityScore <= productivityEnd);
                    else if (productivityStart > 0)
                        query = query.Where(x => x.ProductivityScore >= productivityStart);
                    else if (productivityEnd > 0)
                        query = query.Where(x => x.ProductivityScore <= productivityEnd);

                    if (moodStart > 0 && moodEnd > 0)
                        query = query.Where(x => x.MoodScore >= moodStart && x.MoodScore <= moodEnd);
                    else if (moodStart > 0)
                        query = query.Where(x => x.MoodScore >= moodStart);
                    else if (moodEnd > 0)
                        query = query.Where(x => x.MoodScore <= moodEnd);

                    if (stressStart > 0 && stressEnd > 0)
                        query = query.Where(x => x.StressLevel >= stressStart && x.StressLevel <= stressEnd);
                    else if (stressStart > 0)
                        query = query.Where(x => x.StressLevel >= stressStart);
                    else if (stressEnd > 0)
                        query = query.Where(x => x.StressLevel <= stressEnd);

                    // Выполняем запрос и возвращаем результат
                    return query.ToList(); //метод, который выполняет запрос и возвращает результаты в виде списка(List<SleepData>).
                } else
                {
                    var sleepData = context.SleepData.OrderBy(r => r.Id).ToList();
                    return sleepData;
                }
            }
        }
    }

}
