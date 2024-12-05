using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Diagnostics;

namespace TestDrive;

public partial class MainPage : ContentPage
{
    private BluetoothClient client = new BluetoothClient();
    private BluetoothDeviceInfo device = null;
    private Stream stream = null;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Fechar recursos Bluetooth e fluxo
        if (stream is not null)
        {
            stream.Dispose();
            stream = null;
        }

        if (client.Connected)
        {
            client.Close();
        }
    }

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            // Mostrar os dispositivos
            var picker = new BluetoothDevicePicker();
            picker.ClassOfDevices.Add(new ClassOfDevice(DeviceClass.AudioVideoUnclassified, ServiceClass.Audio));
            device = await picker.PickSingleDeviceAsync();

            if (device != null)
            {
                await DisplayAlert("Found device", $"Connecting to device {device.DeviceName}...", "OK");

                // Emparelhar se necessário
                if (!device.Authenticated)
                {
                    bool paired = BluetoothSecurity.PairRequest(device.DeviceAddress, null);
                    if (!paired)
                    {
                        await DisplayAlert("Error", "Pairing failed. Please retry.", "OK");
                        return;
                    }
                }

                // Conectar ao dispositivo
                client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                if (client.Connected)
                {
                    stream = client.GetStream();
                    await DisplayAlert("Connecting", "Connection successful!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "It wasn't possible to connect to the device", "OK");
                }
            }
            else
            {
                await DisplayAlert("No device", "No device selected.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }
}

// Após tentar connectar aos meus fones aparece:
// Ocorreu um erro: One or more errors occured.
// (Specific argument was out of the range of valid issues. (Prameter 'index'))

// Possível problema: só estão a aparecer na lista dispositivos que já tenham sido emparelhados
