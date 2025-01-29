namespace TestDrive;

using System.Diagnostics;
using InTheHand.Net.Sockets;
using Plugin.Maui.Audio;

public partial class RecordingPage2 : ContentPage
{
    readonly IAudioManager _audioManager;
    readonly IAudioRecorder _audioRecorder;
    IAudioPlayer? _audioPlayer;
    public BluetoothDeviceInfo Device { get; }
    public Stream ConnectionStream { get; }

    public RecordingPage2(IAudioManager audioManager, BluetoothDeviceInfo device, Stream stream)
    {
        InitializeComponent();

        if (audioManager == null)
        {
            throw new ArgumentNullException(nameof(audioManager), "AudioManager null.");
        }

        _audioManager = audioManager;
        _audioRecorder = audioManager.CreateRecorder();

        Device = device;
        ConnectionStream = stream;

        DeviceInfoLabel.Text = $"Recording for device: {Device.DeviceName}";
    }

    private async void OnRecordClicked(object sender, EventArgs e)
    {
        try
        {
            Debug.WriteLine("Botão de gravação clicado");

            // Verificar permissões antes de gravar
            if (await Permissions.RequestAsync<Permissions.Microphone>() != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "It's necessary to use the mic.", "OK");
                return;
            }

            Debug.WriteLine($"_audioRecorder inicializado: {_audioRecorder != null}");
            Debug.WriteLine($"_audioRecorder.IsRecording: {_audioRecorder?.IsRecording}");

            if (!_audioRecorder.IsRecording)
            {
                await _audioRecorder.StartAsync();
                ((Button)sender).Text = "Stop"; // Atualizar botão
                RecordingStatusLabel.Text = "Status: Recording...";
                Debug.WriteLine("Gravação iniciada");
            }
            else
            {
                var recordedAudio = await _audioRecorder.StopAsync();
                ((Button)sender).Text = "Start";
                RecordingStatusLabel.Text = "Status: Idle";
                Debug.WriteLine("Gravação terminada");

                if (recordedAudio == null)
                {
                    await DisplayAlert("Error", "No audio was recorded.", "OK");
                    return;
                }

                if (_audioPlayer != null)
                {
                    _audioPlayer.Dispose(); // Garantir que o player anterior foi disposto
                    _audioPlayer = null;
                }

                _audioPlayer = AudioManager.Current.CreatePlayer(recordedAudio.GetAudioStream());
                Debug.WriteLine("Áudio carregado para reproduzir");
                _audioPlayer.Play();
                Debug.WriteLine("Áudio reproduzido");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There's been an error during recording: {ex.Message}", "OK");
            Debug.WriteLine($"Erro ao gravar: {ex.Message}");
        }
    }
}
