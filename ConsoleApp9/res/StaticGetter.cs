using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using System.Runtime.InteropServices;

namespace ConsoleApp9
{
    //statyczna klasa do wyświetlania informacji
    static public class StaticGetter
    {
        static public int control = 0;
        static public string path = "";
        static public Parser file = new Parser();
        static string _axis;
        static public double _fps;
        static public void getSkeleton(ref Parser data)
        {
            Console.Clear();
            int counter = 1;
            int i = 0;
            int ty = 0;
            while (data.nodes.Count > counter)
            {
                if (data.nodes[counter].getParent() > data.nodes[counter - 1].getParent())
                    Console.Write("|_");

                if (data.nodes[counter].getParent() < data.nodes[counter - 1].getParent())
                {
                    ty = data.nodes[counter].getParent();
                }
                else ty = data.nodes[counter].getParent() + 1;
                while (ty >= i)
                {
                    i++;
                    Console.Write(" ");
                }

                Console.WriteLine(data.nodes[counter].getName());
                counter++;
            }
        }

        static public void inputInterface(Parser x)
        {
            var input = Console.ReadKey();
            switch (input.Key) //Switch on Key enum
            {
                case ConsoleKey.D1:
                    StaticGetter.getSkeleton(ref x);
                    break;
                case ConsoleKey.D2:
                    StaticGetter.getHierarchy(ref x);
                    break;
                case ConsoleKey.D3:
                    StaticGetter.getMotion(ref x);
                    break;
                case ConsoleKey.D4:
                    StaticGetter.ShowHierarchyParent(ref x);
                    break;
                case ConsoleKey.D5:
                    StaticGetter.ShowTransform(ref x);
                    break;
                case ConsoleKey.D6:
                    StaticGetter.ShowPosXYZ(ref x);
                    break;
                case ConsoleKey.D7:
                    StaticGetter.WriterXYZ(ref x);
                    break;
                case ConsoleKey.D8:
                    StaticGetter.StartWindow();
                    StaticGetter.control++;
                    break;
                case ConsoleKey.D9:
                    StaticGetter.Projection(ref x);
                    break;
                case ConsoleKey.D0:
                    file = new Parser();
                    StaticGetter.startParsing(file);
                    break;
            }
            
        }

       static public void startParsing(Parser x)
        {
            Console.WriteLine("ver 0.1a\n");
            Console.WriteLine("Things to do:\nMultiple times simulation play,\nStop simulation in specified frame,\nFrame Counter,\nProjection from any angle,\nSave all projected node for multiple bvh files,\nParsing next file and memory clear\n\nThanks for using. Good luck!\n\nPress key\n");

            while (true)
            {

                string temp, norm;
                Console.ReadLine();
                Console.Clear(); 
                Console.WriteLine("Input Path: ");
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
                temp = Console.ReadLine() + ".bvh";
                while (true)
                {
                    Console.WriteLine("Normalize? [y/n]");
                    while (Console.KeyAvailable)
                    Console.ReadKey(false);
                    norm = Console.ReadLine();
                    if (norm == "y" || norm == "n")
                        break;
                    else
                        Console.WriteLine("Bad input");
                }
                StaticGetter.path = temp;
                Console.WriteLine(temp);
                if (x.parseResult(temp, (norm == "y")))
                {
                    Console.Clear();
                    if (norm == "y")
                    {
                        Console.WriteLine("Normalized");
                    }
                    break;
                }
                Console.WriteLine("ERROR: BAD PATH");
            }
            //x.inputOrigi();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation\n9 2D projection\n0 new file parsing(dangerous, no memory clearing)");
        }

