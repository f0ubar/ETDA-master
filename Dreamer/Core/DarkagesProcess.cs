using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Binarysharp.MemoryManagement;
using System.ComponentModel;

namespace Dreamer.Core
{
    public class DarkagesProcess : IProcess
    {
        public static ProcessMonitor<DarkagesProcess> ProcessMonitor = new ProcessMonitor<DarkagesProcess>("Darkages");
        public DarkagesProcess() { }
        public Process Process { get; set; }
        public MemorySharp Memory { get; set; }
        public int SendPointer { get; set; }
        public int RecvPointer { get; set; }
        public void Initialize()
        {
            InjectModule();
            InjectCodeStubs();
            Program.MainForm.Invoke((MethodInvoker)delegate ()
            {
                //invoke OnAttached, so signal creation of Packet Handlers.
                //
                Console.WriteLine("Attached to client");
            });
        }
        private void InjectModule()
        {
            if (Memory.Read<byte>((IntPtr)StaticPointers.ETDA, false) == 85)
                Memory.Modules.Inject(Path.Combine(Environment.CurrentDirectory, "EtDA.dll"));
        }
        private void InjectCodeStubs()
        {
            if (Memory == null || !Memory.IsRunning)
                return;
            var offset = 0x697B;
            var send = Memory.Read<int>((IntPtr)0x85C000, false) + offset;
            var payload = new byte[] { 0x13, 0x01 };
            var payload_length_arg =
                Memory.Memory.Allocate(2,
                Binarysharp.MemoryManagement.Native.MemoryProtectionFlags.ExecuteReadWrite);
            Memory.Write(payload_length_arg.BaseAddress, (short)payload.Length, false);
            var payload_arg =
                Memory.Memory.Allocate(sizeof(byte) * payload.Length,
                Binarysharp.MemoryManagement.Native.MemoryProtectionFlags.ExecuteReadWrite);
            Memory.Write(payload_arg.BaseAddress, payload, false);
            var asm = new string[]
            {
                "mov eax, " + payload_length_arg.BaseAddress,
                "push eax",
                "mov edx, " + payload_arg.BaseAddress,
                "push edx",
                "call " + send,
            };
            Memory.Assembly.Inject(asm, (IntPtr)0x006FE000);
        }
        public void ProcessExited()
        {

        }
        public enum StaticPointers : int
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
        public void PacketReceived(int Id, Packet Packet)
        {

        }
        public void PacketSent(int Id, Packet Packet)
        {

        }
    }
}
