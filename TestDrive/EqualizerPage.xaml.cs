using NAudio.Wave;
using NAudio.Dsp;
using System.Collections.ObjectModel;

namespace TestDrive;
public partial class EqualizerPage : ContentPage
{
    private readonly string audioFilePath;
    private Equalizer equalizer;
    private IWavePlayer waveOut;
    private AudioFileReader audioFile;
    private ObservableCollection<EqualizerBand> bands;

    public EqualizerPage(string filePath)
    {
        InitializeComponent();

        audioFilePath = filePath;
        bands = new ObservableCollection<EqualizerBand>
        {
            new EqualizerBand { Frequency = 100, Bandwidth = 0.8f, Gain = 0 },   //Frequ�ncia para graves (voz grossa)
            new EqualizerBand { Frequency = 1000, Bandwidth = 0.8f, Gain = 0 },  //Frequ�ncia para m�dia (clareza da voz)
            new EqualizerBand { Frequency = 3000, Bandwidth = 0.8f, Gain = 0 },  //Frequ�ncia para m�dios-agudos (sibil�ncia)
            new EqualizerBand { Frequency = 5000, Bandwidth = 0.8f, Gain = 0 },  //Frequ�ncia para agudos (brilho da voz)
        };

        //Configura o leitor de �udio
        SetupAudioPlayer();

        // Vincula as bandas ao ListView
        BandsListView.ItemsSource = bands;
    }

    private void SetupAudioPlayer()
    {
        if (!File.Exists(audioFilePath))
        {
            DisplayAlert("Error", "File not found.", "OK");
            return;
        }

        try
        {
            audioFile = new AudioFileReader(audioFilePath);
            equalizer = new Equalizer(audioFile, bands.ToArray());
            waveOut = new WaveOutEvent();
            waveOut.Init(equalizer);
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Couldn't load audio file: {ex.Message}", "OK");
        }
    }


    private void OnGainTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
        {
            if (float.TryParse(entry.Text, out float newGain))
            {
                // Limita o ganho entre -12 e +12 dB
                newGain = Math.Clamp(newGain, -12f, 12f);
                if (newGain != float.Parse(entry.Text)) // O valor foi ajustado
                {
                    entry.Text = newGain.ToString("F1"); // Atualiza a interface com o valor ajustado
                }

                // Atualiza o ganho na banda correspondente
                var band = entry.BindingContext as EqualizerBand;
                if (band != null)
                {
                    band.Gain = newGain;
                    equalizer.Update(); // Atualiza imediatamente
                    waveOut?.Play(); // Reinicia a reprodu��o se necess�rio
                }
            }
        }
    }


    private void OnApplyEqualizerClicked(object sender, EventArgs e)
    {
        try
        {
            waveOut?.Stop();
            audioFile.Seek(0, SeekOrigin.Begin);  //Reinicia o �udio a partir do in�cio
            waveOut?.Play();  //Inicia o �udio novamente com o equalizador aplicado
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
