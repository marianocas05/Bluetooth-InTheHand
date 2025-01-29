using InTheHand.Net.Sockets;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;

#if WINDOWS
using Windows.Devices.Bluetooth;
#endif

namespace TestDrive;

public partial class DeviceInfoPage : ContentPage
{
#if WINDOWS
    private BluetoothBatteryReader _batteryReader;
#endif

    private readonly BluetoothDeviceInfo _device;
    private readonly Stream _stream;

    public DeviceInfoPage(BluetoothDeviceInfo device, Stream stream)
    {
        InitializeComponent();

        _device = device;
        _stream = stream;

#if WINDOWS
        // Inicializar o leitor de bateria
        _batteryReader = new BluetoothBatteryReader(device,stream);
#endif

        //Mostrar informações do dispositivo na interface
        DeviceNameLabel.Text = $"{device.DeviceName}";
        DeviceAddressLabel.Text = $"Device Address: {device.DeviceAddress}";
        DeviceStatusLabel.Text = $"Status: {(device.Authenticated ? "Authenticated" : "Not Authenticated")}";
        BatteryLevelLabel.Text = "Battery: Not checked yet";
    }

    private async void OnBatteryCheckClicked(object sender, EventArgs e)
    {
#if WINDOWS
        try
        {
            string batteryStatus = await _batteryReader.GetBatteryStatusAsync();
            BatteryLevelLabel.Text = batteryStatus;
            await DisplayAlert("Battery Status", batteryStatus, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to read battery: {ex.Message}", "OK");
        }
#else
        await DisplayAlert("Unsupported", "Battery check is only supported on Windows.", "OK");
#endif
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        //Voltar para a página anterior
        await Navigation.PopAsync();
    }

    private async void OnNextButtonClicked(object sender, EventArgs e)
    {
#if WINDOWS
        await Navigation.PushAsync(new RecordingPage());
    }
#else
        var audioManager = new AudioManager();
        await Navigation.PushAsync(new RecordingPage2(audioManager, _device, _stream));
        //await Navigation.PushAsync(new RecordingPage());
    }
 #endif  
}
