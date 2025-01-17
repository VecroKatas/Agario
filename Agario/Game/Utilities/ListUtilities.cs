namespace Agario.Game.Utilities;

public static class ListUtilities
{
    public static void SwapRemove<T>(this List<T> list, T objectToRemove)
    {
        int index = list.IndexOf(objectToRemove);

        if (index != -1)
        {
            list[index] = list[^1];
            list.RemoveAt(list.Count - 1);   
        }
    } 
}