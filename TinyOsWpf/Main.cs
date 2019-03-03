using Hanselman.CST352;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyOsWpf
{
    public class Main
    {
        public static void Run(string[] args)
        {

            OS theOS = null;
            // Total addressable (virtual) memory taken from the command line
            uint bytesOfVirtualMemory = Properties.Settings.Default.VirtualMemory;
            uint bytesOfPhysicalMemory = Properties.Settings.Default.PhysicalMemory;
            uint bytesOfProcessMemory = Properties.Settings.Default.ProcessMemory;

            PrintHeader();

            if (args.Length < 1)
                PrintInstructions();
            else
            {
                //try
                {
                    // Set the CPU configuration
                    CPU.MemoryPageSize = Properties.Settings.Default.MemoryPageSize;
                    CPU.IsDumpRegisters = Properties.Settings.Default.DumpRegisters;
                    CPU.IsDumpInstruction = Properties.Settings.Default.DumpInstruction;
                    CPU.IsDumpPhysicalMemory = Properties.Settings.Default.DumpPhysicalMemory;
                    // Setup static physical memory
                    CPU.initPhysicalMemory(bytesOfPhysicalMemory);

                    // Create the Memory Manager with Virtual Memory
                    MemoryManager memoryMgr = new MemoryManager(bytesOfVirtualMemory);
                    memoryMgr.SharedMemoryRegionSize = Properties.Settings.Default.SharedMemoryRegionSize;
                    memoryMgr.NumOfSharedMemoryRegions = Properties.Settings.Default.NumOfSharedMemoryRegions;

                    // Create the OS with created Memory Manager
                    theOS = new OS(memoryMgr);
                    // Set the OS configuration
                    theOS.IsDumpInstruction = Properties.Settings.Default.DumpInstruction;
                    theOS.IsPauseOnExit = Properties.Settings.Default.PauseOnExit;
                    theOS.IsDumpContextSwitch = Properties.Settings.Default.DumpContextSwitch;
                    theOS.StackSize = Properties.Settings.Default.StackSize;
                    theOS.DataSize = Properties.Settings.Default.DataSize;

                    // Let the CPU know about the OS
                    CPU.theOS = theOS;

                    //Console.WriteLine("CPU has {0} bytes of physical memory", CPU.physicalMemory.Length);
                    Console.WriteLine("OS  has {0} bytes of virtual (addressable) memory", theOS.memoryMgr.virtualMemSize);

                    // For each file on the command line, load the program and create a process
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (File.Exists(args[i]))
                        {
                            Program p = Program.LoadProgram(args[i]);
                            p.IsDumpProgram = Properties.Settings.Default.DumpProgram;
                            Process rp = theOS.createProcess(p, bytesOfProcessMemory);
                            Console.WriteLine("Process id {0} has {1} bytes of process memory and {2} bytes of heap",
                                rp.PCB.pid, bytesOfProcessMemory, rp.PCB.heapAddrEnd - rp.PCB.heapAddrStart);
                            p.DumpProgram();
                        }
                    }

                    // Start executing!
                    theOS.execute();
                }
                //catch (Exception e)
                {
                    //PrintInstructions();
                    //Console.WriteLine(e.ToString());
                }

                // Pause
                Console.WriteLine("OS execution complete.  Press Enter to continue...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Prints the static instructions on how to invoke from the command line
        /// </summary>
        private static void PrintInstructions()
        {
            Console.WriteLine("");
            Console.WriteLine("usage: [files]");
        }

        /// <summary>
        /// Prints the static informatonal header
        /// </summary>
        private static void PrintHeader()
        {
            Console.WriteLine("Scott's CST352 Virtual Operating System");
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().FullName);
            Console.WriteLine("Copyright (C) Scott Hanselman 2002. All rights reserved." + System.Environment.NewLine);
        }
    }
}
