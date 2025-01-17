using Agario.Game.Interfaces;
using Agario.Infrastructure;

namespace Agario.Game.Components;

public class FoodComponent : IComponent
{
    public float NutritionValue { get; set; }
    public Action OnBeingEaten;

    public GameObject GameObject;

    public FoodComponent(float nutritionValue)
    {
        NutritionValue = nutritionValue;
    }

    public void SetGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public void BeingEaten()
    {
        OnBeingEaten.Invoke();
    }
}