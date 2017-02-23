using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace ConsoleVerification
{
    using ForgetTheMilk.Controllers;
    using NUnit.Framework;

    public class CreateTaskTest :AssertionHelper
    {
        [Test]
        public void DescriptionAndNoDueDate()
        {
            var input = "Pickup the groceries";

            var task = new Task(input, default(DateTime));

            //Två olika sätt
            Expect(task.Description, Is.EqualTo(input));
            Assert.AreEqual(null, task.DueDate);
        }

        [Test]
        [TestCase("Pickup the groceries feb 5 as of 2017-02-28")]
        [TestCase("Pickup the groceries jan 5 as of 2017-02-28")]
        public static void MayDueDateDoesWrapYear(string input)
        {
            //Obra test funkar bara om testmånad = innevarande månad
            var today = new DateTime(2017, 2, 23);

            var task = new Task(input, today);

            Expect(task.DueDate.Value.Year, Is.EqualTo(2018));
        }

        [Test]
        public void MayDueDateDoesNotWrapYear()
        {
            var input = "Pickup the groceries maj 5 as of 2015-05-04";
            var today = new DateTime(2015, 5, 4);

            var task = new Task(input, today);

            Expect(task.DueDate, Is.EqualTo(new DateTime(2015, 5, 5)));
        }

        [Test]
        [TestCase("Groceries jan 5", 1)]
        [TestCase("Groceries feb 5", 2)]
        [TestCase("Groceries mar 5", 3)]
        [TestCase("Groceries apr 5", 4)]
        [TestCase("Groceries maj 5", 5)]
        [TestCase("Groceries jun 5", 6)]
        [TestCase("Groceries jul 5", 7)]
        [TestCase("Groceries aug 5", 8)]
        [TestCase("Groceries sep 5", 9)]
        [TestCase("Groceries okt 5", 10)]
        [TestCase("Groceries nov 5", 11)]
        [TestCase("Groceries dec 5", 12)]
        public static void DueDate(string input, int expectedMonth)
        {
            var task = new Task(input, default(DateTime));

            Expect(task.DueDate, Is.Not.Null);
            Expect(task.DueDate.Value.Month, Is.EqualTo(expectedMonth));
        }

        [Test]
        public void TwoDigitDay_ParseBothDigits()
        {
            var input = "Groceries apr 10";

            var task = new Task(input, default(DateTime));

            Expect(task.DueDate.Value.Day, Is.EqualTo(10));
        }

        [Test]
        public void AddFeb29TaskInMarchOfYearBeforeLeapYear_ParsesDueDate()
        {
            var input = "Groceries feb 29";
            var today = new DateTime(2015, 3, 1);

            var task = new Task(input, today);

            Expect(task.DueDate, Is.EqualTo(new DateTime(2016, 2, 29)));
        }

        [Test]
        public void DayIsPastTheLastOfTheMonth_DoesNotParseDueDate()
        {
            var input = "Groceries apr 44";

            var task = new Task(input, default(DateTime));

            Expect(task.DueDate, Is.Null);
        }
    }
}
