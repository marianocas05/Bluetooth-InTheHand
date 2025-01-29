using NAudio.Wave;

namespace TestDrive;
public class AudioRecorder
{
    private WaveInEvent waveIn;
    private WaveFileWriter writer;
    private string outputFilePath;

    public int InputDeviceIndex { get; set; } = 0; //Por padrão, o primeiro dispositivo

    public void StartRecording()
    {
        ConfigureDevice(); //Certifica-se de que está configurado o dispositivo correto

        outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recording.wav");

        waveIn.WaveFormat = new WaveFormat(44100, 1); //Configuração padrão
        waveIn.DataAvailable += (s, e) =>
        {
            writer?.Write(e.Buffer, 0, e.BytesRecorded);
        };

        writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
        waveIn.StartRecording();
    }

    private void ConfigureDevice()
    {
        if (waveIn != null)
        {
            waveIn.StopRecording();
            waveIn.Dispose();
        }

        waveIn = new WaveInEvent
        {
            DeviceNumber = InputDeviceIndex //Configura o dispositivo
        };
    }

    public void StopRecording()
    {
        waveIn?.StopRecording();
        waveIn?.Dispose();
        writer?.Close();
        writer?.Dispose();
        waveIn = null;
        writer = null;
    }

    public string GetFilePath() => outputFilePath;
}