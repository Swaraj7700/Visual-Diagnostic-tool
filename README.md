Visual-Debugger-for-Visual-C++
A Visual Diagnostic tool for Debugging and other Applications in Visual Studio
System Requirements
Visual Studio 2022
x86 configuration
Aim
The main aim of this project is to reflect the current status of the code while debugging in a kind of diagrammatic way that is easy for the user to visualize.
Show the contents of variables as well as data structures along with the memory stack.
Workflow
Iterate through the running processes in the operating system. Identify the target process and obtain its handle and perform stackwalk operation on the main thread. Checks machine type etc.
![image](https://github.com/user-attachments/assets/8b3f643d-1d98-4a96-a5f2-69776d4b80eb)
Relation between components of the C++ DLL

Screenshot 2024-07-12 203721

Marshalling data from C++ DLL to WPF

Screenshot 2024-07-12 203843

Visualize the UI, you can improv on this definitely, we gave up by the end of it

Screenshot 2024-07-12 203927

Improvements Possible
Read and marshal complex data structures using custom Marshaller and COM Objects.
Allow visualization of multithreaded applications.
Visualization of interlinked connections between data structures across different functions and even multiple threads.
Support visualization of new features from VC++ 11 onwards, such as smart pointers, etc.
