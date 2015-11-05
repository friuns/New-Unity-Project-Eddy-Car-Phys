//using System;
//using System.IO;
//using System.Text;
//using UnityEngine;

//namespace MyNamespace
//{


//public class BinaryWriter : MemoryStream
//{

//    public void Write(string a)
//    {
//        var str = Encoding.UTF8.GetBytes(a);
//        Write(BitConverter.GetBytes(str.Length));
//        Write(str);
//    }

//    public void Write(int b)
//    {
//        Write(BitConverter.GetBytes(b));
//    }
//    public void Write(bool b)
//    {
//        Write((byte)(b ? 1 : 0));
//    }
//    public void Write(byte b)
//    {
//        Write(new[] { b });
//    }
//    public void Write(Vector3 vector3)
//    {
//        Write(vector3.x);
//        Write(vector3.y);
//        Write(vector3.z);
//    }
//    public void Write(Color vector3)
//    {
//        Write(vector3.r);
//        Write(vector3.b);
//        Write(vector3.g);
//        Write(vector3.a);
//    }

//    public void Write(float value)
//    {
//        Write(BitConverter.GetBytes(value));
//    }
//    public void Write(byte[] bts)
//    {
//        var bytes = bts;
//        this.Write(bytes, 0, bytes.Length);
//    }
//    public void Write(Quaternion vector3)
//    {
//        Write(vector3.x);
//        Write(vector3.y);
//        Write(vector3.z);
//        Write(vector3.w);
//    }
//}

//public class BinaryReader : MemoryStream
//{

//    public BinaryReader(byte[] buffer)
//        : base(buffer)
//    {

//    }


//    public string ReadString()
//    {
//        var len = BitConverter.ToInt32(ReadBytes(4), 0);
//        string readString = Encoding.UTF8.GetString(ReadBytes(len), 0, len);
//        return readString;
//    }

//    public Vector3 ReadVector()
//    {
//        Vector3 v = new Vector3();
//        v.x = ReadFloat();
//        v.y = ReadFloat();
//        v.z = ReadFloat();
//        return v;
//    }
//    public Color readColor()
//    {
//        Color c = new Color();
//        c.r = ReadFloat();
//        c.b = ReadFloat();
//        c.g = ReadFloat();
//        c.a = ReadFloat();
//        return c;
//    }
//    public int ReadInt()
//    {
//        return BitConverter.ToInt32(ReadBytes(4), 0);
//    }
//    public float ReadFloat()
//    {
//        return BitConverter.ToSingle(ReadBytes(4), 0);
//    }
//    public bool ReadBool()
//    {
//        return ReadByte2() == 1;
//    }
//    public byte ReadByte2()
//    {
//        var b = base.ReadByte();
//        if (b == -1) throw new Exception("stream end");
//        return (byte)b;
//    }
//    public byte[] ReadBytes(int len)
//    {
//        var b = new byte[len];
//        int a = Read(b, 0, len);
//        if (a != len) throw new Exception("stream ended");
//        return b;
//    }
//    public Quaternion ReadQuater()
//    {
//        Quaternion v = new Quaternion();
//        v.x = ReadFloat();
//        v.y = ReadFloat();
//        v.z = ReadFloat();
//        v.w = ReadFloat();
//        return v;
//    }

//}
//}
