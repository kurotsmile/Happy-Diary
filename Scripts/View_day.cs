using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class View_day : MonoBehaviour
{
    public GameObject panel_view_day;
    public Text txt_title_view_day;

    public Image[] img_btn_menu;
    public Image[] img_btn_icon_menu;
    public GameObject[] panel_function_day;
    public Color32 color_menu_nomal;
    public Color32 color_menu_sel;

    [Header("Info day")]
    public Sprite sp_icon_add;
    public Sprite sp_icon_edit;
    public GameObject item_day_info_prefab;
    public Transform area_list_day_info;

    [Header("Emoji")]
    public Sprite[] sp_emoji;
    public string[] txt_emoji;
    public string[] txt_emoji_key;
    public GameObject item_day_emoji_prefab;
    public Transform area_list_day_emoji;

    [Header("Notice")]
    public Dropdown dropdown_evenet_notice;
    public Dropdown dropdown_evenet_notice_hours;
    public Dropdown dropdown_evenet_notice_minute;
    public InputField intp_evenet_notice;
    public GameObject panel_event_timer;
    public int[] data_time_notice;
    private int sel_time_notice_at = 0;
    private int index_notice_update = -1;
    private int index_note_update = -1;
    private int id_notice_sys = -1;

    [Header("Note")]
    public InputField intp_calendar_note;

    [Header("Photo")]
    public Image img_photo_day;
    private Item_info_day item_day_photo;


    private Calendar_item item_calendar_temp;
    private Carrot.Carrot_Window_Msg msg_question;

    public void load_view_day()
    {
        this.panel_view_day.SetActive(false);
        this.load_data_notice_at_dropdown();
    }

    public void show(Calendar_item c, int type_show = -1)
    {
        this.item_calendar_temp = c;
        this.GetComponent<App>().carrot.ads.show_ads_Interstitial();

        this.txt_title_view_day.text = c.get_datetime().ToString("dddd dd/MMMM/yyyy");
        this.panel_view_day.SetActive(true);
        this.panel_event_timer.SetActive(false);
        this.GetComponent<App>().play_sound();

        if (type_show == -1)
        {
            if (this.item_calendar_temp.is_none_data() && this.item_calendar_temp.get_status() != Day_Status.day_new)
                this.show_func_sel_emoji();
            else
                this.show_func_info();
        }
        else
        {
            if (type_show == 0) this.show_func_info();
            if (type_show == 1) this.show_func_sel_emoji();
            if (type_show == 2) this.show_func_notice();
            if (type_show == 3) this.show_func_note();
        }
        this.check_show_menu_btn();
    } 

    public void load_data_notice_at_dropdown()
    {
        this.dropdown_evenet_notice.ClearOptions();
        this.dropdown_evenet_notice.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("morning", "Morning") + " (7:00)" });
        this.dropdown_evenet_notice.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("noon", "Noon") + " (12:00)" });
        this.dropdown_evenet_notice.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("afternoon", "Afternoon") + " (17:00)" });
        this.dropdown_evenet_notice.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("night", "Night") + " (20:00)" });
        this.dropdown_evenet_notice.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("late_evening", "Late Evening") + " (23:00)" });
        this.dropdown_evenet_notice.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("detailed_customization", "Customize other detailed time") });
        this.dropdown_evenet_notice.RefreshShownValue();

        this.dropdown_evenet_notice_hours.ClearOptions();
        this.dropdown_evenet_notice_minute.ClearOptions();


        for (int i = 0; i <= 23; i++) this.dropdown_evenet_notice_hours.options.Add(new Dropdown.OptionData() { text = i + " " + PlayerPrefs.GetString("hours", "Hours") });
        for (int i = 0; i <= 59; i++) this.dropdown_evenet_notice_minute.options.Add(new Dropdown.OptionData() { text = i + " " + PlayerPrefs.GetString("minute", "Minute") });

        this.dropdown_evenet_notice_hours.value = 0;
        this.dropdown_evenet_notice_minute.value = 0;

        this.dropdown_evenet_notice_hours.RefreshShownValue();
        this.dropdown_evenet_notice_minute.RefreshShownValue();
    }

    private void hide_all_panel_func()
    {
        for (int i = 0; i < this.panel_function_day.Length; i++)
        {
            this.img_btn_menu[i].gameObject.SetActive(true);
            this.panel_function_day[i].SetActive(false);
            this.img_btn_menu[i].color = this.color_menu_nomal;
            this.img_btn_icon_menu[i].color = Color.white;
        }
    }

    private void check_show_menu_btn()
    {
        if (this.item_calendar_temp.get_status() == Day_Status.day_new) this.img_btn_menu[1].gameObject.SetActive(false);
        if (this.GetComponent<App>().carrot.os_app == Carrot.OS.Window) this.img_btn_menu[2].gameObject.SetActive(false);
    }

    private void show_func_info()
    {
        this.hide_all_panel_func();
        this.panel_function_day[0].SetActive(true);
        this.img_btn_menu[0].color = this.color_menu_sel;
        this.img_btn_icon_menu[0].color = Color.black;

        this.GetComponent<App>().carrot.clear_contain(this.area_list_day_info);

        int index_emoji = this.item_calendar_temp.get_index_emobij();
        Item_info_day item_info_emoj = this.add_item_info();
        item_info_emoj.txt_name.text = PlayerPrefs.GetString("emoji", "Emoticon");
        if (this.item_calendar_temp.get_status() != Day_Status.day_new)
        {
            if (index_emoji != -1)
            {
                item_info_emoj.img_icon.sprite = this.sp_emoji[index_emoji];
                item_info_emoj.img_icon.color = Color.white;
                item_info_emoj.txt_tip.text = this.get_label_emoji(index_emoji);
                item_info_emoj.index_func = 1;
                item_info_emoj.img_btn.sprite = this.sp_icon_edit;
                item_info_emoj.obj_btn_edit.GetComponent<Image>().color = this.color_menu_sel;
                item_info_emoj.obj_btn_del.SetActive(true);
            }
            else
            {
                item_info_emoj.img_icon.sprite = this.img_btn_icon_menu[1].sprite;
                item_info_emoj.txt_tip.text = PlayerPrefs.GetString("day_emoji_tip", "How is your day?");
                item_info_emoj.index_func = 1;
            }
        }
        else
        {
            item_info_emoj.img_icon.sprite = this.img_btn_icon_menu[1].sprite;
            item_info_emoj.txt_tip.text = PlayerPrefs.GetString("emoji_tip", "Change emoji packs for calendar");
            item_info_emoj.obj_btn_edit.SetActive(false);
        }

        if (this.GetComponent<App>().carrot.os_app != Carrot.OS.Window)
        {
            Item_info_day item_info_notice = this.add_item_info();
            item_info_notice.txt_name.text = PlayerPrefs.GetString("event", "Event");

            string s_msg_notice = PlayerPrefs.GetString(this.item_calendar_temp.get_id_notice(), "");
            if (s_msg_notice != "")
            {
                int index_notice = this.item_calendar_temp.get_index_notice();
                DateTime datetime_notice=new DateTime(long.Parse(PlayerPrefs.GetString("notice_" + index_notice + "_date")));
                if (datetime_notice > DateTime.Now)
                    item_info_notice.img_icon.sprite = this.GetComponent<App>().notice.sp_remind_on;
                else
                    item_info_notice.img_icon.sprite = this.GetComponent<App>().notice.sp_remind_off;

                item_info_notice.txt_tip.text = s_msg_notice;
                item_info_notice.index_func = 2;
                item_info_notice.img_btn.sprite = this.sp_icon_edit;
                item_info_notice.obj_btn_edit.GetComponent<Image>().color = this.color_menu_sel;
                item_info_notice.obj_btn_del.SetActive(true);
                //item_info_notice.obj_btn_ics.SetActive(true);
            }
            else
            {
                item_info_notice.img_icon.sprite = this.img_btn_icon_menu[2].sprite;
                item_info_notice.txt_tip.text = PlayerPrefs.GetString("event_no", "No events have been set yet"); ;
                item_info_notice.index_func = 2;
            }
        } 

        Item_info_day item_info_note = this.add_item_info();
        item_info_note.txt_name.text = PlayerPrefs.GetString("note", "Note");
        item_info_note.index_func = 3;
        if (this.item_calendar_temp.get_status_note())
        {
            item_info_note.img_icon.sprite = this.GetComponent<App>().calendar.sp_icon_list_note;
            item_info_note.txt_tip.text = PlayerPrefs.GetString(this.item_calendar_temp.get_id_note());
            item_info_note.img_btn.sprite = this.sp_icon_edit;
            item_info_note.obj_btn_edit.GetComponent<Image>().color = this.color_menu_sel;
            item_info_note.obj_btn_del.SetActive(true);
        }
        else
        {
            item_info_note.img_icon.sprite = this.GetComponent<App>().calendar.sp_icon_list_note;
            item_info_note.txt_tip.text = PlayerPrefs.GetString("note_tip", "Write in your diary a day, good memories, memorable things that you don't want to forget");
        }

        this.item_day_photo = this.add_item_info();
        item_day_photo.index_func = 4;
        item_day_photo.obj_btn_camera.SetActive(true);
        item_day_photo.txt_name.text = PlayerPrefs.GetString("photo_souvenir", "Souvenir photos");
        item_day_photo.txt_tip.text = PlayerPrefs.GetString("photo_souvenir_tip", "Let's save these commemorative pictures and dates");
        Sprite sp_pic_day = this.GetComponent<App>().carrot.get_tool().get_sprite_to_playerPrefs(this.item_calendar_temp.get_id_photo());
        if (sp_pic_day != null)
        {
            this.item_day_photo.img_icon.sprite = sp_pic_day;
            this.item_day_photo.img_icon.color = Color.white;
            item_day_photo.img_btn.sprite = this.img_btn_icon_menu[4].sprite;
            item_day_photo.obj_btn_edit.GetComponent<Image>().color = this.color_menu_sel;
            item_day_photo.obj_btn_del.SetActive(true);
        }
        else
        {
            this.item_day_photo.img_icon.sprite = this.img_btn_icon_menu[4].sprite;
            item_day_photo.obj_btn_edit.SetActive(false);
        }
    }

    private Item_info_day add_item_info()
    {
        GameObject item_emoji = Instantiate(this.item_day_info_prefab);
        item_emoji.transform.SetParent(this.area_list_day_info);
        item_emoji.transform.localPosition = new Vector3(item_emoji.transform.localPosition.x, item_emoji.transform.localPosition.y, item_emoji.transform.localPosition.z);
        item_emoji.transform.localScale = new Vector3(1f, 1f, 1f);
        return item_emoji.GetComponent<Item_info_day>();
    }

    public void delete_item_info(int index_func_del)
    {
        string s_body_msg = PlayerPrefs.GetString("del_tip", "Are you sure delete this selected item?");
        
        if (index_func_del == 1) this.msg_question = this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("emoji", "Emoji"), s_body_msg, this.act_del_emoji_yes, this.act_del_no);
        if (index_func_del == 2) this.msg_question = this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("event", "Event"), s_body_msg, this.act_del_notice_yes, this.act_del_no);
        if (index_func_del == 3) this.msg_question = this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("note", "Note"), s_body_msg, this.act_del_note_yes, this.act_del_no);
        if (index_func_del == 4) this.msg_question = this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("photo_souvenir", "Souvenir photos"), s_body_msg,this.act_del_photo_yes, this.act_del_no);
    }

    private void act_del_emoji_yes()
    {

        this.msg_question.close();
        PlayerPrefs.DeleteKey(this.item_calendar_temp.get_id());
        this.item_calendar_temp.set_index_emoji(-1);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.show(this.item_calendar_temp, 0);
        
    }

    private void act_del_notice_yes()
    {
        this.msg_question.close();
        PlayerPrefs.DeleteKey(this.item_calendar_temp.get_id_notice());
        this.item_calendar_temp.set_status_notice(false);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.show(this.item_calendar_temp, 0);
    }

    private void act_del_note_yes()
    {
        PlayerPrefs.DeleteKey(this.item_calendar_temp.get_id_note());
        this.item_calendar_temp.set_status_note(false);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.GetComponent<App>().note_manager.delete_note(this.item_calendar_temp.get_id_note());
        this.show(this.item_calendar_temp, 0);
        this.msg_question.close();
    }

    private void act_del_photo_yes()
    {
        PlayerPrefs.DeleteKey(this.item_calendar_temp.get_id_photo());
        this.item_calendar_temp.set_status_photo(false);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.show(this.item_calendar_temp, 0);
        this.msg_question.close();
    }

    private void act_del_no()
    {
        this.msg_question.close();
        return;
    }

    private void show_func_sel_emoji()
    {
        this.hide_all_panel_func();
        this.panel_function_day[1].SetActive(true);
        this.img_btn_menu[1].color = this.color_menu_sel;
        this.img_btn_icon_menu[1].color = Color.black;
        int index_sel_emoji = this.item_calendar_temp.get_index_emobij();
        this.GetComponent<App>().carrot.clear_contain(this.area_list_day_emoji);
        for (int i = 0; i < this.sp_emoji.Length; i++)
        {
            GameObject item_emoji = Instantiate(this.item_day_emoji_prefab);
            item_emoji.transform.SetParent(this.area_list_day_emoji);
            item_emoji.transform.localPosition = new Vector3(item_emoji.transform.localPosition.x, item_emoji.transform.localPosition.y, item_emoji.transform.localPosition.z);
            item_emoji.transform.localScale = new Vector3(1f, 1f, 1f);
            item_emoji.GetComponent<Item_emoji>().img_icon.sprite = this.sp_emoji[i];
            item_emoji.GetComponent<Item_emoji>().index_emoji = i;
            item_emoji.GetComponent<Item_emoji>().txt_tip.text = this.get_label_emoji(i);
            if (index_sel_emoji == i)
            {
                item_emoji.GetComponent<Image>().color = this.GetComponent<Calendar_happy>().color_calendar_bk_old;
                item_emoji.GetComponent<Item_emoji>().img_bk.color = this.GetComponent<Calendar_happy>().color_calendar_border_old;
            }
        }
    }

    private void show_func_notice()
    {
        this.hide_all_panel_func();
        this.panel_function_day[2].SetActive(true);
        this.img_btn_menu[2].color = this.color_menu_sel;
        this.img_btn_icon_menu[2].color = Color.black;
        this.sel_time_notice_at = PlayerPrefs.GetInt(this.item_calendar_temp.get_id_notice() + "_type_sel_time", 0);
        this.index_notice_update = PlayerPrefs.GetInt(this.item_calendar_temp.get_id_notice() + "_index", -1);
        this.dropdown_evenet_notice.value = this.sel_time_notice_at;
        this.dropdown_evenet_notice.RefreshShownValue();

        if (this.sel_time_notice_at == 5)
        {
            this.panel_event_timer.SetActive(true);
            DateTime datetime_timer_temp = new DateTime(long.Parse(PlayerPrefs.GetString("notice_" + this.index_notice_update + "_date")));
            this.dropdown_evenet_notice_hours.value = datetime_timer_temp.Hour;
            this.dropdown_evenet_notice_minute.value = datetime_timer_temp.Minute;
        }

        this.intp_evenet_notice.text = PlayerPrefs.GetString(this.item_calendar_temp.get_id_notice(), "");
    }

    private void show_func_note()
    {
        this.hide_all_panel_func();
        this.panel_function_day[3].SetActive(true);
        this.img_btn_menu[3].color = this.color_menu_sel;
        this.img_btn_icon_menu[3].color = Color.black;
        this.index_note_update = this.GetComponent<App>().note_manager.get_index_update_by_note_id(this.item_calendar_temp.get_id_note());
        Debug.Log(this.index_note_update);
        this.intp_calendar_note.text = PlayerPrefs.GetString(this.item_calendar_temp.get_id_note(), "");
    }

    private void show_func_photo()
    {
        if (this.item_calendar_temp.get_status_photo())
        {
            this.hide_all_panel_func();
            this.panel_function_day[4].SetActive(true);
            this.img_btn_menu[4].color = this.color_menu_sel;
            this.img_btn_icon_menu[4].color = Color.black;
            Sprite sp_pic_day= this.GetComponent<App>().carrot.get_tool().get_sprite_to_playerPrefs(this.item_calendar_temp.get_id_photo());
            this.img_photo_day.sprite = sp_pic_day;
            this.img_photo_day.rectTransform.sizeDelta = new Vector2(sp_pic_day.texture.width, sp_pic_day.texture.height);
        }
        else
        {
            this.show_camera();
        }
    }

    public void show_camera()
    {
        this.GetComponent<App>().carrot.camera_pro.show_camera(act_done_camera);
    }

    private void act_done_camera(Texture2D texture2d)
    {
        this.GetComponent<App>().carrot.get_tool().PlayerPrefs_Save_texture2D(this.item_calendar_temp.get_id_photo(),texture2d);
        this.item_calendar_temp.set_status_photo(true);
        this.show_func_info();
    }

    public void close()
    {
        this.panel_view_day.SetActive(false);
        this.GetComponent<App>().play_sound();
    }

    public void set_emoji_day(Item_emoji e)
    {
        this.add_emoji_day_data(this.item_calendar_temp.get_id(), e.index_emoji);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.GetComponent<App>().play_sound();
        this.close();
    }

    public void add_emoji_day_data(string id_day,int index_emoji)
    {
        PlayerPrefs.SetInt(id_day, index_emoji);
        this.GetComponent<App>().emoji_manager.add_emoji_in_day(index_emoji, id_day);
    }

    public void add_notice_day_data(string s_msg,string s_id_notice,DateTime date_notice)
    {
        int id_new_notice = this.GetComponent<App>().notice.add_notification(s_msg, s_id_notice, date_notice);
        PlayerPrefs.SetInt(s_id_notice + "_index", id_new_notice);
    }

    public void add_note_day_data(string s_msg, string s_id_notice)
    {
        PlayerPrefs.SetString(s_id_notice, s_msg);
        this.GetComponent<App>().note_manager.add_note(s_msg, s_id_notice);
    }

    public void btn_show_function_day(int index_func)
    {
        this.GetComponent<App>().play_sound(1);
        if (index_func == 0) this.show_func_info();
        if (index_func == 1) this.show_func_sel_emoji();
        if (index_func == 2) this.show_func_notice();
        if (index_func == 3) this.show_func_note();
        if (index_func == 4) this.show_func_photo();
        this.check_show_menu_btn();
    }

    public int get_index_max(List<int> a)
    {
        int[] b = new int[a.Count];
        int x = 1;
        b[0] = a[0];
        for (int i = 1; i < a.Count; i++)
        {
            int dem = 0;
            for (int j = 0; j < x; j++) { if (a[i] == b[j]) dem++; }
            if (dem == 0) { b[x] = a[i]; x++; }
        }

        int[] c = new int[x];
        for (int i = 0; i < x; i++) c[i] = 0;
        for (int i = 0; i < x; i++) { for (int j = 0; j < a.Count; j++) if (a[j] == b[i]) c[i]++; }

        int max = c[0], vtri = 0, y = 1;
        for (int i = 1; i < x; i++)
        {
            if (c[i] > max)
            {
                max = c[i];
                vtri = i;
                y = 1;
            }
            if (c[i] == max) y++;
        }


        if (y == 1)
        {
            return b[vtri];
        }
        else
        {
            int[] d = new int[y];
            int z = 0;
            for (int i = 0; i < x; i++)
                if (c[i] == max)
                {
                    d[z] = i;
                    z++;
                }

            return b[d[0]];
        }
    }

    public void btn_done_func_appointment_schedule()
    {
        if (intp_evenet_notice.text.Trim() == "")
        {
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("appointment_schedule", "Appointment schedule"), PlayerPrefs.GetString("event_error", "Event name cannot be empty!"), Carrot.Msg_Icon.Error);
            return;
        }

        DateTime datetime_set = this.item_calendar_temp.get_datetime();
        int index_type_sel_time = this.dropdown_evenet_notice.value;

        if (index_type_sel_time == 5)
        {
            datetime_set = datetime_set.AddHours(this.dropdown_evenet_notice_hours.value);
            datetime_set =datetime_set.AddMinutes(this.dropdown_evenet_notice_minute.value);
        }
        else
            datetime_set = datetime_set.AddHours(this.data_time_notice[index_type_sel_time]);


        if (this.index_notice_update == -1)
        {
            this.add_notice_day_data(this.intp_evenet_notice.text, this.item_calendar_temp.get_id_notice(), datetime_set);
        }
        else
        {
            this.id_notice_sys = PlayerPrefs.GetInt("notice_" + this.index_notice_update + "_id_sys");
            this.GetComponent<App>().notice.update_notification(this.id_notice_sys, this.index_notice_update, this.intp_evenet_notice.text, datetime_set);
        }
        PlayerPrefs.SetString(this.item_calendar_temp.get_id_notice(), this.intp_evenet_notice.text);
        PlayerPrefs.SetInt(this.item_calendar_temp.get_id_notice() + "_type_sel_time", index_type_sel_time);
        this.item_calendar_temp.set_status_notice(true);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.show(this.item_calendar_temp, 0);
    }

    public void btn_done_func_note()
    {
        if (intp_calendar_note.text.Trim() == "")
        {
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("note", "Note"), PlayerPrefs.GetString("note_error", "Notes cannot be left blank!"), Carrot.Msg_Icon.Error);
            return;
        }

        if (this.index_note_update == -1)
        {
            this.add_note_day_data(this.intp_calendar_note.text, this.item_calendar_temp.get_id_note());
        }
        else
        {
            PlayerPrefs.SetString(this.item_calendar_temp.get_id_note(), this.intp_calendar_note.text);
            this.GetComponent<App>().note_manager.update_note(this.index_note_update, this.intp_calendar_note.text);
        }

        this.item_calendar_temp.set_status_note(true);
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.show(this.item_calendar_temp,0);
    }

    public void check_val_dropdown_timer()
    {
        if (this.dropdown_evenet_notice.value == 5)
        {
            if (this.index_notice_update == -1)
            {
                DateTime t_cur = DateTime.Now;
                this.dropdown_evenet_notice_hours.value = t_cur.Hour;
                this.dropdown_evenet_notice_minute.value = t_cur.Minute;
                this.dropdown_evenet_notice_hours.RefreshShownValue();
                this.dropdown_evenet_notice_minute.RefreshShownValue();
            }
            this.panel_event_timer.SetActive(true);
        } 
        else
            this.panel_event_timer.SetActive(false);
    }

    public string get_label_emoji(int index)
    {
        return PlayerPrefs.GetString(this.txt_emoji_key[index], this.txt_emoji[index]);
    }
}