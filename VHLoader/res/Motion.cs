using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// storing raw motion data for nodes. These data will be later compute for transformation matrices
/// </summary>

namespace ConsoleApp9
{
    public class Motion
    {  
        public double[,] values = null;
        public double frameTime ;
        public int frameCount = 0;
        public Motion(double[,] Values, int FrameCount = 0, double FrameTime = 0.0) //do poprawy (tylko indeks body[0] ma zawierac info o frame)
        {
            values = Values;
            frameCount = FrameCount;
            frameTime = FrameTime;
        }

        public void setValues( double[,] Values)
        {
            values = Values;
        }
        public void getValues()
        {
            Console.WriteLine(frameCount);
            Console.WriteLine(frameTime);
            for (int i = 0; i < frameCount; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Console.Write(values[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
