namespace TestDrive;

public partial class RecordingPage : ContentPage
{
    private AudioRecorder recorder;
    private AudioDeviceManager deviceManager;

    public RecordingPage()
    {
        InitializeComponent();
        recorder = new AudioRecorder();
        deviceManager = new AudioDeviceManager();

#if WINDOWS
        var inputDevices = deviceManager.ListInputDevices();
        foreach (var device in inputDevices)
        {
            DevicePicker.Items.Add(device);
        }

        if (DevicePicker.Items.Count > 0)
        {
            DevicePicker.SelectedIndex = 0;
        }
#endif
    }

    private void DevicePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string selectedDevice = DevicePicker.SelectedItem.ToString();
            recorder.InputDeviceIndex = deviceManager.GetDeviceIndex(selectedDevice);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to set device: {ex.Message}", "OK");
        }
    }

    private void StartButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            recorder.StartRecording();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            DisplayAlert("Recording", "Audio recording started...", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to start recording: {ex.Message}", "OK");
        }
    }

    private void StopButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            recorder.StopRecording();
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            DisplayAlert("Recording Stopped", $"Audio saved to: {recorder.GetFilePath()}", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to stop recording: {ex.Message}", "OK");
        }
    }
}
