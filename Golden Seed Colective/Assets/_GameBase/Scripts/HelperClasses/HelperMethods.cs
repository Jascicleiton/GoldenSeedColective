using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    /// <summary>
    ///  Gets components of ype T at box with center point, size and angle. Returns true if at least one component was found and the found component(s) is(are) returned in the list
    /// </summary>
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle = 0f)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        if (collider2DArray.Length > 0)
        {
            // Loop through all colliders to get an object of type T
            for (int i = 0; i < collider2DArray.Length; i++)
            {
                T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
                else
                {
                    tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                    if (tComponent != null)
                    {
                        found = true;
                        componentList.Add(tComponent);
                    }
                }
            }
        }

        listComponentsAtBoxPosition = componentList;

        return found;
    }

    /// <summary>
    /// Returns an array of components of type T at a box area, with center point (point), size and angle. The numberOfCollidersToTest is passed
    /// as a parameter. Found components are returned in the array
    /// </summary>
    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle = 0f)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];
        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);
        T tcomponent = default(T);
        T[] componentArray = new T[collider2DArray.Length];

        for (int i = collider2DArray.Length - 1; i >= 0; i--)
        {
            if (collider2DArray[i] != null)
            {
                tcomponent = collider2DArray[i].gameObject.GetComponent<T>();
                if (tcomponent != null)
                {
                    componentArray[i] = tcomponent;
                }
            }
        }
        return componentArray;
    }

    /// <summary>
    /// Gets components of type T. Returns true if al least one found, and the found components are returned in compomentsAtPositionList
    /// </summary>
    public static bool GetComponentAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;
        List<T> componentList = new List<T>();
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        T tComponent = default(T);

        //LoopThrough all colliders to get an object of type T
        for (int i = 0; i < collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }
        componentsAtPositionList = componentList;
        return found;
    }
}
