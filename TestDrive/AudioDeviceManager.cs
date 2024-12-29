using NAudio.CoreAudioApi;

namespace TestDrive;
public class AudioDeviceManager
{
    public List<string> ListInputDevices()
    {
        List<string> inputDevices = new List<string>();
        var enumerator = new MMDeviceEnumerator();

        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
        {
            inputDevices.Add(device.FriendlyName);
        }

        return inputDevices;
    }

    public int GetDeviceIndex(string deviceName)
    {
        var enumerator = new MMDeviceEnumerator();
        int index = 0;

        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
        {
            if (device.FriendlyName.Contains(deviceName))
                return index;

            index++;
        }

        throw new Exception($"Device '{deviceName}' not found.");
    }
}
