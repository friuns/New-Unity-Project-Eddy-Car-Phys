using System.IO;
using System.Text;
using UnityEngine;

//namespace MyNamespace
//{


    public class BinaryWriter : System.IO.BinaryWriter
    {
        public BinaryWriter(Stream output) : base(output)
        {
        }
        public BinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }
        public void Write(Vector3 vector3)
        {
            Write(vector3.x);
            Write(vector3.y);
            Write(vector3.z);
        }
        public void Write(Quaternion vector3)
        {
            Write(vector3.x);
            Write(vector3.y);
            Write(vector3.z);
            Write(vector3.w);
        }
    }

    public class BinaryReader : System.IO.BinaryReader
    {
        public BinaryReader(Stream input) : base(input)
        {
        }
        public BinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }
        public Vector3 ReadVector()
        {
            Vector3 v = new Vector3();
            v.x = ReadSingle();
            v.y = ReadSingle();
            v.z = ReadSingle();
            return v;
        }
        public override int Read()
        {
            return ReadInt32();
        }
        public Quaternion ReadQuater()
        {
            Quaternion v = new Quaternion();
            v.x = ReadSingle();
            v.y = ReadSingle();
            v.z = ReadSingle();
            v.w = ReadSingle();
            return v;
        }


    }
//}
