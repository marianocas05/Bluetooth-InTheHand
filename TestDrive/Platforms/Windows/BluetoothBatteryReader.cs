using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

public class BluetoothBatteryReader
{
    public async Task<string> GetBluetoothBatteryAsync()
    {
        try
        {
            //Encontrar dispositivos emparelhados
            var devices = await DeviceInformation.FindAllAsync(
                BluetoothDevice.GetDeviceSelectorFromPairingState(true));

            if (devices.Count == 0)
                return "No paired devices found.";

            //Selecionar o primeiro dispositivo como exemplo
            var deviceInfo = devices[0];
            var device = await BluetoothDevice.FromIdAsync(deviceInfo.Id);

            if (device == null)
                return "Unable to connect to the selected device.";

            //Obter serviços Rfcomm
            var rfcommResult = await device.GetRfcommServicesAsync();
            if (rfcommResult.Services.Count == 0)
                return "No services found on the device.";

            var selectedService = rfcommResult.Services[0];

            //Criar um novo socket para a conexão
            using (var socket = new StreamSocket())
            {
                await socket.ConnectAsync(selectedService.ConnectionHostName, selectedService.ConnectionServiceName);

                //Ler dados do serviço
                var buffer = new Windows.Storage.Streams.Buffer(1024);
                var result = await socket.InputStream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.Partial);
                var reader = DataReader.FromBuffer(result);
                var response = reader.ReadString(result.Length);

                //Validar resposta
                if (response.Contains("IPHONEACCEV"))
                {
                    var batteryData = response.Substring(response.IndexOf("IPHONEACCEV"));
                    var tokens = batteryData.Split(',');

                    if (tokens.Length >= 6 &&
                        int.TryParse(tokens[3], out int leftBattery) &&
                        int.TryParse(tokens[5], out int rightBattery))
                    {
                        return $"Battery Levels - Left: {leftBattery * 10}%, Right: {rightBattery * 10}%";
                    }
                }
                return "Battery data not available or unsupported format.";
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}

