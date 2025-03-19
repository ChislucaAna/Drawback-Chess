using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace DrawbackChess.Classes.DatabaseClasses
{
    public class Test
    {
        [PrimaryKey,AutoIncrement] public int TestId { get; set; }
        public string Message { get; set; }

        public Test() //for sqlLite conventions
        {

        }
        public Test(string message)
        {
            Message = message;
        }   
    }
}
