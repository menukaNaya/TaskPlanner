using System;
using NUnit.Framework;

namespace TaskPlanner.Tests
{
    [TestFixture]
    public class TaskPlannerTests
    {
        TaskPlanner taskPlanner; 
        [SetUp]
        public void Setup()
        {
            taskPlanner = new TaskPlanner();
            TimeSpan s = new TimeSpan(8, 0, 0);
            TimeSpan e = new TimeSpan(16, 0, 0);
            taskPlanner.SetWorkDayStartAndStop(s, e);
        }

        [Test]
        public void GetTaskFinishingDate_WhenTheTaskIsFinishingOnAWorkDayStop_ShouldReturnTheSameDayStopTime()
        {
            // Arrange
            DateTime startDate = new DateTime(2017, 12, 04, 14, 0, 0);
            DateTime actualAns = new DateTime(2017, 12, 04, 16, 0, 0);
            

            // Act
            DateTime result = taskPlanner.GetTaskFinishingDate(startDate, 0.25);

            // Assert
            Assert.AreEqual(result, actualAns);
            
        }

        [Test]
        public void GetTaskFinishingDate_WhenTheTaskIsStartingBeforeTheWorkDayBegins_ShouldReturnTheCorrectFinshingDateAndTime()
        {
            // Arrange
            DateTime startDate = new DateTime(2017, 12, 04, 4, 0, 0);
            DateTime actualAns = new DateTime(2017, 12, 04, 12, 0, 0);


            // Act
            DateTime result = taskPlanner.GetTaskFinishingDate(startDate, 0.5);

            // Assert
            Assert.AreEqual(result, actualAns);

        }

        [Test]
        public void GetTaskFinishingDate_WhenTheTaskIsStartingAfterTheWorkDayEnds_ShouldReturnTheCorrectFinshingDateAndTime()
        {
            // Arrange
            DateTime startDate = new DateTime(2017, 12, 04, 18, 0, 0);
            DateTime actualAns = new DateTime(2017, 12, 05, 12, 0, 0);


            // Act
            DateTime result = taskPlanner.GetTaskFinishingDate(startDate, 0.5);

            // Assert
            Assert.AreEqual(result, actualAns);

        }

        [Test]
        public void GetTaskFinishingDate_WhenTheTaskIsStartingDayIsAHoliday_ShouldReturnTheCorrectFinshingDateAndTime()
        {
            // Arrange
            DateTime startDate = new DateTime(2017, 12, 9, 18, 0, 0);
            DateTime actualAns = new DateTime(2017, 12, 11, 12, 0, 0);


            // Act
            DateTime result = taskPlanner.GetTaskFinishingDate(startDate, 0.5);

            // Assert
            Assert.AreEqual(result, actualAns);

        }

        [Test]
        public void GetTaskFinishingDate_WhenThereAreHolidaysInBetweenTheWorkDays_ShouldReturnTheCorrectFinshingDateAndTime()
        {
            // Arrange
            DateTime startDate = new DateTime(2017, 12, 8, 15, 7, 0);
            DateTime actualAns = new DateTime(2017, 12, 11, 11, 7, 0);


            // Act
            DateTime result = taskPlanner.GetTaskFinishingDate(startDate, 0.5);

            // Assert
            Assert.AreEqual(result, actualAns);

        }

    }
}
