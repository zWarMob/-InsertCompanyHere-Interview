using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUsers
{
    using System.Threading;

    using LogTest;

    class Program
    {
        static void Main(string[] args)
        {
            ILogger  logger = new FileLogger();

            for (int i = 0; i < 15; i++)
            {
                logger.WriteLog("Number with Flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger.Stop_With_Flush();

            ILogger logger2 = new FileLogger();
            AppDomain.CurrentDomain.ProcessExit += logger2.Stop_With_Flush;

            for (int i = 50; i > 0; i--)
            {
                logger2.WriteLog("Number with No flush: " + i.ToString());
                Thread.Sleep(20);
            }

            logger2.Stop_Without_Flush();

            Console.ReadLine();
        }
    }
}
