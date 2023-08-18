using UnityEngine;
using UnityEngine.UI;

public class Item_note : MonoBehaviour
{
    public Text txt_title;
    public Text txt_tip;
    public Calendar_item calendar_day_item;

    public void btn_edit()
    {
        if (this.calendar_day_item != null) GameObject.Find("App").GetComponent<App>().view_day.show(this.calendar_day_item, 3);
        GameObject.Find("App").GetComponent<App>().carrot.close();
    }

    public void btn_delete()
    {
        GameObject.Find("App").GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("note", "Note"), PlayerPrefs.GetString("del_tip", "Are you sure delete this selected item?"),this.act_del_yes,this.act_del_no);
    }

    private void act_del_yes()
    {
        PlayerPrefs.DeleteKey(this.calendar_day_item.get_id_note());
        GameObject.Find("App").GetComponent<App>().calendar.freshen_calander_day_in_month();
        Destroy(this.gameObject);
    }

    private void act_del_no()
    {
        return;
    }

    public void btn_show_point()
    {
        GameObject.Find("App").GetComponent<App>().calendar.show_calendar_by_month_year(this.calendar_day_item.get_datetime().Year, this.calendar_day_item.get_datetime().Month, this.calendar_day_item.get_datetime().Day);
        GameObject.Find("App").GetComponent<App>().carrot.close();
    }
}
