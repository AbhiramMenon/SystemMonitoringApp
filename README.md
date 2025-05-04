README

System Monitoring App (.NET 6+)

This is a cross-platform desktop system monitoring tool built using .NET 6, following the Clean Architecture pattern. It tracks CPU, RAM, and Disk usage, and supports a plugin architecture for extensibility.

✅ Features

CPU, Memory, and Disk Usage Monitoring

Plugin Support (File Logger, Slack Alerts, REST API Plugin, etc.)

Cross-platform compatibility (Windows/Linux/macOS)

Easily extensible with new plugins

Configuration-driven (via appsettings.json)

Written using Clean Architecture principles

📦 Prerequisites

.NET 6 SDK

🚀 How to Run

Clone the repo:

git clone <https://github.com/AbhiramMenon/SystemMonitoringApp.git>

Restore dependencies:

dotnet restore

Build the solution:

dotnet build

Run the app:

dotnet run --project ConsoleApp

🧪 Running Tests

cd Application.Tests

dotnet test

