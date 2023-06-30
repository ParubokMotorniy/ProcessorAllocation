# ProcessorAllocation 🦆
A simulation of three different processor allocation algorithms created in Unity Engine

This highly-configurable simulation allows the user to see how a distributed system handles the stream of incoming jobs by employing three different strategies of job sharing among processors.
In particular:
 + With Random strategy selected, whenever a job arrives at a processor it immediately tries to forward it to another processor whose load does not exceed the minimum threshold value. The host processor probes at most "Max Migration Probes" processors and then simply gives up and puts the job into its own queue. 
 + SenderThreshold strategy represents a similar idea but with a slight twist: a processor starts asking its fellow processors to take over some of its work only in case it notices that its load exceeds the maximum threshold value. 
 + Similarly but in a quite opposite way, a processor following ReceiverThreshold strategy starts asking other processors for some extra work once it finds its load to be below the minimum threshold value.
