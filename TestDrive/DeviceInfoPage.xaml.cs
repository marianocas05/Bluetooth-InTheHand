using InTheHand.Net.Sockets;

namespace TestDrive;

public partial class DeviceInfoPage : ContentPage
{
    public DeviceInfoPage(BluetoothDeviceInfo device)
    {
        InitializeComponent();

        //Mostrar as informações do dispositivo
        DeviceNameLabel.Text = $"{device.DeviceName}";
        DeviceAddressLabel.Text = $"Device Address: {device.DeviceAddress}";
        DeviceStatusLabel.Text = $"Status: {(device.Authenticated ? "Authenticated" : "Not Authenticated")}";
        BatteryLevelLabel.Text = $"To be completed";
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        //Voltar para a página anterior
        await Navigation.PopAsync();
    }
}
