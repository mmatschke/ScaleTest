# ScaleTest
This application is for testing a scale (Type: PFB-6000).
The scale is connected on the serial port with the following parameters.

Parameters:
COM-Port can be editing
Speed (baud) = 9600
Data bits = 8
Stop bits = 1
Parity = None
Flow control = XON/XOFF

By click on the wight button a random number between 1 and 10 is generated.
For the determination of the value, the scale is connected with the randomly determined value.
Between the connection with the port and get the result be sleep the task for 100 ms.
Only the last vlue are viewed in the application
All values are stored in a CSV-File on the path: C:\KernScaleTest


