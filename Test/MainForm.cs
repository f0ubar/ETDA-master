﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private int idx;
        private int previd;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x004A)
            {
                var ptr = (Copydatastruct)Marshal.PtrToStructure(m.LParam, typeof(Copydatastruct));
                if (ptr.CbData <= 0)
                    return;

                var buffer = new byte[ptr.CbData];
                var id = CheckTouring(ref m, ref ptr);

                if (!Collections.AttachedClients.ContainsKey(id))
                    return;

                Marshal.Copy(ptr.LpData, buffer, 0, ptr.CbData);

                var packet = new Packet
                {
                    Date = DateTime.Now,
                    Data = buffer,
                    Type = (int)ptr.DwData,
                    Client = Collections.AttachedClients[id]
                };

                if (packet.Type == 1)
                    Console.WriteLine("OnPacketReceived");
                    //Collections.AttachedClients[id].OnPacketRecevied(id, packet);
                if (packet.Type == 2)
                    Console.WriteLine("OnPacketSent");
                    //Collections.AttachedClients[id].OnPacketSent(id, packet);

                Intercept(ptr, packet, id);
            }
        }

        private int CheckTouring(ref Message m, ref Copydatastruct ptr)
        {
            var id = m.WParam.ToInt32();
            if (id > 0x7FFFF && idx++ % 2 == 0)
            {
                if (ptr.DwData == 2)
                    if (Collections.AttachedClients.ContainsKey(previd))
                        Collections.AttachedClients[previd].SendPointer = id;
                if (ptr.DwData != 1) return id;
                if (Collections.AttachedClients.ContainsKey(previd))
                    Collections.AttachedClients[previd].RecvPointer = id;
            }
            else
            {
                previd = id;
            }
            return id;
        }


#if TESTMODE

#endif

        private static void Intercept(Copydatastruct ptr, Packet packet, int id)
        {
            if (packet.Data.Length <= 0 || packet.Data.Length != ptr.CbData)
                return;

            var c = Collections.AttachedClients[id];

            if (packet.Type == 2)
            {
                Console.WriteLine($"Client->Server: {packet.ToString()}");
                //c.ClientPacketHandler[packet.Data[0]]?.Invoke(id, packet);
            }
            else if (packet.Type == 1)
            {
                Console.WriteLine($"Server->Client: {packet.ToString()}");
                //c.ServerPacketHandler[packet.Data[0]]?.Invoke(id, packet);
            }
            else if (packet.Type == 3)
            {

            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Copydatastruct
        {
            public readonly uint DwData;
            public readonly int CbData;
            public readonly IntPtr LpData;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            CleanupMemory();
        }

        private static void CleanupMemory()
        {
            foreach (var client in Collections.AttachedClients.Values)
            {
                //client.CleanUpMememory();
                //client.DestroyResources();
            }

            new Thread(delegate ()
            {
                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
            }).Start();
        }
    }
}