using System;
using System.Collections.Generic;
using System.Linq;

namespace CPUScheduling
{
    class Process
    {
        public int Id { get; set; }
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int RemainingTime { get; set; }
        public int CompletionTime { get; set; }
        public int WaitingTime { get; set; }
        public int TurnaroundTime { get; set; }
        public int ResponseTime { get; set; }
        public bool Started { get; set; }

        public Process(int id, int arrivalTime, int burstTime)
        {
            Id = id;
            ArrivalTime = arrivalTime;
            BurstTime = burstTime;
            RemainingTime = burstTime;
            Started = false;
        }

        public double GetResponseRatio(int currentTime)
        {
            int waitingTime = currentTime - ArrivalTime;
            return (waitingTime + BurstTime) / (double)BurstTime;
        }
    }

    class SchedulingResults
    {
        public double AverageWaitingTime { get; set; }
        public double AverageTurnaroundTime { get; set; }
        public double CPUUtilization { get; set; }
        public double Throughput { get; set; }
        public double AverageResponseTime { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("CPU Scheduling Algorithms - SRTF and HRRN");
            Console.WriteLine("----------------------------------------");

            //Get number of processes from user
            Console.Write("Enter the number of processes: ");
            int n = int.Parse(Console.ReadLine());

            List<Process> processes = new List<Process>();

            //Input process details
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\nProcess {i + 1}:");
                Console.Write("Arrival Time: ");
                int arrivalTime = int.Parse(Console.ReadLine());
                Console.Write("Burst Time: ");
                int burstTime = int.Parse(Console.ReadLine());

                processes.Add(new Process(i + 1, arrivalTime, burstTime));
            }

            Console.WriteLine("\nChoose an algorithm:");
            Console.WriteLine("1. Shortest Remaining Time First (SRTF)");
            Console.WriteLine("2. Highest Response Ratio Next (HRRN)");
            Console.Write("Enter your choice (1 or 2): ");
            int choice = int.Parse(Console.ReadLine());

            SchedulingResults results;
            List<Process> updatedProcesses; 
            if (choice == 1)
            {
                (results, updatedProcesses) = SRTF(CloneProcesses(processes));
                Console.WriteLine("\nSRTF Scheduling Results:");
            }
            else
            {
                (results, updatedProcesses) = HRRN(CloneProcesses(processes));
                Console.WriteLine("\nHRRN Scheduling Results:");
            }

            //Display the results
            Console.WriteLine($"Average Waiting Time: {results.AverageWaitingTime:F2} ms");
            Console.WriteLine($"Average Turnaround Time: {results.AverageTurnaroundTime:F2} ms");
            Console.WriteLine($"CPU Utilization: {results.CPUUtilization:F2}%");
            Console.WriteLine($"Throughput: {results.Throughput:F4} processes/ms");
            Console.WriteLine($"Average Response Time: {results.AverageResponseTime:F2} ms");

            Console.WriteLine("\nProcess Details:");
            Console.WriteLine("ID\tArrival\tBurst\tCompletion\tTurnaround\tWaiting\tResponse");
            
            List<Process> sortedProcesses = updatedProcesses.OrderBy(p => p.Id).ToList();
            foreach (var p in sortedProcesses)
            {
                Console.WriteLine($"{p.Id,-5}\t{p.ArrivalTime,-7}\t{p.BurstTime,-5}\t{p.CompletionTime,-10}\t{p.TurnaroundTime,-10}\t{p.WaitingTime,-7}\t{p.ResponseTime,-8}");
            }
            Console.WriteLine("\nGantt Chart:");
            PrintGanttChart(updatedProcesses);
        }

        static List<Process> CloneProcesses(List<Process> originalProcesses)
        {
            List<Process> clonedProcesses = new List<Process>();
            foreach (var p in originalProcesses)
            {
                clonedProcesses.Add(new Process(p.Id, p.ArrivalTime, p.BurstTime));
            }
            return clonedProcesses;
        }

