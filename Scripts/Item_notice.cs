using System;
using UnityEngine;
using UnityEngine.UI;

public class Item_notice : MonoBehaviour
{
    public Text txt_title;
    public Text txt_tip;
    public Image[] btn_notice_menu;
    public GameObject obj_btn_notice_edit;
    public string id;
    public int index_notice=-1;
    public Calendar_item calendar_day_item;
    public DateTime datetime;
    public GameObject obj_line_none;
    private Carrot.Carrot_Window_Msg msg_question;

    public void btn_show_list_notice_in_month()
    {
        GameObject.Find("App").GetComponent<App>().calendar.btn_show_list_notice();
    }

    public void btn_show_list_all_notice()
    {
        GameObject.Find("App").GetComponent<App>().calendar.btn_show_list_all_notice();
    }

    public void set_sel_menu(int index_sel)
    {
        this.btn_notice_menu[0].color = GameObject.Find("App").GetComponent<App>().calendar.color_notice_menu_nomal;
        this.btn_notice_menu[1].color = GameObject.Find("App").GetComponent<App>().calendar.color_notice_menu_nomal;
        this.btn_notice_menu[index_sel].color = GameObject.Find("App").GetComponent<App>().calendar.color_notice_menu_sel;
    }

    public void btn_edit()
    {
        if (this.calendar_day_item != null) GameObject.Find("App").GetComponent<App>().view_day.show(this.calendar_day_item, 2);
        GameObject.Find("App").GetComponent<App>().carrot.close();
    }

    public void btn_delete()
    {
        this.msg_question=GameObject.Find("App").GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("appointment_schedule", "Appointment schedule"), PlayerPrefs.GetString("event_delete_question","Are you sure you want to delete this event reminder?"),this.act_yes_delete, act_no_delete);
    }

    private void act_yes_delete()
    {
        PlayerPrefs.DeleteKey(this.id);
        Debug.Log("Delete notice ID:" + this.id + " index: " + this.index_notice);
        GameObject.Find("App").GetComponent<App>().notice.delete_buy_index(this.index_notice);
        GameObject.Find("App").GetComponent<App>().calendar.freshen_calander_day_in_month();
        GameObject.Find("App").GetComponent<App>().notice.get_list_notice();
        GameObject.Find("App").GetComponent<App>().carrot.close();
        Destroy(this.gameObject);
        this.msg_question.close();
    }

    private void act_no_delete()
    {
        this.msg_question.close();
    }

    public void btn_show_point()
    {
        GameObject.Find("App").GetComponent<App>().calendar.show_calendar_by_month_year(this.datetime.Year, this.datetime.Month, this.datetime.Day);
        GameObject.Find("App").GetComponent<App>().carrot.close();
    }

    public void set_old_notice()
    {
        this.obj_line_none.SetActive(true);
    }
}
