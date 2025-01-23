using UnityEngine;
using UnityEngine.UI;

public class Item_photo : MonoBehaviour
{
    public Image img;
    public Text txt;
    public Calendar_item calendar_item;

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().view_day.show(this.calendar_item,0);
        GameObject.Find("App").GetComponent<App>().carrot.close();
    }
}
