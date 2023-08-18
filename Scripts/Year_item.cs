using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Year_item : MonoBehaviour
{
    public Text txt;
    public Text txt_small;
    public Image img_bk;
    public Image img_emoji;
    public Image img_menu_more;
    public DateTime datetime;
    public GameObject panel_nomal;
    public GameObject panel_emoji;

    public void load_item(DateTime date_new_val)
    {
        this.datetime = date_new_val;
        this.panel_emoji.SetActive(false);
        this.panel_nomal.SetActive(true);

        List<int> arr_emoji_id = new List<int>();
        for (int i = 1; i <= 12; i++)
        {
            int id_emoji = PlayerPrefs.GetInt("month_" + i.ToString("D2") + "_" + this.datetime.ToString("yyyy"), -1);
            if (id_emoji != -1) arr_emoji_id.Add(id_emoji);
        }

        if (arr_emoji_id.Count > 0)
        {
            int index_emobj = GameObject.Find("App").GetComponent<App>().view_day.get_index_max(arr_emoji_id);
            this.img_emoji.sprite = GameObject.Find("App").GetComponent<App>().view_day.sp_emoji[index_emobj];
            this.panel_emoji.SetActive(true);
            this.panel_nomal.SetActive(false);
        }
    }

    public string get_id()
    {
        return "year_"+this.datetime.ToString("yyyy");
    }

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().calendar.set_datetime_view(this.datetime);
        GameObject.Find("App").GetComponent<App>().calendar.show_calendar_all_month_in_year();
        GameObject.Find("App").GetComponent<App>().play_sound();
    }

    public void view_detail()
    {
        GameObject.Find("App").GetComponent<App>().carrot.Create_Box(this.datetime.ToString("MMM"), this.img_emoji.sprite);
    }

}
