using System.Reflection;
using Agario.Infrastructure;

namespace Agario.Game.Configs;

public static class AnimationsConfig
{
    public static Dictionary<string, AnimationConfig> Dictionary = new Dictionary<string, AnimationConfig>();
    
    public static string HumanPlayerIdlePath;
    
    public static string HumanPlayerWalkingDownPath;
    public static string HumanPlayerWalkingUpPath;
    public static string HumanPlayerWalkingLeftPath;
    public static string HumanPlayerWalkingRightPath;
    
    public static string HumanPlayerEatingDownPath;
    public static string HumanPlayerEatingUpPath;
    public static string HumanPlayerEatingLeftPath;
    public static string HumanPlayerEatingRightPath;

    public static void LoadAnimationConfigs()
    {
        foreach (FieldInfo field in typeof(AnimationsConfig).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(Dictionary<string, AnimationConfig>))
                continue;
            
            if (field.GetValue(null) is string filePath)
            {
                AnimationConfig newAnimationConfig = new AnimationConfig();

                string fullPath = Path.Combine(AppContext.BaseDirectory, EntryPointConfig.AnimationsFolder, filePath);
                
                newAnimationConfig = ConfigService.LoadInstanceConfig(newAnimationConfig, fullPath) as AnimationConfig;

                Dictionary[newAnimationConfig.Name] = newAnimationConfig;
            }
        }
    }
}