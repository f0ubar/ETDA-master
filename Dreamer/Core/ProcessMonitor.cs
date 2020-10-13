using Binarysharp.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace Dreamer.Core
{
    public interface IProcess
    {
        Process Process { get; set; }
        MemorySharp Memory { get; set; }
        void Initialize();
        void ProcessExited();
    }
    public class ProcessMonitor<T> where T : IProcess, new()
    {
        public ProcessMonitor(string ProcessName)
        {
            this.ProcessName = ProcessName;
            Timer = new System.Timers.Timer(1000);
            Timer.Elapsed += CheckProcesses;
            Timer.Enabled = true;
        }
        public string ProcessName { get; set; }
        System.Timers.Timer Timer { get; } = new System.Timers.Timer(1000);
        public Dictionary<int, T> Processes = new Dictionary<int, T>();
        private void CheckProcesses(object sender, ElapsedEventArgs e)
        {
            if (Program.MainForm == null) return;
            Process[] currentProcesses = Process.GetProcessesByName(ProcessName);
            foreach (Process process in currentProcesses)
                if (!Processes.ContainsKey(process.Id))
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += ProcessExited;
                    T newProcess = new T()
                    {
                        Process = process,
                        Memory = new MemorySharp(process.Id)
                    };
                    Processes.Add(process.Id, newProcess);
                    ProcessLocated(process);
                }
        }
        private void ProcessLocated(Process process)
        {
            process.EnableRaisingEvents = true;
            process.Exited += ProcessExited;
            T newProcess = Processes[process.Id];
            newProcess.Initialize();
        }
        private void ProcessExited(object sender, EventArgs e)
        {
            int processId = ((Process)sender).Id;
            if (Processes.ContainsKey(processId))
            {
                T process = Processes[processId];
                process.ProcessExited();
                process.Memory.Dispose();
                process.Process.Dispose();
                Processes.Remove(processId);
            }
        }
    }
}
