# Operations

Use a disposable Windows 11 VM and a non-sensitive USB for evaluation. Install the .NET 8 SDK and ensure Microsoft Defender is available and active. Launch the application as a standard user.

1. Connect a removable drive and check that its label appears.
2. Inspect harmless sample files and confirm the count.
3. Add an inert text file named sample.ps1 and confirm it is flagged.
4. Add a harmless file whose first bytes are MZ with a .mp4 extension; confirm it is flagged.
5. Import a flagged batch and confirm it remains in Incoming.
6. Import harmless files and confirm a successful Defender scan precedes release to Safe.
7. Test device removal, unreadable files, nested directories and cancellation. Confirm failures never report release.
8. Verify that the UI cannot open source files and that ejection instructions use the Windows taskbar.

Review retained batches manually. Do not launch retained content. Storage quotas, retention, durable audit logs and recovery UI are not implemented. Close the app before manually removing abandoned imports. Hardware, Defender integration and visual checks are pending.
