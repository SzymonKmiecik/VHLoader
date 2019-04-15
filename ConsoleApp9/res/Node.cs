using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConsoleApp9
{
   public class node
    {
        public node()
        {

        }
        public node (string Name, double[] Offset, string[] Channel, int ChannelIndex = 0 , int Parent= 0)
        {
            name = Name;
            offset = Offset;
            channels = Channel;
            parent = Parent;
            channelIndex = ChannelIndex;
        }

        public bool isRoot = false;
        private string name;
        public double[] offset = new double[3];
        public string[] channels;
        private int parent;
        private int channelIndex;
        private  Motion motion = null;
        public Matrix4[] transform;
        public Vector4[] locTrans;
        public Vector3 origin;
        public int frameCount;
        public double[,] posXYZ;
        

        public void setNode (string Name, double Offsetx, double Offsety, double Offsetz, string[] Channels)
        {
            name = Name;
            offset[0] = Offsetx;
            offset[1] = Offsety;
            offset[2] = Offsetz;
            channels = Channels;
        }
        public void getNode()
        {
            Console.WriteLine("Name: " + name + " \nOffsets: " + offset[0] + " " + offset[1] + " " + offset[2] + " " + "\nChannels: " + channels[0] + " " + channels[1] + " " + channels[2] + " " + channels[3] + " " + channels[4] + " " + channels[5] + "\nParent: " + parent);
        }
        public string getName()
        {
            return name;
        }

        public int getParent()
        {
            if (parent == 0)
                return 0; 
            return parent;
        }
        public void setParent( int Parent)
        {
             parent = Parent;
        }
        public int getChannelIndex()
        {
            return channelIndex;
        }
        public void setMotion(double[,] Values, int FrameCount = 0, double FrameTime = 0.0)
        {
            motion = new Motion(Values, FrameCount, FrameTime);
        //    StaticGetter.file.inputOrigi();
            getTransformation();
        }
        public void getMotion()
        {
             motion.getValues(); 
        }

        public void getTransformation()
        {
            transform = GLTransformM.computeMatrices(ref this.motion, ref channels, ref offset);
            locTrans =  GLTransformM.localTransform(ref transform, ref origin);
            this.frameCount = this.motion.frameCount;
            posXYZ = new double[frameCount, 3];
            int index = 0;
            foreach (Matrix4 x in transform)
            {
                double[] xyz = new double[3];
                xyz = GLTransformM.computeXYZ((GLTransformM.computeGobalMatrices(x, this.parent,  index)), ref origin);
                posXYZ[index,0] = xyz[0];
                posXYZ[index,1] = xyz[1];
                posXYZ[index,2] = xyz[2];
                index++;
            }
            
          
            this.motion = null;
        }

    }
}
