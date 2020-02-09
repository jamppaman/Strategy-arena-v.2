using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIsystem
{
    public class HitText : MonoBehaviour
    {

        public GameObject hitTextPrefab;
        public Vector3 offset;
        public Color damageColor;
        public Color healColor;
        public Color zeroColor;

        public void DamageText(CharacterSystem.CharacterValues target, int damage)
        {
            GameObject hit = hitTextPrefab;
            hit.transform.position = new Vector3(target.currentTile.x, 0, target.currentTile.z) + offset;

            hit.GetComponent<TextMesh>().text = "" + damage;
            if (damage < 0)
            {
                hit.GetComponent<TextMesh>().color = damageColor;
            }
            else if (damage > 0)
            {
                hit.GetComponent<TextMesh>().color = healColor;
            }
            else if (damage == 0)
            {
                hit.GetComponent<TextMesh>().color = zeroColor;
            }

            Instantiate(hit, hit.transform.position, hit.transform.rotation);


        }

    }

}