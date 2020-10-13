using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Binarysharp.MemoryManagement;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Test
{
    class Collections
    {
        public static Dictionary<int, Client> AttachedClients = new Dictionary<int, Client>();
    }
    class Program
    {
        //60 Frames per second!
        static TimeSpan UpdateSpan { get; set; } = new TimeSpan(0, 0, 0, 0, 16);

        //Track Frame Updates
        static DateTime LastUpdate { get; set; }
        static Thread _updatingThread;
        static List<UpdateableComponent> _components = new List<UpdateableComponent>();
        static MainForm _parentForm = new MainForm();

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Main(string[] args)");
            //Initialize Components
            SetupComponents();

            //Update Frame
            _updatingThread = new Thread(DoUpdate) { IsBackground = true };
            _updatingThread.Start();

            Application.Run(_parentForm);
        }

        //This Function Sets up Components used for this bot.
        private static void SetupComponents()
        {
            Console.WriteLine("SetupComponents()");
            //a process monitor Component, It will listen for new DA Windows,
            //and attach ETDA.DLL to them, and remove ETDA.dll when client is closed.
            var monitor = new ProcessMonitor();
            monitor.Attached += monitor_Attached;
            monitor.Removed += monitor_Removed;

            //Add this to our component list, so it will get updated in the main frame.
            _components?.Add(monitor);
        }

        //DA process was removed, unload all necessary resources.
        static void monitor_Removed(object sender, EventArgs e)
        {
            Console.WriteLine("monitor_Removed(object sender, EventArgs e)");
            //var client = Collections.AttachedClients[(int)sender];
            //client.CleanUpMememory();
            //client.DestroyResources();
            //Collections.AttachedClients.Remove((int)sender);
        }

        //DA Process was removed.
        static void monitor_Attached(object sender, EventArgs e)
        {
            Console.WriteLine("monitor_Attached(object sender, EventArgs e)");
            //create a new client class for this DA Process
            var client = new Client();

            //prepare ETDA.dll and inject it into the process.
            client.InitializeMemory(
                System.Diagnostics.Process.GetProcessById((int)sender),
                Path.Combine(Environment.CurrentDirectory, "EtDA.dll"));

            //Add to our Global collections dictionary.
            Collections.AttachedClients[(int)sender] = client;

            InjectCodeStubs(client);

            _parentForm.Invoke((MethodInvoker)delegate ()
            {
                //invoke OnAttached, so signal creation of Packet Handlers.
                client.OnAttached();
            });
        }

        public static void InjectCodeStubs(GameClient client)
        {
            Console.WriteLine("InjectCodeStubs(GameClient client)");
            if (client == null || client.Memory == null || !client.Memory.IsRunning)
                return;

            var mem = client.Memory;
            var offset = 0x697B;
            var send = mem.Read<int>((IntPtr)0x85C000, false) + offset;
            var payload = new byte[] { 0x13, 0x01 };
            var payload_length_arg =
                mem.Memory.Allocate(2,
                Binarysharp.MemoryManagement.Native.MemoryProtectionFlags.ExecuteReadWrite);
            mem.Write((IntPtr)payload_length_arg.BaseAddress, (short)payload.Length, false);
            var payload_arg =
                mem.Memory.Allocate(sizeof(byte) * payload.Length,
                Binarysharp.MemoryManagement.Native.MemoryProtectionFlags.ExecuteReadWrite);
            mem.Write((IntPtr)payload_arg.BaseAddress, payload, false);

            var asm = new string[]
            {
                "mov eax, " + payload_length_arg.BaseAddress,
                "push eax",
                "mov edx, " + payload_arg.BaseAddress,
                "push edx",
                "call " + send,
            };

            mem.Assembly.Inject(asm, (IntPtr)0x006FE000);
        }

        //this is updated 60 frames per second.
        //It's job is to update and elapsed components.
        static void DoUpdate()
        {
            Console.WriteLine("DoUpdate()");
            while (true)
            {
                //Console.WriteLine("DoUpdate() -> while (true)");
                var delta = (DateTime.Now - LastUpdate);
                try
                {
                    Update(delta);
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    LastUpdate = DateTime.Now;
                    Thread.Sleep(UpdateSpan);
                }
            }
        }

        static void Update(TimeSpan elapsedTime)
        {
            //Console.WriteLine("Update(TimeSpan elapsedTime)");
            //Update all components.
            lock (_components)
            {
                _components.ForEach(i => i.Update(elapsedTime));
            }

            //Update all attached clients in our collections dictionary, this will allow
            //any updateable components inside client to also update accordinaly to the elapsed frame.

            //copy memory here is deliberate!
            List<Client> copy;
            lock (Collections.AttachedClients)
                copy = new List<Client>(Collections.AttachedClients.Values);

            var clients = copy.ToArray();
            foreach (var c in clients)
                c.Update(elapsedTime);
        }
    }
    public class UpdateTimer
    {
        public TimeSpan Timer { get; set; }
        public TimeSpan Delay { get; set; }

        public bool Elapsed
        {
            get { return (this.Timer >= this.Delay); }
        }

        public UpdateTimer(TimeSpan delay)
        {
            this.Timer = TimeSpan.Zero;
            this.Delay = delay;
        }

        public void Reset()
        {
            this.Timer = TimeSpan.Zero;
        }
        public void Update(TimeSpan elapsedTime)
        {
            this.Timer += elapsedTime;
        }
    }
    [Serializable]
    public abstract class UpdateableComponent : IDisposable
    {
        public bool Enabled
        {
            get; set;
        }

        public GameClient Client
        {
            get; set;
        }

        public UpdateTimer Timer
        {
            get; set;
        }

        public MemorySharp _memory
        {
            get { return Client.Memory; }
        }

        public int ComponentID { get; internal set; }

        protected int lastTick;
        protected int lastFrameRate;
        protected int frameRate;

        public int CalculateFrameRate()
        {
            Console.WriteLine("CalculateFrameRate()");
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

        public double Cycle = 0;
        public abstract void Update(TimeSpan tick);
        public virtual void Pulse()
        {
            Cycle = CalculateFrameRate();
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose()");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Console.WriteLine("Dispose(bool disposing)");
            if (disposing)
            {
                Client = null;
                Timer = null;

                Console.WriteLine("Destroying Component {0} {1}", GetType().Name, ComponentID);
            }
        }

        public bool IsInGame()
        {
            Console.WriteLine("IsInGame()");
            try
            {
                if (_memory == null || !_memory.IsRunning)
                    return false;
                var x = _memory.Read<int>((IntPtr)DAStaticPointers.ObjectBase, false);
                if (x > 0)
                    x = _memory.Read<int>((IntPtr)x + 0x23C, false);
                if (x > 0)
                    return true;
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
    public enum DAStaticPointers : int
    {
        [Description("DA 7.41 No Blind Address, Write 0x75 to disable blind effects")]
        NoBlind = 0x005D2DD4,
        [Description(@"DA 7.41 Character Base Object Pointer, A commonly used pointer to common information, such as map info, location, ect")]
        ObjectBase = 0x00882E68,
        [Description("DA 7.41 ETDA custom Option Table Pointer, Used to hide gold, filter sprites, animations, ect")]
        OptionTable = 0x00750000,
        [Description("DA 7.41 Attributes Pointer, HP, MP ect")]
        Attributes = 0x00755AA4,
        [Description("DA 7.41 User ID (Unique Serial)")]
        UserID = 0x0073D944,
        [Description("DA 7.41 Movement Lock Pointer (Write 0x74 to Lock Movement, 75 to resume movement)")]
        Movement = 0x005F0ADE,
        [Description("DA 7.41 Pointer where ETDA is injected.")]
        ETDA = 0x00567FB0,
        [Description("DA 7.41 Pointer to the clients Send Buffer")]
        SendBuffer = 0x006FD000,
        [Description("DA 7.41 Pointer to the clients Recv Buffer")]
        RecvBuffer = 0x00721000,
        [Description("DA 7.41 Pointer My Players Username")]
        Username = 0x0073D910,
        [Description("DA 7.41 Pointer My Players ActiveBar ")]
        ActiveBar = 0x006887EC,
        [Description("DA 7.41 Is Player Currently Casting something? 0 = no, 1 = yes ")]
        IsCasting = 0x0073FD78
    }
    public class ProcessMonitor : UpdateableComponent
    {
        public List<int> Processes = new List<int>();
        public event EventHandler Updated = delegate { };
        public event EventHandler Attached = delegate { };
        public event EventHandler Removed = delegate { };

        public ProcessMonitor()
        {
            Console.WriteLine("ProcessMonitor()");
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(500.0));
        }

        public override void Update(TimeSpan tick)
        {
            //Console.WriteLine("Update(TimeSpan tick)");
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Timer.Reset();
                base.Pulse();

                var count = Process.GetProcessesByName("Darkages");
                if (count.Length != Processes.Count)
                {
                    var id = count.Select(i => i.Id).Except(Processes).FirstOrDefault();
                    var p = count.FirstOrDefault(i => i.Id == id);

                    SetupProcess(p);
                }

                Updated(this, new EventArgs());
            }
        }

        private void SetupProcess(Process p)
        {
            Console.WriteLine("SetupProcess(Process p)");
            //try {
            if (Processes.Contains(p.Id))
                return;

            p.EnableRaisingEvents = true;
            p.Exited += PExited;

            Processes.Add(p.Id);
            Attached(p.Id, new EventArgs());
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Error, There is a mismatch, if you run as admin, ensure you run both da and bot as admin, or both as normal.", "Bot Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    System.Diagnostics.Process.GetCurrentProcess().Kill();
            // }           
        }

        void PExited(object sender, EventArgs e)
        {
            Console.WriteLine("PExited(object sender, EventArgs e)");
            var p = (Process)sender;

            if (Processes.Contains(p.Id))
            {
                Processes.Remove(p.Id);
                Removed(p.Id, new EventArgs());
            }
        }
    }
    [Serializable]
    public abstract partial class GameClient : UpdateableComponent
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(int value);

        public enum MovementState : byte
        {
            [Description("Movement is Locked, and you cannot sent walk packets.")] Locked = 0x74,
            [Description("Movement is Free, and you can send walk packets.")] Free = 0x75
        }

        private ProgressCallback callback;
        public MemorySharp Memory { get; set; }

        public int SendPointer { get; set; }
        public int RecvPointer { get; set; }

        public GameClient()
        {
            Console.WriteLine("GameClient()");
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));
        }

        internal static string Hack { get; set; }
        public int WalkOrdinal { get; internal set; }

        [DllImport("EtDA.dll")]
        public static extern void OnAction([MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        public void InitializeMemory(Process p, string Hack)
        {
            Console.WriteLine("InitializeMemory(Process p, string Hack)");
            Memory = new MemorySharp(p);

            var injected = Memory.Read<byte>((IntPtr)DAStaticPointers.ETDA, false);
            if (injected == 85)
            {
                var HackModule = Memory.Modules.Inject(Hack);
                if (HackModule.IsValid)
                {
                    Console.Beep();
                }

                GameClient.Hack = Hack;
            }
        }

        public virtual void OnAttached()
        {
            Console.WriteLine("OnAttached()");
            //Task.Run(() => ProcessOutQueue(this));
            //Task.Run(() => ProcessInQueue(this));

            //setup the utilities
            //Utilities = new GameUtilities(this);
        }

        public override void Update(TimeSpan tick)
        {
            //Console.WriteLine("Update(TimeSpan tick)");
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                try
                {
                    //UpdateComponents(tick);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    Timer.Reset();
                }
            }
        }
    }
    [Serializable]
    public class Client : GameClient
    {
        public Client()
        {
            Console.Write("Client()");
            Client = this;
        }

        public new Client OnAttached()
        {
            Console.Write("Client OnAttached()");
            base.OnAttached();
            return this;
        }
    }
}