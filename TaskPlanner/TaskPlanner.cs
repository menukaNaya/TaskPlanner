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
        //For storing holidays and recurring holidays...
        private List<String> holidays = new List<String>();
        private List<RecurringHoiday> recHloidays = new List<RecurringHoiday>();
        

        //Method to add start time and stop time...
        public void SetWorkDayStartAndStop(TimeSpan startTime, TimeSpan stopTime)
        {
            workDayStartTime = startTime;
            workDayStopTime = stopTime;
        }

        //Method for set holidays...
        public void SetHoliday(DateTime holiday)
        {

            holidays.Add(holiday.Date.ToString("d"));
        }

        //Method for set recurring Holidays...
        public void SetRecurringHoliday(DateTime holiday)
        {
            recHloidays.Add(new RecurringHoiday(holiday.Month,holiday.Day));
        }

        /*This method for finding whether it's a rec holiday or not..
         * It returns a boolean,
         * If recHoliday it returns true... */
        public bool IsARecHoliday(DateTime day)
        {
            bool isAHoliday = false;
            foreach (RecurringHoiday recHoliday in recHloidays)
            {
                isAHoliday = recHoliday.IsEqual(new RecurringHoiday(day.Month, day.Day));
            }
            return isAHoliday;
        }

        /*This method finds the input day is a holiday or not...
         * It cathches all holidays, recurring holidays and weekends...
         * If it's a holiday it's returns the correct day for startover...
        */
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

        //This is a structure use to store recurring holidays...
        private struct RecurringHoiday
        {
            //For keeping given recurring holiday month and the date...
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

            //This method is using for catching the recurring holidays...
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

        //Method to get task finshing date...
        public DateTime GetTaskFinishingDate(DateTime start, double days)
        {
            //This is using for finding whether workdays are minus or not...
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
            if ((start.Hour + (start.Minute / 60.0)) < (int)(workDayStartTime).TotalHours)
            {
                if (hrs > 0)
                {
                    start = start.AddHours((int)(workDayStartTime).TotalHours - start.Hour).AddMinutes(-start.Minute);
                }

                if (hrs < 0)
                {
                    start = start.AddHours(- (start.Hour + (24- (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);
                }
            }

            //If the start time is after working day stop time...
            if ((start.Hour + (start.Minute / 60.0)) > (int)(workDayStopTime).TotalHours)
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
            int min = start.Minute + (int)((totalNumOfHoursForTheTask - Math.Truncate(totalNumOfHoursForTheTask)) * 60);

            //Main logic for finding the finishing date...
            if (hrs >= ((int)(workDayStopTime).TotalHours - start.Hour) || -hrs >= (start.Hour - (int)(workDayStartTime).TotalHours))
            {

                if (hrs >= 0)
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
                        start = IsAHoliday(start, hrsAreMinus);

                        for (int i = 0; i < hrs / 8; i++)
                        {
                            start = start.AddDays(1);
                            start = IsAHoliday(start, hrsAreMinus);

                        }

                        if(hrs%8 == 0)
                        {
                            start = start.AddHours(8);
                        }

                        start = start.AddHours(hrs % 8).AddMinutes(min);
                    }
                    else
                    {

                        start = start.AddHours(-(start.Hour + (24 - (int)(workDayStopTime).TotalHours))).AddMinutes(-start.Minute);
                        start = IsAHoliday(start, hrsAreMinus);

                        for (int i = 0; i < -hrs / 8; i++)
                        {
                            start = start.AddDays(-1);
                            start = IsAHoliday(start, hrsAreMinus);
                        }

                        if (hrs % 8 == 0)
                        {
                            start = start.AddHours(-8);
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

        static void Main(string[] args)
        {
        }

    }

}
