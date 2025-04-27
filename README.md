# CPU Scheduling Simulation

## Overview
This project simulates two CPU scheduling algorithms:

- Shortest Remaining Time First (SRTF)
- Highest Response Ratio Next (HRRN)

## Prerequisites
- Visual Studio or any compatible C# IDE
- .NET 6.0 or later

## How to Run
1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/Felipe123459/CPU-Scheduling.git
2. - Open the solution in Visual Studio or a compatible C# IDE.
3. - Compile and run Program.cs.
4. - Follow the prompts to input process details and select an algorithm.

## Outputs
The program generates the following key metrics:
- **Average Waiting Time**: Indicates efficiency in process waiting.
- **Average Turnaround Time**: Measures the total time taken by processes.
- **CPU Utilization**: Shows CPU efficiency during execution.
- **Throughput**: Number of processes completed per unit time.
- **Average Response Time**: Captures the initial response time for processes.
- **Gantt Chart (text-based)**: Visualizes process execution over time.

Sample outputs are included in `/Output/SampleOutput.txt`.

## Notes
- HRRN is non-preemptive.
- SRTF is preemptive and executes processes per time unit.

---
