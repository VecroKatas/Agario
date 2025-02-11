using System.Reflection;
using Agario.Game.Configs;
using SFML.Audio;

namespace Agario.Infrastructure.Systems.Audio;

/*public enum SoundTypes
{
    Chomp,
    Moving,
    Swap,
    GameStart,
    GameOver,
}*/

public class AudioPlayer
{
    private static Random _random = new Random();
    
    private static AudioPlayer _instance;

    private readonly string _soundFolderPath;

    private Dictionary<string, List<Sound>> SoundLibrary = new Dictionary<string, List<Sound>>();

    private AudioPlayer(string soundFolderPath)
    {
        _soundFolderPath = soundFolderPath;
    }
    
    public static AudioPlayer GetInstance(string soundFolderPath)
    {
        return _instance ??= new AudioPlayer(soundFolderPath);
    }

    public void LoadSounds(string format = "*.*")
    {
        if (!Directory.Exists(_soundFolderPath))
        {
            Console.WriteLine($"[AudioSystem] Sound folder '{_soundFolderPath}' not found.");
            return;
        }

        foreach (var field in typeof(AudioConfig).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(List<string>))
            {
                List<string> soundPaths = (List<string>)field.GetValue(null);

                List<Sound> sounds = new List<Sound>();
                foreach (var soundPath in soundPaths)
                {
                    string fullPath = Path.Combine(AppContext.BaseDirectory, EntryPointConfig.SoundsFolder, soundPath);
                    if (File.Exists(fullPath))
                    {
                        Sound sound = new Sound(new SoundBuffer(fullPath));
                        sounds.Add(sound);
                    }
                }

                SoundLibrary.Add(field.Name, sounds);
            }
        }
    }
    
    public static void PlayOnce(string soundType, float volume = 100)
    {
        if (_instance.SoundLibrary.TryGetValue(soundType, out List<Sound> sounds))
        {
            int index = _random.Next(sounds.Count);
            Sound sound = sounds[index];
            sound.Volume = volume;
            sound.Loop = false;
            sound.Play();
        }
    }
    
    public static void PlayLooped(string soundType, float volume = 100)
    {
        if (_instance.SoundLibrary.TryGetValue(soundType, out List<Sound> sounds))
        {
            int index = _random.Next(sounds.Count);
            Sound sound = sounds[index];
            sound.Volume = volume;
            sound.Loop = true;
            sound.Play();
        }
    }

    public static void Stop(string soundType)
    {
        if (_instance.SoundLibrary.TryGetValue(soundType, out List<Sound> sounds))
        {
            foreach (var sound in sounds)
            {
                sound.Stop();
            }
        }
    }

    public static void StopAllSounds()
    {
        foreach (var pair in _instance.SoundLibrary)
        {
            Stop(pair.Key);
        }
    }
}