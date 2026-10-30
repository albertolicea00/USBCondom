using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace USBCondom;
public record Device(string Root, string Name);
public partial class MainWindow : Window
{
    readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(3) };
    readonly CancellationTokenSource lifetime = new();
    bool busy;
    public MainWindow() { InitializeComponent(); timer.Tick += (_, _) => Refresh(); }
    void OnLoaded(object sender, RoutedEventArgs e) { Refresh(); timer.Start(); }
    void OnClosed(object? sender, EventArgs e) { timer.Stop(); lifetime.Cancel(); }
    void Refresh()
    {
        if (busy) return;
        var devices = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable).Select(d => {
            try { return new Device(d.Name, d.IsReady ? $"{d.VolumeLabel} ({d.Name})" : $"Not ready ({d.Name})"); }
            catch (IOException) { return new Device(d.Name, d.Name); }
        }).ToArray();
        var old = (Drives.SelectedItem as Device)?.Root;
        if (Drives.ItemsSource is Device[] previous && previous.SequenceEqual(devices)) return;
        Drives.ItemsSource = devices;
        Drives.SelectedItem = devices.FirstOrDefault(d => d.Root == old) ?? devices.FirstOrDefault();
    }
    void OnSelection(object sender, SelectionChangedEventArgs e)
    {
        Heading.Text = (Drives.SelectedItem as Device)?.Name ?? "Connect a removable drive";
        Files.ItemsSource = null;
        Inspect.IsEnabled = Import.IsEnabled = !busy && Drives.SelectedItem is Device;
    }
    async Task Run(Func<Device, Task> action)
    {
        if (busy || Drives.SelectedItem is not Device device) return;
        busy = true; Inspect.IsEnabled = Import.IsEnabled = false;
        try { await action(device); }
        catch (OperationCanceledException) { Status.Text = "Operation cancelled. Import was not released."; }
        catch (Exception ex) { Status.Text = "Operation failed: " + ex.Message; }
        finally { busy = false; Inspect.IsEnabled = Import.IsEnabled = Drives.SelectedItem is Device; }
    }
    async void OnInspect(object sender, RoutedEventArgs e) => await Run(async device => {
        Status.Text = "Inspecting…";
        var entries = await Task.Run(() => Pipeline.Inspect(device.Root, lifetime.Token));
        Files.ItemsSource = entries.Select(f => $"{(f.Blocked ? "FLAGGED" : "Unverified")}  {f.Relative}");
        Status.Text = $"{entries.Count} files · {entries.Count(f => f.Blocked)} flagged. No files opened.";
    });
    async void OnImport(object sender, RoutedEventArgs e) => await Run(async device => {
        Status.Text = "Copying to Incoming, checking file types, then scanning with Microsoft Defender…";
        var result = await Task.Run(() => Pipeline.ImportAsync(device.Root, lifetime.Token));
        Status.Text = result;
    });
    void OnEject(object sender, RoutedEventArgs e) => MessageBox.Show("Wait for operations to finish, then use Windows Safely Remove Hardware in the taskbar. USBCondom does not force removal.", "Eject device");
}
