using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceTests
{
    public class TestDataGenerator
    {
        public static object[][] GetDepositChequeData = new object[][]
        {
            new object[] {1000.00, new DateTime(2019, 04, 17, 13, 00, 00), 12345, new DateTime(2019, 04, 18, 13, 00, 00)}
        };

        public static object[][] GetDepositChequeUnblockData = new object[][]
        {
            new object[]
            {
                1000.00, 500.00, 2000.00, 1000.00, new DateTime(2019, 04, 17, 13, 00, 00), 12345,
                new DateTime(2019, 04, 18, 13, 00, 00)
            }
        };

        public static object[][] GetDateTime = new object[][]
        {
            new object[] {new DateTime(2019, 04, 15, 08, 00, 00), new DateTime(2019, 04, 16, 08, 00, 00)},
            new object[] {new DateTime(2019, 04, 15, 10, 00, 00), new DateTime(2019, 04, 16, 10, 00, 00)},
            new object[] {new DateTime(2019, 04, 15, 18, 00, 00), new DateTime(2019, 04, 17, 18, 00, 00)},
            new object[] {new DateTime(2019, 04, 18, 18, 00, 00), new DateTime(2019, 04, 22, 18, 00, 00)},
            new object[] {new DateTime(2019, 04, 19, 10, 00, 00), new DateTime(2019, 04, 22, 10, 00, 00)},
            new object[] {new DateTime(2019, 04, 19, 18, 00, 00), new DateTime(2019, 04, 23, 18, 00, 00)},
            new object[] {new DateTime(2019, 04, 20, 18, 00, 00), new DateTime(2019, 04, 23, 18, 00, 00)},
            new object[] {new DateTime(2019, 04, 21, 18, 00, 00), new DateTime(2019, 04, 23, 18, 00, 00)},
        };
    }

}

