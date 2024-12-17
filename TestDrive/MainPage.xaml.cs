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

    //protected override void OnDisappearing()
    //{
    //    base.OnDisappearing();

    //    //Fechar recursos Bluetooth e fluxo
    //    if (stream is not null)
    //    {
    //        stream.Dispose();
    //        stream = null;
    //    }

    //    if (client.Connected)
    //    {
    //        client.Close();
    //    }
    //}

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            ConnectionStatusLabel.Text = "Status: Connecting...";

            //Usar o BluetoothDevicePicker para escolher um dispositivo compatível
            var picker = new BluetoothDevicePicker();
            picker.ClassOfDevices.Add(new ClassOfDevice(DeviceClass.AudioVideoUnclassified, ServiceClass.Audio));

            //Tentar selecionar um dispositivo
            device = await picker.PickSingleDeviceAsync();
            if (device == null)
            {
                ConnectionStatusLabel.Text = "Status: Disconnected";
                await DisplayAlert("No Device", "No device was selected or found.", "OK");
            }
            else
            {
                //Verificar se o dispositivo já está emparelhado, caso contrário, emparelhar
                if (!device.Authenticated)
                {
                    bool paired = BluetoothSecurity.PairRequest(device.DeviceAddress, null);
                    if (!paired)
                    {
                        await DisplayAlert("Error", "Pairing failed. Please retry.", "OK");
                        return;
                    }
                }

                //Conectar ao dispositivo via HFP (única forma que deu)
                client.Connect(device.DeviceAddress, BluetoothService.Handsfree);
                if (client.Connected)
                {
                    stream = client.GetStream();
                    await DisplayAlert("Success", "Connection successful!", "OK");

                    ConnectionStatusLabel.Text = "Status: Connected";

                    //Iniciar o loop de leitura do stream de bytes diretamente
                    //await Task.Run(() => StreamLoop());

                    await Navigation.PushAsync(new DeviceInfoPage(device));
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Couldn't connect to the device", "OK");
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    //private void StreamLoop()
    //{
    //    // Exemplo de loop de leitura do stream de bytes
    //    try
    //    {
    //        byte[] buffer = new byte[1024]; 
    //        while (stream != null && stream.CanRead)
    //        {
    //            int bytesRead = stream.Read(buffer, 0, buffer.Length); // Lê bytes diretamente do stream
    //            if (bytesRead > 0)
    //            {
    //                // Processar os bytes lidos
    //                Debug.WriteLine($"Received {bytesRead} bytes");
    //                // Adicionar lógica para manipular os dados de áudio aqui
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"Stream loop error: {ex.Message}");
    //    }
    //}

}

// Possível problema: só estão a aparecer na lista dispositivos que já tenham sido emparelhados
