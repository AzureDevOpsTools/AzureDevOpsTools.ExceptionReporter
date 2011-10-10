using System.Linq;

namespace QueueTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueTool = new QueueUtil();

            if (args.Count() > 0)
            {
                if (args[0].Equals("-r"))
                {
                    queueTool.RemoveLast();
                }
                if (args[0].Equals("-p"))
                {
                    queueTool.Purge();
                }
                if (args[0].Equals("-c"))
                {
                    queueTool.CreateMessage();
                }
                if (args[0].Equals("-w"))
                {
                    queueTool.ReadWebService();
                }
            }
            else
            {
                queueTool.CreateMessage();
            }
        }
    }
}
