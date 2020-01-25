using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Newtonsoft.Json;

namespace Aurora.Devices.Asus
{
    [Serializable]
    public class MyObject : ISerializable
    {
        public int n1;
        public int n2;
        public String str;

        public MyObject()
        {
        }

        protected MyObject(SerializationInfo info, StreamingContext context)
        {
            n1 = info.GetInt32("i");
            n2 = info.GetInt32("j");
            str = info.GetString("k");
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("i", n1);
            info.AddValue("j", n2);
            info.AddValue("k", str);
        }
    }


    class SocketServer
    {
        static UdpClient client;
        IPEndPoint ep = null;

        public void Start()
        {
            client = new UdpClient("230.0.0.1", 9092);
            Console.WriteLine("CreatedClient");
        }
            internal void Send(Dictionary<DeviceKeys, Color> keyColors)
        {
            try
            {
                string json = JsonConvert.SerializeObject(keyColors);
                client.Send(StrToByteArray(json), json.Length);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public static byte[] StrToByteArray(string str)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetBytes(str);
        }
    }
}
