using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotCore.Types;
using System.Drawing;
using BotCore.BotForms;
using BotCore.Actions;
using System.Runtime.InteropServices;
using BotCore.Shared.Memory;
using System.Collections.Generic;
using Bot;

namespace BotCore
{
    public partial class BotInterface : Form
    {
        public Client client { get; set; }
        public MiniInterace MiniWindow { get; set; }

        public BotInterface(Client client)
        {
            InitializeComponent();
            this.client = client;
            MiniWindow = new MiniInterace(client);
            MiniWindow.MdiParent = Collections.ParentForm;
            MiniWindow.Dock = DockStyle.Bottom;
            WizardBotThread = new WizardBotThread(client);
            //Task.Run(() => { UpdateClient(); });
            //Task.Run(() => { BotThread(); });
            Task.Run(() => { WizardBotThread.ThreadLoop(); });
            VisibleChanged += BotInterface_VisibleChanged;
        }

        WizardBotThread WizardBotThread;

        public Color GetBlendedColor(int percentage)
        {
            return Color.FromArgb(percentage, Color.Green);
        }

        private void BotInterface_VisibleChanged(object sender, EventArgs e)
        {
            if (client.ClientReady && client.IsInGame() && !IsDisposed)
                Text = client.Attributes.PlayerName;
        }

        private System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        private ushort RandomNumber(ushort MaxValue)
        {
            byte[] Data = new byte[2];
            rng.GetBytes(Data);
            return (ushort)(BitConverter.ToUInt16(Data, 0) % MaxValue);
        }

        private Position RandomMapPosition()
        {
            return new Position((int)RandomNumber((ushort)client.FieldMap.Width), (int)RandomNumber((ushort)client.FieldMap.Height));
        }

        private List<PathFinding.PathSolver.PathFinderNode> RandomWalkableMapPosition(int MinDistance)
        {
            List<PathFinding.PathSolver.PathFinderNode> PathToMapObject = null;
            while (PathToMapObject == null)
            {
                Position pos = RandomMapPosition();
                while (client.Attributes.ServerPosition.DistanceFrom(pos) < MinDistance)
                    pos = RandomMapPosition();
                PathToMapObject = client.FieldMap.Search(client.Attributes.ServerPosition, pos);
            }
            return PathToMapObject;
        }

