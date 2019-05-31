namespace AccountBalance.Reactive.Tests.Helper
{
    using System;
    public class TestDataGenerator
    {
        public static object[][] GetDepositChequeData = new object[][]
        {
            //Monday Before 05:00PM
            new object[] {1000.00, new DateTime(2019, 04, 15, 08, 00, 00), new DateTime(2019, 04, 16, 08, 00, 00)},
            //Monday 09:00AM - 05:00PM
            new object[] {1000.00, new DateTime(2019, 04, 15, 10, 00, 00), new DateTime(2019, 04, 16, 10, 00, 00)},
            //Monday After 05:00PM
            new object[] {1000.00, new DateTime(2019, 04, 15, 18, 00, 00), new DateTime(2019, 04, 17, 18, 00, 00)},
            //Thursday After 05:00PM
            new object[] {1000.00, new DateTime(2019, 04, 18, 18, 00, 00), new DateTime(2019, 04, 22, 18, 00, 00)},
            //Friday 09:00AM - 05:00PM
            new object[] {1000.00, new DateTime(2019, 04, 19, 10, 00, 00), new DateTime(2019, 04, 22, 10, 00, 00)},
            //Friday After 05:00PM
            new object[] {1000.00, new DateTime(2019, 04, 19, 18, 00, 00), new DateTime(2019, 04, 23, 18, 00, 00)},
            //Saturday 12:00AM - 11:59PM
            new object[] {1000.00, new DateTime(2019, 04, 20, 18, 00, 00), new DateTime(2019, 04, 23, 18, 00, 00)},
            //Sunday 12:00AM - 11:59PM
            new object[] {1000.00, new DateTime(2019, 04, 21, 18, 00, 00), new DateTime(2019, 04, 23, 18, 00, 00)},
        };

        public static object[][] GetDepositChequeDataUnblockAccount = new object[][]
        {
            //Monday Before 05:00PM
            new object[] { 500.00, 100.00, new DateTime(2019, 04, 15, 10, 00, 00), new DateTime(2019, 04, 16, 10, 00, 00)},
            //Monday 09:00AM - 05:00PM
            new object[] { 500.00, 100.00, new DateTime(2019, 04, 15, 10, 00, 00), DateTime.Today },
        };
    }

}

