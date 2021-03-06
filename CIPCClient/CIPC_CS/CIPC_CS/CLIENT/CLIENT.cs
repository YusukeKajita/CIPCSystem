﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;

namespace CIPC_CS.CLIENT
{
    public class CLIENT : BASE
    {
        #region constructer
        public CLIENT()
            : base(SETTINGS.MyInfo.Port,SETTINGS.RemoteExchangeServer.IP, SETTINGS.RemoteExchangeServer.Port,SETTINGS.MyInfo.Name, SETTINGS.MyInfo.fps)
        {
        }

        public CLIENT(int myPort, string remoteIP, int serverPort)
            :base(myPort,remoteIP,serverPort,SETTINGS.MyInfo.Name, SETTINGS.MyInfo.fps)
        {
        }
        public CLIENT(int myPort, string remoteIP, int serverPort, string name, int fps)
            : base(myPort, remoteIP, serverPort, name, fps)
        {

        }
        ~CLIENT() 
        {
            try
            {
                this.Close();
            }catch(Exception ex)
            {

            }
        }

        
        #endregion

        #region private method
        protected override void Connect_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.udp_client.Send(enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            while (true)
            {
                if (this.udp_client.Received_Data != null)
                {
                    dec.Source = this.udp_client.Received_Data;
                    break;
                }
            }
            if (this.dec.get_int() != SETTINGS.ConnectionCommands.REPLY.OK)
            {
                throw new Exception("接続失敗");
            }
            this.udp_client.Close();
            this.udp_client = null;
            Thread.Sleep(100);
            this.remotePort = dec.get_int();
        }

        protected override void Connect_Sender_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.enc += this.fps;
            this.enc += SETTINGS.ConnectionCommands.MODE.SEND;
            this.enc += this.name;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            while (true)
            {
                if (this.udp_client.Received_Data != null)
                {
                    dec.Source = this.udp_client.Received_Data;
                    break;
                }
            }
        }

        protected override void Connect_Both_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.enc += this.fps;
            this.enc += SETTINGS.ConnectionCommands.MODE.BOTH;
            this.enc += this.name;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            while (true)
            {
                if (this.udp_client.Received_Data != null)
                {
                    dec.Source = this.udp_client.Received_Data;
                    break;
                }
            }
            this.udp_client.DataReceived += CLIENT_DataReceived;
        }

        protected override void Connect_Direct_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.enc += this.fps;
            this.enc += SETTINGS.ConnectionCommands.MODE.DIRECT;
            this.enc += this.name;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            while (true)
            {
                if (this.udp_client.Received_Data != null)
                {
                    dec.Source = this.udp_client.Received_Data;
                    break;
                }
            }
            this.udp_client.DataReceived += CLIENT_DataReceived_Direct;
        }

        private void CLIENT_DataReceived_Direct(object sender, byte[] e)
        {
            this.dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            this.dec.Source = e;
            if (dec.get_int() == 0)
            {
                this.directIP = dec.get_string();
                this.directPort = dec.get_int();

                this.udp_client.DataReceived -= CLIENT_DataReceived_Direct;
                this.udp_client.Close();
                this.udp_client = null;

                Thread.Sleep(100);

                this.udp_client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.directIP, this.directPort, this.myPort);
                this.udp_client.DataReceived += CLIENT_DataReceived;
            }
        }

        protected override void Connect_Receiver_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.enc += this.fps;
            this.enc += SETTINGS.ConnectionCommands.MODE.RECEIVE;
            this.enc += this.name;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            while (true)
            {
                if (this.udp_client.Received_Data != null)
                {
                    dec.Source = this.udp_client.Received_Data;
                    break;
                }
            }
            this.udp_client.DataReceived += CLIENT_DataReceived;
        }

        void CLIENT_DataReceived(object sender, byte[] e)
        {
            OnDataReceived(e);
        }

        protected override void Close_add()
        {
            this.udp_client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.serverIP, this.serverPort, this.myPort);
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += SETTINGS.ConnectionCommands.GREETING.END;
            enc += this.remotePort;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            while (true)
            {
                if (this.udp_client.Received_Data != null)
                {
                    dec.Source = this.udp_client.Received_Data;
                    break;
                }
            }
            if (dec.get_int() == SETTINGS.ConnectionCommands.GREETING.END)
            {

            }
            else 
            {


            }
            this.udp_client.Close();
            this.udp_client = null;
        }
        #endregion

        #region Static Method
        public static void Connect(int SenderID, int ReceiverID,string RemoteIP,int RemotePort,int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)5;
            enc += SenderID;
            enc += ReceiverID;

            client.Send(enc.data);
            client.Close();
        }
        public static void ConnectByServerPort(int SenderServerPort, int ReceiverServerPort, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)11;
            enc += SenderServerPort;
            enc += ReceiverServerPort;

            client.Send(enc.data);
            client.Close();
        }
        public static void ConnectByUserPort(int SenderUserPort, int ReceiverUserPort, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)13;
            enc += SenderUserPort;
            enc += ReceiverUserPort;

            client.Send(enc.data);
            client.Close();
        }

        public static void Disconnect(int SenderID, int ReceiverID, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)6;
            enc += SenderID;
            enc += ReceiverID;

            client.Send(enc.data);
            client.Close();
        }
        public static void DisconnectByServerPort(int SenderServerPort, int ReceiverServerPort, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)12;
            enc += SenderServerPort;
            enc += ReceiverServerPort;

            client.Send(enc.data);
            client.Close();
        }
        public static void DisconnectByUserPort(int SenderUserPort, int ReceiverUserPort, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)14;
            enc += SenderUserPort;
            enc += ReceiverUserPort;

            client.Send(enc.data);
            client.Close();
        }
        #endregion
    };
}
