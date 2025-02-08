using SFML.Audio;

namespace Agario.Infrastructure.Systems.Audio;

public enum SoundTypes
{
    Chomp,
    Moving,
    Swap,
    GameStart,
    GameOver,
}

public class AudioSystem
{
    private static AudioSystem _instance;

    private readonly string _soundFolderPath;

    private Dictionary<SoundTypes, Sound> SoundLibrary = new Dictionary<SoundTypes, Sound>();

    private AudioSystem(string soundFolderPath)
    {
        _soundFolderPath = soundFolderPath;
    }
    
    public static AudioSystem GetInstance(string soundFolderPath)
    {
        return _instance ??= new AudioSystem(soundFolderPath);
    }

    public void LoadSounds(string format = "*.*")
    {
        if (!Directory.Exists(_soundFolderPath))
        {
            Console.WriteLine($"[AudioSystem] Sound folder '{_soundFolderPath}' not found.");
            return;
        }

        foreach (string file in Directory.GetFiles(_soundFolderPath, $"*{format}"))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            
            if (Enum.TryParse(fileName, out SoundTypes soundType))
            {
                try
                {
                    SoundBuffer buffer = new SoundBuffer(file);
                    Sound sound = new Sound(buffer);
                    SoundLibrary[soundType] = sound;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AudioSystem] Error loading sound '{file}': {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[AudioSystem] Skipping file '{fileName}', no matching SoundType.");
            }
        }
    }
    
    public static void PlayOnce(SoundTypes soundType, float volume = 100)
    {
        if (_instance.SoundLibrary.TryGetValue(soundType, out Sound sound))
        {
            sound.Volume = volume;
            sound.Loop = false;
            sound.Play();
        }
    }
    
    public static void PlayLooped(SoundTypes soundType, float volume = 100)
    {
        if (_instance.SoundLibrary.TryGetValue(soundType, out Sound sound))
        {
            sound.Volume = volume;
            sound.Loop = true;
            sound.Play();
        }
    }

    public static void Stop(SoundTypes soundType)
    {
        if (_instance.SoundLibrary.TryGetValue(soundType, out Sound sound))
        {
            sound.Stop();
        }
    }

    public static void StopAllSounds()
    {
        foreach (var pair in _instance.SoundLibrary)
        {
            pair.Value.Stop();
        }
    }
}