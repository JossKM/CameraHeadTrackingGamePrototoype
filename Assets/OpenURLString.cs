using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURLString : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    string URLToOpen;

    public void Open()
    {
        Application.OpenURL(URLToOpen);
    }
}
