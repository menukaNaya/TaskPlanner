using System;
using System.Collections.Generic;
using System.Text;

namespace TaskPlanner
{
    public class TaskPlanner
    {
        //For storing timespans...
        private TimeSpan workDayStartTime;
        private TimeSpan workDayStopTime;
        private List<String> holidays = new List<String>();
        private List<RecurringHoiday> recHloidays = new List<RecurringHoiday>();
        

        //Method to add start time and stop time...
        public void SetWorkDayStartAndStop(TimeSpan startTime, TimeSpan stopTime)
        {
            workDayStartTime = startTime;
            workDayStopTime = stopTime;
        }

        //Method to get task finshing date...
        public DateTime GetTaskFinishingDate(DateTime start, double days)
        {
            bool hrsAreMinus;
            //Taking the number of hours for a working day...
            double hoursPerWorkDay =(double)(workDayStopTime - workDayStartTime).TotalHours;

            //Finding the number of hours needed to complete the task...
            double totalNumOfHoursForTheTask = days * hoursPerWorkDay;

            int hrs = (int)totalNumOfHoursForTheTask;

            if(hrs >= 0)
            {
                hrsAreMinus = false;
            }
            else
            {
                hrsAreMinus = true;
            }

            //Checking whether start day is a holiday or not...
            start = IsAHoliday(start, hrsAreMinus);

            //If the start time is before working day start time...
            if (start.Hour <= (int)(workDayStartTime).TotalHours)
            {
                if(hrs > 0)
                {
                    start = start.AddHours((int)(workDayStartTime).TotalHours - start.Hour).AddMinutes(-start.Minute);
                }

                if (hrs < 0)
                {
                    start = start.AddHours(- (start.Hour + (24- (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);
                }
            }

            //If the start time is after working day stop time...
            if (start.Hour >= (int)(workDayStopTime).TotalHours)
            {
                if(hrs > 0)
                {
                    start = start.AddHours(23 - start.Hour).AddMinutes(60 - start.Minute).Add(workDayStartTime);
                }
                if (hrs < 0)
                {
                    start = start.AddHours(-(start.Hour - (int)(workDayStopTime).TotalHours)).AddMinutes(- start.Minute);
                }
            }

           /* if((start.Hour < (int)(workDayStopTime).TotalHours) && (start.Hour > (int)(workDayStartTime).TotalHours))
            {
                if (hrs > 0)
                {
                    start = start.AddHours(23 - start.Hour).AddMinutes(60 - start.Minute).Add(workDayStartTime);
                }

                if (hrs < 0)
                {
                    start = start.AddHours(-(start.Hour + (24 - (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);
                }
            }*/


            //Finding the number of hours and minutes to add to the starting date...
            int min = start.Minute + (int)((totalNumOfHoursForTheTask - Math.Truncate(totalNumOfHoursForTheTask)) * 100);

            //if the number of miniutes are larger than 60, then adding it to the hrs...
            if (min > 60)
            {
                hrs += 1;
                min -= 60;
            }

            //Main logic
            if (hrs >= ((int)(workDayStopTime).TotalHours - start.Hour) || -hrs >= (start.Hour - (int)(workDayStartTime).TotalHours))
            {
                
                if(hrs >= 0)
                {
                    hrs -= ((int)(workDayStopTime).TotalHours - start.Hour);

                }
                else
                {
                    hrs += (start.Hour-(int)(workDayStartTime).TotalHours);
                    
                }
                

                if (hrs == 0 && min == 0)
                {
                    if (hrsAreMinus)
                    {
                        start = start.AddHours(((int)(workDayStartTime).TotalHours) - start.Hour);
                    }else
                    {
                        start = start.AddHours(((int)(workDayStopTime).TotalHours - start.Hour));
                    }

                    Console.WriteLine(start);
                    return start;    
                }else    
                {
                    if (hrs >= 0)
                    {
                        start = start.AddHours(23 - start.Hour).AddMinutes(60 - start.Minute).Add(workDayStartTime);
                        //Console.WriteLine(start);
                        start = IsAHoliday(start, hrsAreMinus);
                        //Console.WriteLine(start);

                        for (int i = 0; i < hrs / 8; i++)
                        {
                            //Console.WriteLine(start);
                            start = start.AddDays(1);
                            //if()
                            //{
                                start = IsAHoliday(start, hrsAreMinus);
                            //}
                            //Console.WriteLine(start);

                        }
                        start = start.AddHours(hrs % 8).AddMinutes(min);
                    }
                    else
                    {
                        start = start.AddHours(-(start.Hour + (24 - (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);
                        start = IsAHoliday(start, hrsAreMinus);
                        for (int i = 0; i < hrs / 8; i++)
                        {
                            start = start.AddDays(1);
                            start = IsAHoliday(start, hrsAreMinus);

                        }
                        start = start.AddHours(hrs % 8).AddMinutes(min);
                        
                    }
                        
                    Console.WriteLine(start);   
                    return start;    
                }
                    
            }else
            {
                start = IsAHoliday(start, hrsAreMinus);
                start = start.AddHours(hrs % 8).AddMinutes(min - start.Minute);
                Console.WriteLine(start);
                return start;
            }

        }

        public void SetHoliday(DateTime holiday)
        {

            holidays.Add(holiday.Date.ToString("d"));
        }

        public void SetRecurringHoliday(DateTime holiday)
        {
            recHloidays.Add(new RecurringHoiday(holiday.Month,holiday.Day));
        }

        public DateTime IsAHoliday(DateTime day, bool hrsAreMinus)
        {
            while ((day.DayOfWeek == DayOfWeek.Saturday) || (day.DayOfWeek == DayOfWeek.Sunday) || (holidays.Contains(day.Date.ToString("d"))) || IsARecHoliday(day))
            {
                if (hrsAreMinus)
                {
                    day = day.AddHours(-(day.Hour + (24 - (int)(workDayStopTime).TotalHours))).AddMinutes(-day.Minute);

                }else
                {
                    day = day.AddHours(23 - day.Hour).AddMinutes(60 - day.Minute).Add(workDayStartTime);
                }
            }

            return day;
        }

        public bool IsARecHoliday(DateTime day)
        {
            bool isAHoliday = false;
            foreach (RecurringHoiday recHoliday in recHloidays)
            {
                isAHoliday = recHoliday.IsEqual(new RecurringHoiday(day.Month, day.Day));
            }
            return isAHoliday;
        }
        
        private struct RecurringHoiday
        {
            private int _month;
            private int _date;

            public  RecurringHoiday(int month, int date)
            {
                this._month = month;
                this._date = date;
            }
            public int Month
            {
                get { return this._month; }
            }

            public int Date
            {
                get { return this._date; }
            }

            public bool IsEqual(RecurringHoiday givenDay)
            {
                if((this.Month == givenDay.Month) && (this.Date == givenDay.Date))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            
            }
        }

        static void Main(string[] args)
        {
            DateTime start = new DateTime(2004, 5, 25, 8, 0, 0);
            TimeSpan s = new TimeSpan (8,0,0);
            TimeSpan e = new TimeSpan(16, 0, 0);

            TaskPlanner taskPlanner = new TaskPlanner();

            taskPlanner.SetRecurringHoliday(new DateTime(2004, 5, 17, 0, 0, 0));
            //taskPlanner.SetRecurringHoliday(new DateTime(2004, 5, 24, 0, 0, 0));
            taskPlanner.SetHoliday(new DateTime(2004, 5, 27, 0, 0, 0));

            taskPlanner.SetWorkDayStartAndStop(s,e);
            taskPlanner.GetTaskFinishingDate(start, 44.723656);

        }
    }
}
