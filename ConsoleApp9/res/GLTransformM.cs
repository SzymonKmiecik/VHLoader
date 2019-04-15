using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

/// <summary>
/// Class is for compute transformation matrices for nodes for each frames.
/// </summary>

namespace ConsoleApp9
{
    public class GLTransformM
    {
        static public Matrix4[] computeMatrices(ref Motion m, ref string[] channels, ref double[] offset)
        {
            GameWindow window = new GameWindow(800, 600);
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 matrix = Matrix4.CreatePerspectiveFieldOfView(45.0f*3.14f/180.0f, window.Width / window.Height, 1.0f, 100.0f);
            GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4[] matrices = new Matrix4[m.frameCount];
            for (int i = 0; i< m.frameCount; i++)
            {
                GL.LoadIdentity();
                GL.Translate(offset[0], offset[1], offset[2]);
                int x = 0;
                foreach (string s in channels)
                {
                    if (s == "Xposition")
                        GL.Translate(m.values[i, x], 0, 0);
                    else if (s == "Yposition")
                        GL.Translate(0, m.values[i, x], 0);
                    else if (s == "Zposition")
                        GL.Translate(0, 0, m.values[i, x]);
                    else if (s == "Xrotation")
                        GL.Rotate(m.values[i, x], 1.0, 0.0, 0.0);
                    else if (s == "Yrotation")
                        GL.Rotate(m.values[i, x], 0.0, 1.0, 0.0);
                    else if (s == "Zrotation")
                        GL.Rotate(m.values[i, x], 0.0, 0.0, 1.0);
                    x++;
                }
                
                GL.GetFloat(GetPName.ModelviewMatrix, out matrices[i]);
            }
            return matrices; 
        }
        static public Vector4[] localTransform(ref Matrix4[] transform, ref Vector3 origin)
        {
            Vector4[] loc = new Vector4[transform.Length];
            for (int i = 0; i < transform.Length; i++)
            {
               Vector4 tmp = (new Vector4(origin, 1.0f) * transform[i]);
                loc[i] = tmp;
            }
            return loc;
        }


        static public double[] computeXYZ(Matrix4 transform, ref Vector3 origin)
        {
           // Matrix4 mat = new Matrix4(  1, 0, 0, 0,
           //                             0, 1, 0, 0,
           //                             0, 0, 0, 0,
           //                             0, 0, 0, 1);
            //Matrix4 rot = new Matrix4();
            //GL.LoadIdentity();
            //GL.Rotate(90, 1.0, 0.0, 0.0);
            //GL.GetFloat(GetPName.ModelviewMatrix, out rot);

            double[] cartXYZ = new double[3];

            {
                //Vector3 temp = Vector3.TransformPerspective(origin, transform);
                //temp = Vector3.TransformPerspective(temp, rot); 
                //transform.Transpose()
                Vector4 temp = new Vector4(origin, 1.0f) * transform;
                cartXYZ[0] = temp.X;
                cartXYZ[1] = temp.Y;
                cartXYZ[2] = temp.Z;
                
            }

            return cartXYZ;
        }
        static public Matrix4 computeGobalMatrices(Matrix4 localTransform, int parent, int farmeNumber)
        { 
            {
                if (parent > 0)
                {
                    return computeGobalMatrices(localTransform * StaticGetter.file.nodes[parent].transform[farmeNumber], StaticGetter.file.nodes[parent].getParent(), farmeNumber);
                    // return localTransform * computeGobalMatrices(StaticGetter.file.nodes[parent].transform[farmeNumber], StaticGetter.file.nodes[parent].getParent(), farmeNumber);
                }
                else return localTransform;
            }
        }

        static public Vector4 computeGobalMatrices(Vector4 localTransform, int parent, int farmeNumber)
        {
            {
                if (parent > 0)
                {
                    //return computeGobalMatrices(localTransform * StaticGetter.file.nodes[parent].transform[farmeNumber], StaticGetter.file.nodes[parent].getParent(), farmeNumber);
                     return localTransform * computeGobalMatrices(StaticGetter.file.nodes[parent].transform[farmeNumber], StaticGetter.file.nodes[parent].getParent(), farmeNumber);
                }
                else return localTransform * StaticGetter.file.nodes[1].transform[farmeNumber];
            }
        }
    }
}
