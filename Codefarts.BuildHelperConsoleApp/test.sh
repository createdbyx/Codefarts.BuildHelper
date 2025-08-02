#!/bin/bash

# Run your application
#buildhelper
dotnet "/media/createdbyx/DriveE/OneDrive/Programming/Code Projects/Codefarts.BuildHelper/Codefarts.BuildHelperConsoleApp/bin/Debug/net8.0/BuildHelper.dll"

# Capture the exit code
exit_code=$?

# Display the exit code
echo "Exit code: $exit_code"

# Check the exit code and display a message
if [ $exit_code -eq 0 ]; then
  echo "Application executed successfully. code: " + $exit_code
else
  echo "Application execution failed. code: " + $exit_code
fi