        private void BotThread()
        {
            var lastPosition = new Position(short.MaxValue, short.MaxValue);
            DateTime lastPositionChange = DateTime.Now,
                lastWalkingDate = DateTime.Now;
            int i = 0;
            Dictionary<int, MapObject> targetObjects = new Dictionary<int, MapObject>();
            Position walkingTarget = null;
            List<PathFinding.PathSolver.PathFinderNode> path = null;
            List<Position> LastWalks = new List<Position>();
            while (true)
            {
                if (checkBox9.Checked &&
                    client != null &&
                    client.ClientReady &&
                    client.IsInGame() &&
                    !IsDisposed &&
                    client.ObjectSearcher != null &&
                    ((DateTime.Now - lastWalkingDate < new TimeSpan(0, 0, 2)) ||
                     (DateTime.Now - lastPositionChange > new TimeSpan(0, 0, 2))))
                {
                    var position = client.Attributes.ServerPosition;
                    if (position == null) continue;
                    if (position.X != lastPosition.X || position.Y != lastPosition.Y)
                    {
                        lastPosition = position;
                        lastPositionChange = DateTime.Now;
                        continue;
                    }
                    int[] HostileSprites = new int[]
                    {
                        52
                    };
                    int[] TargetSprites = new int[]
                    {
                        53, 7
                    };
                    List<MapObject> mapObjects = client.ObjectSearcher.RetrieveMonsterTargets(o => o != null && o.Active).ToList();
                    List<MapObject> selectedMapObjects = new List<MapObject>();
                    for (int h = 0; selectedMapObjects.Count == 0 && h < HostileSprites.Length; h++)
                        selectedMapObjects = mapObjects.Where(o => o != null && o.Sprite == HostileSprites[h]).ToList();
                    if (selectedMapObjects.Count == 0)
                        selectedMapObjects = mapObjects.Where(o => o != null && TargetSprites.Contains(o.Sprite)).ToList();
                    mapObjects = selectedMapObjects;
                    List<int> objectSerials = mapObjects.Select(o => o.Serial).ToList();
                    List<int> targetObjectSerials = targetObjects.Select(o => o.Key).ToList();
                    targetObjectSerials.ForEach(o => { if (!objectSerials.Contains(o)) targetObjects.Remove(o); });
                    if (targetObjects.Count < 1)
                    {
                        mapObjects = mapObjects
                            .Where(o => !targetObjects.ContainsKey(o.Serial) && (o.ServerPosition == null || client.Attributes.ServerPosition.DistanceFrom(o.ServerPosition) <= 10))
                            .OrderBy(o => client.Attributes.ServerPosition.DistanceFrom(o.ServerPosition))
                            .ToList();
                        if (mapObjects.Count > 0)
                            targetObjects.Add(mapObjects[0].Serial, mapObjects[0]);
                    }
                    if (targetObjects.Count < 1)
                    {
                        var mppct = client.Attributes.MP / (float)client.Attributes.MaxMP;
                        if (mppct < 0.5f)
                        {
                            System.Threading.Thread.Sleep(1);
                            continue;
                        }
                        while (path == null ||
                            path.Count < 4 ||
                            client.FieldMap.IsWall((short)path[1].X, (short)path[1].Y))
                        {
                            path = RandomWalkableMapPosition(20);
                            walkingTarget = path[path.Count - 1].Position;
                            Console.WriteLine("Path changed");
                        }
                        if (path[0].X != client.Attributes.ServerPosition.X ||
                            path[0].Y != client.Attributes.ServerPosition.Y)
                        {
                            path = client.FieldMap.Search(client.Attributes.ServerPosition, walkingTarget);
                            Console.WriteLine("Path changed");
                        }
                        if (path.Count > 1)
                        {
                            var wall = client.FieldMap.IsWall((short)path[1].X, (short)path[1].Y);
                            var direction = path[0].DirectionTo(path[1]);
                            bool directionChange = client.Attributes.Direction != direction;
                            GameActions.Walk(client, direction);
                            LastWalks.Add(path[1].Position);
                            while (LastWalks.Count > 3)
                                LastWalks.RemoveAt(0);
                            int uniquex = LastWalks.Select(o => o.X).Distinct().Count(),
                                uniquey = LastWalks.Select(o => o.Y).Distinct().Count();
                            if (LastWalks.Count == 3 &&
                                uniquex == 1 &&
                                uniquey == 1)
                            {
                                path = null;
                                continue;
                            }
                            System.Threading.Thread.Sleep(500);
                            Console.WriteLine("walking");
                            lastWalkingDate = DateTime.Now;
                            if (!directionChange)
                                path.RemoveAt(0);
                            continue;
                        }
                    }
                    targetObjectSerials = targetObjects.Select(o => o.Key).ToList();
                    string[] spells = new string[]
                    {
                        //"beag srad",
                        "beag sal",
                        //"beag athar"
                    };
                    i = ++i % spells.Length;
                    MapObject currentTarget = targetObjects[targetObjectSerials[0]];
                    if (getClose.Checked && currentTarget.ServerPosition.DistanceFrom(client.Attributes.ServerPosition) > 7)
                    {
                        var xdiff = client.Attributes.ServerPosition.X - currentTarget.ServerPosition.X;
                        var ydiff = client.Attributes.ServerPosition.Y - currentTarget.ServerPosition.Y;
                        var d = Math.Abs(xdiff) > Math.Abs(ydiff) ?
                                xdiff < 0 ? Direction.East : Direction.West :
                                ydiff < 0 ? Direction.South : Direction.North;
                        var p = client.Attributes.ServerPosition + d;
                        if (!client.FieldMap.IsWall((short)p.X, (short)p.Y))
                        {
                            LastWalks.Add(p);
                            while (LastWalks.Count > 3)
                                LastWalks.RemoveAt(0);
                            int uniquex = LastWalks.Select(o => o.X).Distinct().Count(),
                                uniquey = LastWalks.Select(o => o.Y).Distinct().Count();
                            if (LastWalks.Count < 3 ||
                                (uniquex != 1 ||
                                uniquey != 1))
                            {
                                GameActions.Walk(client, d);
                                System.Threading.Thread.Sleep(500);
                                Console.WriteLine("walking");
                                lastWalkingDate = DateTime.Now;
                                continue;
                            }
                        }
                        else
                        {
                            d = Math.Abs(xdiff) > Math.Abs(ydiff) ?
                                ydiff < 0 ? Direction.South : Direction.North :
                                xdiff < 0 ? Direction.East : Direction.West;
                            p = client.Attributes.ServerPosition + d;
                            if (!client.FieldMap.IsWall((short)p.X, (short)p.Y))
                            {
                                LastWalks.Add(p);
                                while (LastWalks.Count > 3)
                                    LastWalks.RemoveAt(0);
                                int uniquex = LastWalks.Select(o => o.X).Distinct().Count(),
                                    uniquey = LastWalks.Select(o => o.Y).Distinct().Count();
                                if (LastWalks.Count < 3 ||
                                    (uniquex != 1 ||
                                    uniquey != 1))
                                {
                                    GameActions.Walk(client, d);
                                    System.Threading.Thread.Sleep(500);
                                    Console.WriteLine("walking");
                                    lastWalkingDate = DateTime.Now;
                                    continue;
                                }
                            }
                        }
                    }
                    Console.WriteLine($"Casting on sprite {targetObjects[targetObjectSerials[0]].Sprite}");
                    if (client.Utilities.CastSpell(spells[i], targetObjects[targetObjectSerials[0]]))
                        System.Threading.Thread.Sleep(300);
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        public void UpdateClient()
        {
            while (true)
            {

                if (!Disposing && !IsDisposed
                    && InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        if (client.Paused)
                        {
                            if (!button4.Enabled)
                                button4.Enabled = true;
                            if (button3.Enabled)
                                button3.Enabled = false;
                        }
                        else
                        {
                            if (button4.Enabled)
                                button4.Enabled = false;
                            if (!button3.Enabled)
                                button3.Enabled = true;
                        }

                        try
                        {
                            if (!client.ClientReady || !client.IsInGame())
                            {
                                client.ClientReady = client.IsInGame();
                                if (Visible)
                                    Hide();
                            }
                            else
                            {
                                if (!Visible && !MiniWindow.Visible && !MiniWindow.Disposing
                                && !MiniWindow.IsDisposed)
                                    Show();

                                MiniWindow.UpdateStatistics();
                                //MiniWindow.Show();
                            }
                        }
                        catch
                        {

                        }
                    });
                }
                Thread.Sleep(1500);
            }
        }

        private void BotInterface_Load(object sender, EventArgs e)
        {
            Invalidated += BotInterface_Invalidated;
            client.ObjectSearcher.OnTargetUpdated += ObjectUpdated;
            comboBox1.DataSource = client.StateMachine.States.OrderBy(i => i.Priority).Select(i => i.GetType().Name).ToList();
            button4.Enabled = false;


            //example target condition for sprite 467 (noam plain white bird)
            Collections.TargetConditions[467] = new TargetCondition()
            {
                predicate = (target) => true,
                Priority = 3
            };

            //packet editor initialize
            richTextBox1.BackColor = Color.White;
            richTextBox1.ReadOnly = true;
        }

        void BotInterface_Invalidated(object sender, InvalidateEventArgs e)
        {
            SetStateNodes();
        }

        private void SetStateNodes()
        {
            panel1.Controls.Clear();
            foreach (var s in client.StateMachine.States.Where(i => i.Enabled).OrderBy(i => i.Priority))
            {
                var spanel = new statetoggle();
                spanel.state = s;
                spanel.client = client;
                spanel.botinterface = this;
                spanel.Dock = DockStyle.Top;
                spanel.SetInfo();
                panel1.Controls.Add(spanel);
            }
            panel1.Invalidate();
        }

        private void ObjectUpdated(object sender, MapObject[] e)
        {
            var nearest = client.ObjectSearcher?.NearestTarget;

            if (nearest != null)
            {

            }
        }

        private void BotInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (Visible)
            {
                Hide();
                MiniWindow.Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeActiveState(comboBox1.Text);
        }

        public void MakeActiveState(string name)
        {
            statepanel.Controls.Clear();
            statepanel.Invalidate();

            var state = client.StateMachine.States.FirstOrDefault(i => i.GetType().Name == name);
            if (state != null)
            {
                state.InitState();
                statepanel.Controls.Add(state.SettingsInterface);
            }

            SetStateNodes();
        }


        #region Packet Editor Stuff
        bool EnablePacketEditor = false;
        bool IsReceivingPackets = false;
        bool IsSendingPackets = false;


        public void AppendText(RichTextBox box, string text, Color color, bool AddNewLine = false)
        {
            if (box.Lines.Length > 256)
                box.Clear();

            if (pausePackets)
            {
                return;
            }

            if (AddNewLine)
            {
                text += Environment.NewLine;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;

            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            EnablePacketEditor = !EnablePacketEditor;

            if (EnablePacketEditor)
            {
                client.OnPacketRecevied += EditorReceivedPacket;
                client.OnPacketSent += EditorSentPacket;
                richTextBox1.Enabled = true;
            }
            else
            {
                client.OnPacketRecevied -= EditorReceivedPacket;
                client.OnPacketSent -= EditorSentPacket;
                richTextBox1.Enabled = false;
            }
        }

        private void EditorSentPacket(object sender, Packet e)
        {
            if (!IsSendingPackets)
                return;

            AppendText(richTextBox1, "s: " + e.ToString(), Color.Blue, true);
        }

        private void EditorReceivedPacket(object sender, Packet e)
        {
            if (!IsReceivingPackets)
                return;

            AppendText(richTextBox1, "r: " + e.ToString(), Color.Red, true);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            IsReceivingPackets = !IsReceivingPackets;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            IsSendingPackets = !IsSendingPackets;
        }

        public static byte[] StringToByteArray(string hex)
        {
            try
            {
                return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Invalid packet");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var line in richTextBox2.Lines)
            {
                try
                {
                    byte[] buffer = null;
                    var newline = line;

                    if (comboBox2.Text == "Auto")
                    {
                        bool outgoing = false;

                        if (line.StartsWith("r:"))
                            newline = line.Replace("r:", string.Empty);
                        else if (line.StartsWith("s:"))
                        {
                            newline = line.Replace("s:", string.Empty);
                            outgoing = true;
                        }

                        newline = newline.Trim();
                        buffer = StringToByteArray(newline.Replace(" ", string.Empty).Trim());
                        if (buffer != null && buffer.Length <= 0)
                            continue;

                        if (outgoing)
                            GameClient.InjectPacket<ServerPacket>(client, new Packet(buffer), true);
                        else
                            GameClient.InjectPacket<ClientPacket>(client, new Packet(buffer), true);

                        continue;
                    }

                    if (line.StartsWith("r:"))
                        newline = line.Replace("r:", string.Empty);
                    else if (line.StartsWith("s:"))
                        newline = line.Replace("s:", string.Empty);

                    buffer = StringToByteArray(newline.Replace(" ", string.Empty).Trim());
                    if (buffer != null && buffer.Length <= 0)
                        continue;



                    if (comboBox2.Text == "Client")
                        GameClient.InjectPacket<ClientPacket>(client, new Packet(buffer), true);
                    if (comboBox2.Text == "Server")
                        GameClient.InjectPacket<ServerPacket>(client, new Packet(buffer), true);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }
        }

        bool pausePackets = false;

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pausePackets = true;
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && pausePackets)
                pausePackets = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            client.Paused = true;
            WizardBotThread.Paused = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client.Paused = false;
        }


        public static int OptionTable = (int)DAStaticPointers.OptionTable;

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x08] = (byte)((checkBox4.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x10] = (byte)((checkBox5.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x02] = (byte)((checkBox8.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x06] = (byte)((checkBox7.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x04] = (byte)((checkBox6.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                client.Utilities.CastSpell("Dark Seal", client);

            }).Start();

        }
    }
}
