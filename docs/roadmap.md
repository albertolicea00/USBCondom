# Roadmap

1. Validate the desktop prototype, add bounded scanning, progress, quotas, durable job manifests and automated pipeline tests.
2. Build a Windows service with WMI volume events, restricted IPC, SQLite audit records and service-owned staging. Include USB fixed disks.
3. Design and test Windows application-control deployment with administrator-managed policy; report actual enforcement health.
4. Add robust file-format identification, archive and macro policy, and race-resistant import handles.
5. Add controlled release, real safe ejection and optional read-only device policy with explicit administrator deployment.

The full five-phase product is not yet implemented. In particular, this prototype does not claim system-wide execution blocking or an antivirus replacement.
