using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Globalization;
using OpenTK;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;



namespace ConsoleApp9
{    //klasa znaczników parsera

    public class Marks
    {
        //lista słów kluczowych, używanych do ładowania pliku w odpowiednie typy zmiennych
        static public    string[] marks = { "ROOT", "JOINT" , "OFFSET",
                           "CHANNELS", "HIERARCHY", "MOTION",
                           "{", "}", "End Site" };
    }

    [StructLayout(LayoutKind.Auto)]
    public class Parser
    {
        //przechowuje hierarchię i dane ruchu(sam ruch w indeksie [0], od [1] hierarchia rozpoczynająca się od roota)
        private bool _norm;
        public List<node> nodes = null;
        //lista słów kluczowych, używanych do ładowania pliku w odpowiednie typy zmiennych
        string[] marks = { "ROOT", "JOINT" , "OFFSET",
                           "CHANNELS", "HIERARCHY", "MOTION",
                           "{", "}", "End Site" };
        ///public List<node> parseResult( string fileName)
        
        //ładowanie surowego tekstu, oraz wywołanie dalszych funkcji
        public bool parseResult(string fileName, bool norm)                                                    
        {
            string data;
            _norm = norm;
            try
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
                {
                    if (sr == null)
                        return false;
                    data = sr.ReadToEnd();
                }
            
            string[] lines = data.Split(new[] { "\n" } , StringSplitOptions.None);
                for (int i =0; i < lines.Length -1; i++)
                {
                    lines[i] = Regex.Replace(lines[i], @"\r", "");
                }
            parse(ref lines);
            return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        private void parse(ref string[] lines)
        { 
            int channelCount = 0;                                               //zliczamy liczbę kanałów, by wiedzieć jaki indeks motion należy do                    
            //int[] parents;                                       //do obliczania rodzica węzła
            int[] counter = { 0, 0, 0, 0 };                                     //validacja czy cokolwiek zostało odczytane
            for (int i = 0; i < lines.Length; i++)
            {
                //jeżeli napotkamy roota- rozpakowujemy go
                if (lines[i].Contains(marks[0]))
                {
                //    HierarchyParse x = new HierarchyParse(ref lines);
                //    parents = x.start();
                    nodes = translateRoot(ref lines, i, ref channelCount);
                    counter[0]++;
                }
                //rozpakowujemy jointy
                else if (lines[i].Contains(marks[1]))
                {
                    nodes.Add(translateJoint(ref lines, i, ref channelCount));
                    counter[1]++;
                //    currID++;
                }
                //sprawdzamy czy są dwa nagłówki MODE pliku bsv
                else if (lines[i].Contains(marks[4]) || lines[i].Contains(marks[5]))
                {

                    if (!lines[i].Contains(marks[4]))
                    {
                        // parents = new int[nodes.Count];
                        compHierFromWhite( countWhite(ref lines));
                    }

                    counter[2]++;
                }
                //liczymy nawiasy do dalszego ustalania hierarchii
                else if (lines[i].Contains(marks[6]))
                {  
                
                }
                else if (lines[i].Contains(marks[7]))
                {   
                
                }
                //znak końca gałęzi
                else if (lines[i].Contains(marks[8]))
                {    
                    counter[3]++;
                }
                //jeżeli wszystkie inne marki nie występują, to napewno jest to część pliku odpowiedzialna za ruch
                else if (!(lines[i].Contains(marks[2])) && !(lines[i].Contains(marks[3])))
                {
                    parseMotion(ref lines, i);
                    break;
                }
            }
            Console.WriteLine("Number of ROOTS: " + counter[0].ToString() + 
                    "\nNumber of JOINTS: " + counter[1].ToString() + 
                    "\nNumber of MODES: " + counter[2].ToString() + 
                    "\nNumber of Brackets and ends: " + 
                    counter[3].ToString());
        }

        //ładowanie węzła roota
        private List<node> translateRoot( ref string[] line, int index, ref int channelCount) 
        {
            List<node> Body = new List<node>();
            Body.Add(new node());
            string[] splitName = line[index].Split(' ');
            ///node root = new node(splitName[1], translateOffset(ref line, index + 2) , translateChannel(ref line, index + 3));
            Body.Add(new node(splitName[1], translateOffset(ref line, index + 2), translateChannel(ref line, index + 3, ref channelCount)));
            Body[1].isRoot = true;
            return Body;
        }

        //ładowanie offsetu z danych węzła
        private double[] translateOffset(ref string[] line, int index)
        {
            double[] offsets = new double[3];
            string temp = line[index].Substring(line[index].IndexOf(marks[2]));
            string[] tempSplit = temp.Split(' ');
            offsets[0] = double.Parse(tempSplit[1], CultureInfo.InvariantCulture);
            offsets[1] = double.Parse(tempSplit[2], CultureInfo.InvariantCulture);
            offsets[2] = double.Parse(tempSplit[3], CultureInfo.InvariantCulture);
            return offsets;
        }

        //ładowanie ilości kanałów, oraz ich typów w kolejności
        private string[] translateChannel(ref string[] line, int index, ref int channelCount)
        {
            string temp = line[index].Substring(line[index].IndexOf(marks[3]));
            string[] tempSplit = temp.Split(' ');
            string[] channels = new string[tempSplit.Length - 2];
            for (int i =0;  i < tempSplit.Length -2; i++)
            {
                channels[i] = tempSplit[i + 2];
            }
            channelCount += Int32.Parse(tempSplit[1]);
            return channels;
        }

        //ładowanie danych ruchu (powinno się wywołać tylko raz dla całego pliku)
        //mozliwe ulepszenie (caly ruch dla body[0], wezły z hierarchi mogą obliczać własną referencję do odpowiedniej cześci całości, zamiast fizycznie dzielić i zapisywać nowe dane)
        private void parseMotion(ref string[] Lines, int index)
        {
            //string temp = Lines[index + 1].Substring(Lines[index + 1].IndexOf(marks[3]));
            string[] tempSplit = Lines[index ].Split('\t');
            int frameCount = Int32.Parse(tempSplit[1]);
            tempSplit = Lines[index + 1].Split('\t');
            double frameTime = double.Parse(tempSplit[1], CultureInfo.InvariantCulture);
            StaticGetter._fps = 1.0 / frameTime;
            for (int k = 1; k < nodes.Count; k++)
            {
                double[,] values = new double[frameCount, nodes[k].channels.Length];
                for (int i = 2; index + i < Lines.Length - 1; i++)
                {
                    tempSplit = Lines[index + i].Split(' ');
                    int tmpi = 0; 
                    if (nodes[k].getChannelIndex() == 0 && _norm == true)
                    {
                        nodes[k].offset[0] = 0.0;
                        nodes[k].offset[1] = 0.0;
                        nodes[k].offset[2] = 0.0;
                        values[i - 2, 0] = 0.0;
                        values[i - 2, 1] = 0.0;
                        values[i - 2, 2] = 0.0;
                        tmpi += 3;
                    }
                    for (int j = tmpi; j < nodes[k].channels.Length; j++)
                    {

                        values[i - 2, j] = double.Parse(tempSplit[nodes[k].getChannelIndex() + j], CultureInfo.InvariantCulture);
                    } 
                }
                nodes[k].setMotion(values, frameCount, frameTime);
            }
            
        }

        //ładujemy jointy
        private node  translateJoint(ref string[] Lines, int index, ref int ChannelCount)
        {
            node joint = null;
            string[] splitName = Lines[index].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            //splitName = splitName[0].Split(' ');
            joint = new node(splitName[1], translateOffset(ref Lines, index + 2), translateChannel(ref Lines, index + 3, ref ChannelCount), ChannelCount - nodes[nodes.Count - 1].channels.Length);
            return joint;
        }
        
        //liczymy spacje (sposób na określenie hierarchii - z 10 różnych podejść ten jest najprostszy i daje 100% poprawny wynik)
        private int[] countWhite (ref string[] Lines)
        {
            int[] spaces = new int[nodes.Count+2];
            spaces[0] = 0;
            spaces[nodes.Count + 1] = 10000;
            int j = 1;
            for (int i = 0; i < Lines.Length; i++)
            {
                string source = Lines[i];
                int count = 0;
                if (Lines[i].Contains(marks[5]))
                    break;
                if (Lines[i].Contains(marks[1]))
                    foreach (char c in source)
                    if (c == ' ') count++;
                    else if (count == 0) break; 
                    else
                    {
                        spaces[j++] = count;
                        break;
                    }
            }
            return spaces; 
        }

        //wyliczamy hierarchie z głebokości białych znaków
        private void compHierFromWhite( int[] deep)
        {
            int rootSi = deep[0];
            int rootID = 1;
            int n = 0;
        for(int i = 0; i < nodes.Count-2; i++)
            {
                if (deep[i + 1] > deep[i])
                {
                    nodes[i + 2].setParent(n + 1);
                    n++;
                }
                else if (deep[i + 1] - rootSi == 2)
                {
                    nodes[i + 2].setParent(rootID);
                    n++;
                }
                else
                {
                    rootSi = deep[i + 1] -2;
                    for (int j = i+1; j >= 0; j--)
                    {
                        if (deep[j]  == rootSi)
                        {
                            rootID = j+1;
                            nodes[i + 2].setParent(rootID);
                            n++;
                            break;
                            
                        }

                    }
                }
                    

            }
        }

        public void inputOrigi()
        {
            foreach (node x in nodes)
            {
                x.origin =(getOrigin(x));  
            }
        }
        private Vector3 getOrigin(node x)
        {
            Vector3 xyz = new Vector3((float)x.offset[0], (float)x.offset[1], (float)x.offset[2]);

            if (x.getParent() == 0)
                return xyz;
            return xyz + getOrigin(nodes[x.getParent()]);
        }
       /* int ParentFromRoot(ref int currID, ref string[] lines, int i)
        {
            int bracketCount = 0;
            int currPar;
            bool searchForToken = true;
            while (searchForToken)
            {
                if ((lines[i].Contains(marks[6]) || lines[i].Contains(marks[7])))
                {
                    while (!lines[i].Contains(marks[7]))
                    {
                        bracketCount++;
                    }
                    if (bracketCount == 0)
                    {
                        currPar = currID;
                        return currPar;
                    }
                    else if (bracketCount > 0)
                    {
                        currPar = currID + bracketCount;
                        return currPar;
                    }
                    else if (lines[i].Contains(marks[7]))
                    {
                        bracketCount--;
                        if (!(lines[i + 1].Contains(marks[7])))
                            searchForToken = false;
                    }
                }
                i++;
            }

            return currID - bracketCount;
        } */
    }

}
