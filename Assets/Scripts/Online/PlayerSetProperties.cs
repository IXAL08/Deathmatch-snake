using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetProperties : NetworkBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    public override void OnStartNetwork()
    {
        StartCoroutine(SetPlayerData());
    }
    IEnumerator SetPlayerData()
    {
        while (PlayerPropiedades.Instance.Properties.ContainsKey(Owner) == false)
        {
 
            yield return null;
        }

        PlayerPropiedades.PlayerProperty property = PlayerPropiedades.Instance.Properties[Owner];


        switch (property.indexColor)
        {
            case 0:
                spriteRenderer.color = Color.white;
                break;
            case 1:
                spriteRenderer.color = Color.yellow;
                break;
            case 2:
                spriteRenderer.color = Color.green;
                break;
            case 3:
                spriteRenderer.color = Color.magenta;
                break;
            case 4:
                spriteRenderer.color = new Color32(255, 165, 0, 255);
                break;
            case 5:
                spriteRenderer.color = Color.cyan;
                break;
        }
    }

}
