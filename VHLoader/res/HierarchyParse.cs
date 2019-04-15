using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class is finding parent of a node
/// </summary>


namespace ConsoleApp9
{
    public class HierarchyParse
    {
        static public int Hcounter = 0; 
        string[] lines; 
        public HierarchyParse(ref string[] Lines)
        {
            lines = Lines;
        }

        public int[] FindParent(ref string[] lines )
        {
            int[] parents = new int[100];
            int i = 0;
            bool opener = false;
            foreach (string x in lines)
            {
                if (x.Contains(Marks.marks[6]))
                {   
                    if(opener == false)
                    Hcounter++;
                    if (checkToStop(x, opener))
                    {
                        parents[i] = Hcounter;
                        i++;
                    }
                    opener = true;
                }
                else if (x.Contains(Marks.marks[7]))
                {
                    Hcounter--;
                    opener = false;
                    if (checkToStop(x, opener))
                    {
                        parents[i] = Hcounter;
                        i++;
                    }
                    opener = true;
                }
                else continue;
            }
            return parents;

        }

        private bool checkToStop(string line, bool foundOpener = false)
        {
            if (line.Contains(Marks.marks[6]) && foundOpener)
                return true;
            else if (line.Contains(Marks.marks[7]))
                return false;

            return true;
        }

        public int[] start()
        {
            return FindParent(ref lines);
        }
    }
}
