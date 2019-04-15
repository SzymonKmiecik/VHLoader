using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace ConsoleApp9
{
    class Program
    {

        static void Main(string[] args)
        {
            StaticGetter.startParsing(StaticGetter.file);
            while (true)
            {
                StaticGetter.inputInterface(StaticGetter.file);
            }            
        }
    }  
}
    

