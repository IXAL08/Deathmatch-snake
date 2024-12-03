using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropiedadesUI : MonoBehaviour
{
    [SerializeField] int index = 0;
    [SerializeField] Image[] Serpiente;

    void Awake()
    {
        index = PlayerPrefs.GetInt("Color");
        ChangeColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (index == 6)
        {
            index = 0;
            ChangeColor();
        }
        if (index == -1) {
            index = 5;
            ChangeColor();
        }
    }

    public void Right()
    {
        if (index < 6)
        {
            index++;
            ChangeColor();
        } 
    }

    public void Left() {
        if (index > -1)
        {
            index--;
            ChangeColor();
        }
    }

    void ChangeColor()
    {
        switch (index)
        {
            case 0:
                for (int i = 0; i < Serpiente.Length; i++)
                {
                    Serpiente[i].color = Color.white;
                }
                break;
            case 1:
                for (int i = 0; i < Serpiente.Length; i++)
                {
                    Serpiente[i].color = Color.yellow;
                }
                break;
            case 2:
                for (int i = 0; i < Serpiente.Length; i++)
                {
                    Serpiente[i].color = Color.green;
                }
                break;
            case 3:
                for (int i = 0; i < Serpiente.Length; i++)
                {
                    Serpiente[i].color = Color.magenta;
                }
                break;
            case 4:
                for (int i = 0; i < Serpiente.Length; i++)
                {
                    Serpiente[i].color = new Color32(255, 165, 0, 255);
                }
                break;
            case 5:
                for (int i = 0; i < Serpiente.Length; i++)
                {
                    Serpiente[i].color = Color.cyan;
                }
                break;

        }
    }

    public void SaveColor() { 
        PlayerPrefs.SetInt("Color", index);

        PlayerPropiedades.LocalProperty.indexColor = index;
    }
}

