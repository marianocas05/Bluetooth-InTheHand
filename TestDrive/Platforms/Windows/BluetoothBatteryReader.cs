using InTheHand.Net.Sockets;
using System.Text;

public class BluetoothBatteryReader
{
    private BluetoothDeviceInfo _device;
    private Stream _stream;

    public BluetoothBatteryReader(BluetoothDeviceInfo device, Stream stream)
    {
        _device = device;
        _stream = stream;
    }

    public async Task<string> GetBatteryStatusAsync()
    {
        try
        {
            if (_stream == null || !_stream.CanRead)
                return "Error: Stream is not available.";

            //Enviar comando para obter status de bateria (se o dispositivo suportar)
            byte[] batteryCommand = Encoding.ASCII.GetBytes("AT+BATT?\r\n"); // Exemplo de comando AT
            await _stream.WriteAsync(batteryCommand, 0, batteryCommand.Length);

            //Ler a resposta
            byte[] responseBuffer = new byte[1024];
            int bytesRead = await _stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);

            if (bytesRead > 0)
            {
                string response = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);

                //Verificar se a resposta contém informações de bateria
                if (response.Contains("BATT"))
                {
                    //Interpretar o valor da bateria
                    int startIndex = response.IndexOf("BATT:") + 5;
                    int endIndex = response.IndexOf("%", startIndex);
                    string batteryLevel = response.Substring(startIndex, endIndex - startIndex).Trim();

                    return $"Battery Level: {batteryLevel}%";
                }

                return "Battery information not available.";
            }

            return "No response from device.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
