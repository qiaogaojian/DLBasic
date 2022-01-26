using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ImageText : MonoBehaviour
{
    public Image symbol;
    public Image[] sprites;
    public Sprite[] numSprites;
    public Sprite[] symbolSprites;
    public int maxCount;

    int oldCount;
    private void Awake()
    {
        oldCount = maxCount;
    }
    [Button]
    public void Value(int value, bool hasSymbol)
    {
        if (hasSymbol)
        {
            bool isTur = value >= 0;
            symbol.gameObject.SetActive(true);
            symbol.sprite = symbolSprites[isTur ? 0 : 1];
        }
        else
        {
            if (symbol != null)
                symbol.gameObject.SetActive(false);
        }
        value = Mathf.Abs(value);

        string str = value.ToString();
        //Hack CY ->超出界限限制
        if (str.Length > sprites.Length)
        {
            str = "";
            for (int i = 0; i < sprites.Length; i++)
            {
                str += "9";
            }
        }

        int strCount = str.Length;
        if (strCount > sprites.Length) return;
        RestNum();
        for (int i = 0; i < strCount; i++)
        {
            sprites[i].gameObject.SetActive(true);
            sprites[i].sprite = numSprites[int.Parse(str[i].ToString())];
        }
        oldCount = strCount;
    }
    [Button]
    public void RestNum()
    {
        for (int i = 0; i < oldCount; i++)
        {
            sprites[i].gameObject.SetActive(false);
        }
    }
}
