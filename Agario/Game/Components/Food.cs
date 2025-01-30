using Agario.Game.Interfaces;
using Agario.Infrastructure;

namespace Agario.Game.Components;

public class Food : IComponent
{
    public float NutritionValue { get; set; }
    public Action OnBeingEaten { get; set; }

    public GameObject GameObject;

    public Food(float nutritionValue)
    {
        NutritionValue = nutritionValue;
    }

    public void SetParentGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public void BeingEaten()
    {
        OnBeingEaten.Invoke();
    }
}