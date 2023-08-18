using UnityEngine;
using UnityEngine.UI;

public class Item_emoji : MonoBehaviour
{
    public Image img_icon;
    public Image img_bk;
    public Text txt_tip;
    public int index_emoji;

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().view_day.set_emoji_day(this);
    }
}
