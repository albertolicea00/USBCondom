# Architecture

The initial implementation is one unelevated desktop process. A DispatcherTimer discovers removable volumes. Inspection and import run off the UI thread. Closing the window cancels active work. Device discovery is based on DriveType.Removable; USB-attached fixed disks may not appear.

The import pipeline is: removable source → unique Incoming directory → copied-byte inspection → Defender custom scan → directory move to Safe. A flagged file holds the entire batch. Links and reparse points abort inspection. Failed copies, denied access, disconnection and Defender failures prevent release. Empty directories are not preserved.

The application does not use a service, SQLite, WMI events, kernel hooks or a Defender SDK. Defender is invoked using MpCmdRun.exe. A future service needs restricted IPC, authenticated clients, service-owned storage and durable job records before privileged deployment.
