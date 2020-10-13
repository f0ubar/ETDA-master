using Dreamer.Mapping;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Dreamer.Core;

namespace Dreamer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        Mapping.Map Map;

        private void LoadMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "JSON Map Files|*.map.json"
            };
            DialogResult result = ofd.ShowDialog(this);
            if (result == DialogResult.OK && ofd.FileName.ToLower().EndsWith(".map.json") && ofd.CheckFileExists)
            {
                Map = new Map(ofd.FileName);
                Map.PathFinder.AddWayPoint(new Mapping.Point(5, 5));
                Map.PathFinder.AddWayPoint(new Mapping.Point(Map.Bounds.X - 5, 5));
                Map.PathFinder.AddWayPoint(new Mapping.Point(Map.Bounds.X - 5, Map.Bounds.Y - 5));
                Map.PathFinder.AddWayPoint(new Mapping.Point(5, Map.Bounds.Y - 5));
                pictureBox1.Location = new System.Drawing.Point(10, menuStrip1.Size.Height + 10);
                pictureBox1.Size = new Size(300, 300);
                Mapping.Point aspect = (new Mapping.Point(pictureBox1.Size) / Map.Bounds);
                int minAspect = aspect.Min;
                Map.TabMap.Aspect = new Mapping.Point(minAspect, minAspect);
                Map.TabMap.Draw();
                CheckPictureBox();
            }
        }

        private void CheckPictureBox()
        {
            if (Map.TabMap.Bitmap != pictureBox1.Image)
            {
                Image oldImage = pictureBox1.Image;
                pictureBox1.Image = Map.TabMap.Bitmap;
                if (oldImage != null)
                    oldImage.Dispose();
            }
            else pictureBox1.Refresh();
        }

        Mapping.Point TabMapPoint;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TabMapPoint = new Mapping.Point(e.Location) / Map.TabMap.Aspect;
                AddWayPointToolStripMenuItem.Visible = Map.PathFinder.WayPoints.Where(wayPoint => wayPoint.Point == new Mapping.Point(e.Location)).Count() <= 0;
                RemoveWayPointToolStripMenuItem.Visible = !AddWayPointToolStripMenuItem.Visible;
                TabMapMenu.Show(pictureBox1.PointToScreen(e.Location));
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Mapping.Point Point = new Mapping.Point(e.X, e.Y) / Map.TabMap.Aspect;
            Text = $"Dreamer {Point}";
        }

        private void AddWayPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map.PathFinder.AddWayPoint(TabMapPoint);
            Map.TabMap.Draw();
            CheckPictureBox();
        }

        private void RemoveWayPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map.PathFinder.RemoveWayPoint(TabMapPoint);
            Map.TabMap.Draw();
            CheckPictureBox();
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
                if (!DarkagesProcess.ProcessMonitor.Processes.ContainsKey(id))
                    return;

                Marshal.Copy(ptr.LpData, buffer, 0, ptr.CbData);

                var packet = new Packet
                {
                    Date = DateTime.Now,
                    Data = buffer,
                    Type = (int)ptr.DwData,
                    DarkagesProcess = DarkagesProcess.ProcessMonitor.Processes[id]
                };

                if (packet.Type == 1)
                    Console.WriteLine("OnPacketReceived");
                packet.DarkagesProcess.PacketReceived(id, packet);
                if (packet.Type == 2)
                    Console.WriteLine("OnPacketSent");
                packet.DarkagesProcess.PacketSent(id, packet);
                Intercept(ptr, packet, id);
            }
        }

        private int CheckTouring(ref Message m, ref Copydatastruct ptr)
        {
            var id = m.WParam.ToInt32();
            if (id > 0x7FFFF && idx++ % 2 == 0)
            {
                if (ptr.DwData == 2)
                    if (DarkagesProcess.ProcessMonitor.Processes.ContainsKey(previd))
                        DarkagesProcess.ProcessMonitor.Processes[previd].SendPointer = id;
                if (ptr.DwData != 1) return id;
                if (DarkagesProcess.ProcessMonitor.Processes.ContainsKey(previd))
                    DarkagesProcess.ProcessMonitor.Processes[previd].RecvPointer = id;
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

            var c = DarkagesProcess.ProcessMonitor.Processes[id];

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
        }
    }
}
