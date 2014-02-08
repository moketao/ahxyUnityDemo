using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class MySocket
{

	Socket s = null;
	Socket temp = null;
	Thread thread1 = null;
	public string beach = null;
    string data = null;

	public void Init()
	{
		int port = 7981;
		string host = "192.168.101.139";

		IPAddress ip = IPAddress.Parse(host);
		IPEndPoint ipe = new IPEndPoint(ip, port);
		s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		s.Bind(ipe);
		s.Listen(10);

		thread1 = new Thread(new ThreadStart(RecvThread));
		thread1.Start();
	}

	void SendThread()
	{
		bool bRet = false;
		while (!bRet)
		{
			temp = s.Accept();
			bRet = true;
		}
		bool pass = true;
		while (pass)
		{ 
			string sendStr = "OK";
			byte[] bs = Encoding.ASCII.GetBytes(sendStr);
			temp.Send(bs, bs.Length, 0);
			pass = false;
		}
	}

	void RecvThread()
	{ 
		int i;
		temp = s.Accept();
		byte[] bytes=new byte[1024];
		while((i=temp.Receive(bytes))!=0)
		{
            lock (this)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Debug.Log(data.ToString());
            }
			// byte[] msg =System.Text.Encoding.ASCII.GetBytes(data);
			// temp.Send(msg);
		}
	}

	public void EndGo()
	{

		thread1.Abort();
		temp.Close();
		s.Close();
	}

    public string ReturnData()//
    {
        lock (this)
        {
            beach = data;
        }
        return beach;
    }

}
