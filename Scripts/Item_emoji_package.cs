using UnityEngine;
using UnityEngine.UI;

public class Item_emoji_package : MonoBehaviour
{
    public int index;
    public Image[] img_emoji;
    public Image img_bk;
    public GameObject obj_btn_buy;
    public GameObject obj_btn_rewarded_ads;
    public bool is_buy = false;

    public void click()
    {
        if (this.is_buy)
        {
            GameObject.Find("App").GetComponent<Emoji_Manager>().buy_package_emobj(this.index);
            GameObject.Find("App").GetComponent<App>().play_sound();
        }
        else
        {
            GameObject.Find("App").GetComponent<Emoji_Manager>().sel_package_emoji(this.index);
            GameObject.Find("App").GetComponent<App>().play_sound();
        }

    }

    public void btn_rewarded_ads()
    {
        GameObject.Find("App").GetComponent<App>().carrot.ads.show_ads_Rewarded();
        GameObject.Find("App").GetComponent<Emoji_Manager>().set_rewarded_ads_package_emobj(this.index);
        GameObject.Find("App").GetComponent<App>().play_sound();
    }

}
