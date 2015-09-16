using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssFeedCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            RssFeedProcessor processor = new RssFeedProcessor();
            //processor.GetFeedsAsync();
            //processor.UploadFeedsToDocumentDB();
            processor.TestQueryDocument();
            Console.ReadLine();
        }
    }
}
