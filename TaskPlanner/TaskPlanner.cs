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
                        for (int i = 0; i <= hrs / 8; i++)
                        {
                            start = IsAHoliday(start, hrsAreMinus);
                            start.AddDays(1);
                        }
                        
                        start = start.AddHours(hrs % 8).AddMinutes(min);
                    }
                    else
                    {
                        start = start.AddHours(-(start.Hour + (24 - (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);
                        for (int i = 0; i <= hrs / 8; i++)
                        {
                            start = IsAHoliday(start, hrsAreMinus);
                            start.AddDays(1);
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

        public DateTime IsAHoliday(DateTime start, bool hrsAreMinus)
        {
            while ((start.DayOfWeek == DayOfWeek.Saturday) || (start.DayOfWeek == DayOfWeek.Sunday) || (holidays.Contains(start.Date.ToString("d"))))
            {
                if (hrsAreMinus)
                {
                    start = start.AddHours(-(start.Hour + (24 - (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);

                }else
                {
                    start = start.AddHours(23 - start.Hour).AddMinutes(60 - start.Minute).Add(workDayStartTime);
                }
            }

            return start;
        }

        private struct RecurringHoiday
        {
            private int month;
            private int date;

            public  RecurringHoiday(int month, int date)
            {
                this.month = month;
                this.date = date;
            }


        }

        static void Main(string[] args)
        {
            DateTime start = new DateTime(2017, 12, 7, 15, 10, 0);
            TimeSpan s = new TimeSpan (8,0,0);
            TimeSpan e = new TimeSpan(16, 0, 0);

            //TaskPlanner taskPlanner = new TaskPlanner();

            //taskPlanner.SetHoliday(new DateTime(2017, 12, 8, 0, 0, 0));
            //taskPlanner.SetHoliday(new DateTime(2004, 5, 24, 0, 0, 0));
            //taskPlanner.SetHoliday(new DateTime(2004, 5, 27, 0, 0, 0));

            //taskPlanner.SetWorkDayStartAndStop(s,e);
            //taskPlanner.GetTaskFinishingDate(start, 0.25);

            /*List<String> holidays = new List<String>();
            DateTime hol = new DateTime(2017, 12, 8, 0, 0, 0);
            holidays.Add(hol.Date.ToString("d"));

            while (holidays.Contains(start.Date.ToString("d")))
            {             
                   // start = start.AddHours(-(start.Hour + (24 - (int)(e).TotalHours))).AddMinutes(-start.Minute);
                    start = start.AddHours(23 - start.Hour).AddMinutes(60 - start.Minute).Add(s);
            }*/
            int h = start.Month;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(start.Month.ToString() + start.Day.ToString());
            Console.WriteLine(stringBuilder);
        }
    }
}
