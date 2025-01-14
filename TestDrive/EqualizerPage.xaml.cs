using NAudio.Wave;
using NAudio.Dsp;

namespace TestDrive;
public partial class EqualizerPage : ContentPage
{
    private readonly string audioFilePath;
    private Equalizer equalizer;
    private IWavePlayer waveOut;
    private AudioFileReader audioFile;
    private EqualizerBand[] bands;

    public EqualizerPage(string filePath)
    {
        InitializeComponent();

        audioFilePath = filePath;
        bands = new EqualizerBand[]
        {
            new EqualizerBand { Frequency = 100, Bandwidth = 0.8f, Gain = 0 },
            new EqualizerBand { Frequency = 1000, Bandwidth = 0.8f, Gain = 0 },
            //Aqui adicionar mais bandas se necessário
        };

        //Configura o leitor de áudio
        SetupAudioPlayer();
    }

    private void SetupAudioPlayer()
    {
        try
        {
            audioFile = new AudioFileReader(audioFilePath);
            equalizer = new Equalizer(audioFile, bands);
            waveOut = new WaveOutEvent();
            waveOut.Init(equalizer);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load audio file: {ex.Message}", "OK");
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider)
        {
            int bandIndex = int.Parse((string)slider.BindingContext);
            bands[bandIndex].Gain = (float)e.NewValue;

            //Atualiza o equalizador
            equalizer.Update();

            //Atualiza o Label com o valor do ganho
            string gainText = $"{e.NewValue:F1} dB";
            if (bandIndex == 0)
            {
                GainLabel_100Hz.Text = gainText;
            }
            else if (bandIndex == 1)
            {
                GainLabel_1kHz.Text = gainText;
            }
        }
    }

    private void OnApplyEqualizerClicked(object sender, EventArgs e)
    {
        try
        {
            waveOut?.Stop();
            audioFile.Seek(0, SeekOrigin.Begin);  //Reinicia o áudio a partir do início
            waveOut?.Play();  //Inicia o áudio novamente com o equalizador aplicado
            DisplayAlert("Equalizer", "Playback started with applied equalizer!", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to start playback: {ex.Message}", "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Limpar recursos
        waveOut?.Stop();
        waveOut?.Dispose();
        audioFile?.Dispose();
    }
}