        static public void getMotion(ref Parser data)
        {
            Console.Clear();
            string nodeName;
            Console.WriteLine("Loaded file: " + StaticGetter.path);
            Console.WriteLine("input node name: ");
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            nodeName = Console.ReadLine();
            /// Thread.Sleep(3000);
            for (int i = 1; i < data.nodes.Count; i++)
                if (nodeName.ToLower() == data.nodes[i].getName().ToLower())
                {
                    bool found = true;
                    if (found)
                    {
                        data.nodes[i].getMotion();
                    }

                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Bone don't exist in hierarchy");
                    return;
                }
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        static public void getHierarchy(ref Parser data)
        {
            Console.Clear();
            string nodeName;
            Console.WriteLine();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("Show node mode  " + StaticGetter.path);
            Console.WriteLine("input node name: ");
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            nodeName = Console.ReadLine();
            /// Thread.Sleep(3000);
            bool found = false;
            for (int i = 1; i < data.nodes.Count; i++)
                if (nodeName.ToLower() == data.nodes[i].getName().ToLower())
                {
                    found = true;
                    data.nodes[i].getNode();
                    break;
                    
                }
                if(!found)
                {
                    Console.Clear();
                    Console.WriteLine("Bone don't exist in hierarchy");
                    return;
                }
            //Console.WriteLine(Marshal.SizeOf(typeof(Parser)));
            Console.WriteLine();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        static public void ShowHierarchyParent(ref Parser data)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("Show parent mode  " + StaticGetter.path);
            Console.WriteLine("input node name: ");
            for (int i = 0; i < data.nodes.Count; i++)
                Console.WriteLine(i + "  " + data.nodes[i].getName() + " : " + data.nodes[i].getParent());
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        static public void ShowTransform(ref Parser data)
        {
            Console.Clear();
            string nodeName;
            Console.WriteLine();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("Show transform matrices mode  " + StaticGetter.path);
            Console.WriteLine("input node name: ");
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            nodeName = Console.ReadLine();
            /// Thread.Sleep(3000);
            bool found = false;
            for (int i = 1; i < data.nodes.Count; i++)
                if (nodeName.ToLower() == data.nodes[i].getName().ToLower())
                {
                    found = true;
                    data.nodes[i].getNode();
                    break;

                }
            if (!found)
            {
                Console.Clear();
                Console.WriteLine("Bone don't exist in hierarchy");
                return;
            }
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        static public void ShowPosXYZ(ref Parser data)
        {
            Console.Clear();
            string nodeName;
            Console.WriteLine();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("Show XYZ mode  " + StaticGetter.path);
            Console.WriteLine("input node name: ");
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            nodeName = Console.ReadLine();
            /// Thread.Sleep(3000);
            bool found = false; 
            for (int i = 1; i < data.nodes.Count; i++)
                if (nodeName.ToLower() == data.nodes[i].getName().ToLower())
                {
                    found = true;
                    if (found)
                    {
                        int j = 1;
                        foreach (double s in data.nodes[i].posXYZ)
                        {
                            Console.Write(s + " ");
                            if (j % 3 == 0)
                                Console.WriteLine();
                            j++;
                        }
                        Console.WriteLine("Number of coords:" + (data.nodes[i].posXYZ.Length / 3).ToString());
                        Console.WriteLine("Number of frames:" + data.nodes[i].frameCount.ToString());
                    }

                }
            if (!found)
            {
                Console.Clear();
                Console.WriteLine("Bone don't exist in hierarchy");
                return;
            }
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        static public void WriterXYZ(ref Parser data)
        {
            Console.Clear();
            StreamWriter sw = new StreamWriter("test.csv");
            var csv = new StringBuilder();
            string nodeName;
            Console.WriteLine();
            csv.AppendLine("");
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            csv.AppendLine("Loaded file:  " + StaticGetter.path + ";;;");
            Console.WriteLine("Show XYZ mode  " + StaticGetter.path);
            Console.WriteLine("input node name: ");
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            nodeName = Console.ReadLine();
            csv.AppendLine(nodeName);
            /// Thread.Sleep(3000);
            bool found = false;
            for (int i = 1; i < data.nodes.Count; i++)
                if (nodeName.ToLower() == data.nodes[i].getName().ToLower())
                {
                    found = true;
                    if (found)
                    {
                        int j = 1;
                        foreach (double s in data.nodes[i].posXYZ)
                        {
                            string tmp;
                            tmp = s.ToString().Replace(",", ".");
                            sw.Write(tmp + ',');
                            //tmp = string.Format("{0},", tmp);
                            csv.Append(tmp + ";");
                            if (j % 3 == 0)
                            {
                                csv.AppendLine("");
                                sw.Write(sw.NewLine);
                            }
                            j++;
                        }
                        csv.AppendLine("Number of coords:" + (data.nodes[i].posXYZ.Length / 3).ToString());
                        csv.AppendLine("Number of frames:" + data.nodes[i].frameCount.ToString());
                    }
                }
            if (!found)
            {
                Console.Clear();
                Console.WriteLine("Bone don't exist in hierarchy");
                return;
            }
            sw.Close();
            File.WriteAllText(StaticGetter.path.Substring(0, StaticGetter.path.Length-4) + nodeName +".csv", csv.ToString());
            Console.Clear();
            Console.WriteLine("Coordinates saved to file!");
            Console.WriteLine();
            Console.WriteLine("Loaded file:  " + StaticGetter.path);
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        static public void StartWindow()
        {
           GLWindow window =  new GLWindow();
            window.VSync = OpenTK.VSyncMode.Off;
            window.Run(StaticGetter._fps, StaticGetter._fps);
            

        }

        static public void Projection(ref Parser data)
        {
            Console.WriteLine(" Chose projection axis [x,y,z]");
            while (Console.KeyAvailable)
                Console.ReadKey(false);
            _axis = Console.ReadLine();
            Matrix4 mat = new Matrix4(1, 0, 0, 0,
                                                  0, 1, 0, 0,
                                                  0, 0, 1, 0,
                                                  0, 0, 0, 1);
            if (_axis == "x" || _axis == "y" || _axis == "z")
            {
                if (_axis == "x")
                {
                    mat = new Matrix4(0, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, 0,
                                      0, 0, 0, 1);
                }
                else if (_axis == "y")
                {
                    mat = new Matrix4(1, 0, 0, 0,
                                        0, 0, 0, 0,
                                        0, 0, 1, 0,
                                        0, 0, 0, 1);
                }
                else if (_axis == "z")
                {
                    mat = new Matrix4(1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 0, 0,
                                      0, 0, 0, 1);
                }

            }
            for (int i = 1; i < data.nodes.Count; i++)
            {
                    for (int j = 0; j < data.nodes[i].frameCount; j++)
                    {
                        Vector3 temp = Vector3.TransformPerspective(new Vector3((float)data.nodes[i].posXYZ[j, 0], (float)data.nodes[i].posXYZ[j, 1], (float)data.nodes[i].posXYZ[j, 2]), mat);
                        data.nodes[i].posXYZ[j, 0] = temp.X;
                        data.nodes[i].posXYZ[j, 1] = temp.Y;
                        data.nodes[i].posXYZ[j, 2] = temp.Z;
                    }
            }
            Console.WriteLine("ended");
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");

        }

    }
}

