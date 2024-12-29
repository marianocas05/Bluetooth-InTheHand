using InTheHand.Net.Sockets;
using Microsoft.Maui.Controls;

#if WINDOWS
using Windows.Devices.Bluetooth;
#endif

namespace TestDrive;

public partial class DeviceInfoPage : ContentPage
{
#if WINDOWS
    private BluetoothBatteryReader _batteryReader;
    private BluetoothDeviceInfo _device;
#endif

    public DeviceInfoPage(BluetoothDeviceInfo device)
    {
        InitializeComponent();
#if WINDOWS
        // Salvar o dispositivo recebido
        _device = device;
        // Inicializar o leitor de bateria
        _batteryReader = new BluetoothBatteryReader();
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
            // Obter informações de bateria do dispositivo
            string batteryStatus = await _batteryReader.GetBluetoothBatteryAsync();
            BatteryLevelLabel.Text = batteryStatus; // Atualizar o rótulo com o status da bateria
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
        await Navigation.PushAsync(new RecordingPage());
    }
}
