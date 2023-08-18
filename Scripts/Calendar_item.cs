using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_item : MonoBehaviour
{
    public Text txt;
    public Text txt_small;
    public Image img_bk;
    public Image img_emoji;
    public GameObject panel_nomal;
    public GameObject panel_emoji;
    public GameObject obj_icon_notice;
    public GameObject obj_icon_note;
    public GameObject obj_icon_photo;
    public int id_notice_sys = -1;
    private DateTime datetime;
    private int index_emoji = -1;
    private bool is_notice = false;
    private bool is_note = false;
    private bool is_photo = false;
    private Day_Status day_status;

    public void load_item(DateTime date_new_val)
    {
        this.datetime = date_new_val;
        this.panel_emoji.SetActive(false);
        this.panel_nomal.SetActive(true);
        this.obj_icon_notice.SetActive(false);
        this.obj_icon_note.SetActive(false);
        this.obj_icon_photo.SetActive(false);

        if (PlayerPrefs.GetString(this.get_id_notice(), "") != "")
        {
            this.obj_icon_notice.SetActive(true);
            this.is_notice = true;
        }
        if (PlayerPrefs.GetString(this.get_id_note(), "") != "")
        {
            this.obj_icon_note.SetActive(true);
            this.is_note = true;
        }

        if (PlayerPrefs.GetString(this.get_id_photo(), "") != "")
        {
            this.obj_icon_photo.SetActive(true);
            this.is_photo = true;
        }

        this.index_emoji = PlayerPrefs.GetInt(this.get_id(), -1);
        if (this.index_emoji != -1)
        {
            this.panel_emoji.SetActive(true);
            this.panel_nomal.SetActive(false);
        }
    }

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().view_day.show(this);
    }

    public string get_id()
    {
        return "day_"+this.datetime.ToString("dd_MM_yyyy");
    }

    public string get_id_notice()
    {
        return this.get_id()+"_notice";
    }

    public int get_index_notice()
    {
        return PlayerPrefs.GetInt(this.get_id_notice()+"_index");
    }

    public string get_id_note()
    {
        return this.get_id() + "_note";
    }

    public string get_id_photo()
    {
        return this.get_id() + "_photo";
    }

    public DateTime get_datetime()
    {
        return this.datetime;
    }

    public Day_Status get_status()
    {
        return this.day_status;
    }

    public void set_status(Day_Status day_s)
    {
        if (day_s==Day_Status.day_cur)
        {
            this.GetComponent<Animator>().enabled = true;
            if(this.GetComponent<Animator>().isActiveAndEnabled) this.GetComponent<Animator>().Play("calendar_item");
        }
        this.day_status = day_s;
    }

    public int get_index_emobij()
    {
        return this.index_emoji;
    }

    public void set_index_emoji(int index_emoji)
    {
        this.index_emoji = index_emoji;
    }

    public bool get_status_notice()
    {
        return this.is_notice;
    }

    public void set_status_notice(bool is_notice)
    {
        this.is_notice = is_notice;
    }

    public bool get_status_note()
    {
        return this.is_note;
    }

    public void set_status_note(bool is_note)
    {
        this.is_note = is_note;
    }

    public bool get_status_photo()
    {
        return this.is_photo;
    }

    public void set_status_photo(bool is_photo)
    {
        this.is_photo = is_photo;
    }

    public void play_point()
    {
        this.GetComponent<Animator>().enabled = true;
        this.GetComponent<Animator>().Play("calendar_item_point");
    }

    public bool is_none_data()
    {
        if (this.is_note == false && this.is_notice == false && this.index_emoji == -1)
            return true;
        else
            return false;
    }


}