        static (SchedulingResults, List<Process>) SRTF(List<Process> processes)
        {
            //Sort processes by the arrival time
            processes.Sort((p1, p2) => p1.ArrivalTime.CompareTo(p2.ArrivalTime));
            
            int n = processes.Count;
            int currentTime = 0;
            int completedProcesses = 0;
            int totalIdleTime = 0;
            bool isIdle = false;
            
            while (completedProcesses < n)
            {
                Process selectedProcess = null;
                int shortestTime = int.MaxValue;
                
                //Find the process with shortest remaining time
                foreach (var process in processes)
                {
                    if (process.ArrivalTime <= currentTime && process.RemainingTime > 0 && process.RemainingTime < shortestTime)
                    {
                        shortestTime = process.RemainingTime;
                        selectedProcess = process;
                    }
                }
                
                //If there is no process available, move to the next arrival time
                if (selectedProcess == null)
                {
                    if (!isIdle)
                    {
                        isIdle = true;
                    }
                    
                    //Find the next arriving process
                    int nextArrival = int.MaxValue;
                    foreach (var p in processes)
                    {
                        if (p.ArrivalTime > currentTime && p.RemainingTime > 0 && p.ArrivalTime < nextArrival)
                        {
                            nextArrival = p.ArrivalTime;
                        }
                    }
                    
                    totalIdleTime += nextArrival - currentTime;
                    currentTime = nextArrival;
                    continue;
                }
                
                isIdle = false;
                
                //Track response time the first time a process starts
                if (!selectedProcess.Started)
                {
                    selectedProcess.ResponseTime = currentTime - selectedProcess.ArrivalTime;
                    selectedProcess.Started = true;
                }
                
                //Execute for 1 unit of time
                selectedProcess.RemainingTime--;
                currentTime++;
                
                //Check if the process is completed
                if (selectedProcess.RemainingTime == 0)
                {
                    completedProcesses++;
                    selectedProcess.CompletionTime = currentTime;
                    selectedProcess.TurnaroundTime = selectedProcess.CompletionTime - selectedProcess.ArrivalTime;
                    selectedProcess.WaitingTime = selectedProcess.TurnaroundTime - selectedProcess.BurstTime;
                }
            }
            
            //Calculate the performance metrics
            double totalWaitingTime = 0;
            double totalTurnaroundTime = 0;
            double totalResponseTime = 0;
            int totalBurstTime = 0;
            
            foreach (var process in processes)
            {
                totalWaitingTime += process.WaitingTime;
                totalTurnaroundTime += process.TurnaroundTime;
                totalResponseTime += process.ResponseTime;
                totalBurstTime += process.BurstTime;
            }
            
            int totalTime = currentTime;
            
            return (new SchedulingResults
            {
                AverageWaitingTime = totalWaitingTime / n,
                AverageTurnaroundTime = totalTurnaroundTime / n,
                CPUUtilization = ((totalTime - totalIdleTime) / (double)totalTime) * 100,
                Throughput = n / (double)totalTime,
                AverageResponseTime = totalResponseTime / n
            }, processes);
        }

        static (SchedulingResults, List<Process>) HRRN(List<Process> processes)
        {
            //Sort processes by the arrival time
            processes.Sort((p1, p2) => p1.ArrivalTime.CompareTo(p2.ArrivalTime));
            
            int n = processes.Count;
            int currentTime = 0;
            int completedProcesses = 0;
            int totalIdleTime = 0;
            
            //Create a list to track which processes have been completed
            List<bool> completed = Enumerable.Repeat(false, n).ToList();
            
            while (completedProcesses < n)
            {
                //Find the process with the highest response ratio
                Process selectedProcess = null;
                double highestResponseRatio = -1;
                int selectedIndex = -1;
                
                for (int i = 0; i < n; i++)
                {
                    if (!completed[i] && processes[i].ArrivalTime <= currentTime)
                    {
                        double responseRatio = processes[i].GetResponseRatio(currentTime);
                        if (responseRatio > highestResponseRatio)
                        {
                            highestResponseRatio = responseRatio;
                            selectedProcess = processes[i];
                            selectedIndex = i;
                        }
                    }
                }
                
                //If there is no process available, move to the next arrival time
                if (selectedProcess == null)
                {
                    int nextArrival = int.MaxValue;
                    foreach (var p in processes)
                    {
                        if (p.ArrivalTime > currentTime && p.ArrivalTime < nextArrival && !completed[processes.IndexOf(p)])
                        {
                            nextArrival = p.ArrivalTime;
                        }
                    }
                    
                    totalIdleTime += nextArrival - currentTime;
                    currentTime = nextArrival;
                    continue;
                }
                
                //Track the response time
                selectedProcess.ResponseTime = currentTime - selectedProcess.ArrivalTime;
                
                //Execute the process completely 
                currentTime += selectedProcess.BurstTime;
                selectedProcess.CompletionTime = currentTime;
                selectedProcess.TurnaroundTime = selectedProcess.CompletionTime - selectedProcess.ArrivalTime;
                selectedProcess.WaitingTime = selectedProcess.TurnaroundTime - selectedProcess.BurstTime;
                
                //Mark it as completed
                completed[selectedIndex] = true;
                completedProcesses++;
            }
            
            //Calculate performance metrics
            double totalWaitingTime = 0;
            double totalTurnaroundTime = 0;
            double totalResponseTime = 0;
            int totalBurstTime = 0;
            
            foreach (var process in processes)
            {
                totalWaitingTime += process.WaitingTime;
                totalTurnaroundTime += process.TurnaroundTime;
                totalResponseTime += process.ResponseTime;
                totalBurstTime += process.BurstTime;
            }
            
            int totalTime = currentTime;
            
            return (new SchedulingResults
            {
                AverageWaitingTime = totalWaitingTime / n,
                AverageTurnaroundTime = totalTurnaroundTime / n,
                CPUUtilization = ((totalTime - totalIdleTime) / (double)totalTime) * 100,
                Throughput = n / (double)totalTime,
                AverageResponseTime = totalResponseTime / n
            }, processes);
        }

        static void PrintGanttChart(List<Process> processes)
{
    var executionOrder = processes.OrderBy(p => p.CompletionTime - p.BurstTime).ToList();

    
    foreach (var p in executionOrder)
    {
        Console.Write("-------");
    }
    Console.WriteLine();

    
    foreach (var p in executionOrder)
    {
        Console.Write($"| P{p.Id}  ");
    }
    Console.WriteLine("|");

    
    foreach (var p in executionOrder)
    {
        Console.Write("-------");
    }
    Console.WriteLine();

    
    int time = executionOrder.First().CompletionTime - executionOrder.First().BurstTime;
    Console.Write($"{time, -3}");
    foreach (var p in executionOrder)
    {
        time += p.BurstTime;
        Console.Write($"{time,6}");
    }
    Console.WriteLine();
    }
    }
}