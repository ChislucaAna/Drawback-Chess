using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes.DatabaseClasses
{
    public class Test
    {
        public int TestId { get; set; }
        public string Message { get; set; }

        public Test(int testId, string message)
        {
            TestId = testId;
            Message = message;
        }   
    }
}
