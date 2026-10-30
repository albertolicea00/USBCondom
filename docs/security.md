# Security model

This prototype reduces accidental file opening inside its own interface. It cannot intercept Windows process creation or prevent Explorer, another application, or an interpreter from reading the USB. Blocking arbitrary document opening conflicts with allowing ordinary reads unless access is mediated by a privileged broker.

Deploy Windows application control separately and validate policies for executables, installers, scripts, interpreters and removable volumes. Killing a process after it starts is not execution prevention. No policy is installed by this application.

Extension checks and the two-byte MZ heuristic are triage only. They do not validate MP4, PDF, Office files, archives, polyglots or scripts disguised as data. A clean Defender result is not proof of safety. Archives and macros need a separate policy.

Incoming and Safe are writable by the current user. They are not tamper-proof, access-controlled quarantine or non-executable storage. Another process can change files before or after scanning. Source path checks do not eliminate device substitution or link races. Do not use with adversarial devices on a production workstation until a privileged broker and Windows policies enforce isolation.

The app does not modify USB contents, but it does not enforce read-only mounting. Use Windows Safely Remove Hardware; forced ejection is not implemented.

Reference: [Microsoft Defender command-line arguments](https://learn.microsoft.com/en-us/defender-endpoint/command-line-arguments-microsoft-defender-antivirus). Custom scans use ScanType 3, a specific folder and DisableRemediation so a nonzero threat result holds the batch.
