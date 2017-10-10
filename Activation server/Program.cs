using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activation_server {
    class Program {
        static void Main(string[] args) {
//            TODO run program
            var db = new DatabaseHandler();
//            Console.WriteLine(db.ActivateProduct("1234567890123456789012345", "1234567890123456789012341"));
            Console.WriteLine(db.DeactivateProduct("1234567890123456789012345"));
        }
    }
}