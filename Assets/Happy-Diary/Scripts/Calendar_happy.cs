using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Day_Status {day_new,day_cur,day_old};

public class data_status_emoji
{
    public int index = -1;
    public int count = 0;
}

public class data_notice
{
    public string id;
    public int id_notice_sys;
    public string title;
    public DateTime datetime;
    public int index_notice;
    public Calendar_item calendar_item;
}

public class data_note
{
    public string title;
    public Calendar_item calendar_item;
}

public class data_photo
{
    public Sprite pic;
    public Calendar_item calendar_item;
}

public class Calendar_happy : MonoBehaviour
{
    public Transform area_body_calendar;
    public Transform area_body_calendar_week;
    public Transform[] area_body_item_week;
    public GameObject item_calendar_prefab;
    public GameObject item_calendar_blank_prefab;
    public GameObject item_month_prefab;
    public GameObject item_year_prefab;
    public GameObject item_week_name_prefab;
    public GameObject item_status_day_in_month_prefab;
    public Sprite[] sp_icon_status;
    public Image img_icon_status;
    public GameObject obj_btn_switch_calendar;
    public GameObject obj_btn_today_calendar;
    public GameObject obj_btn_list_notice;
    public GameObject obj_btn_list_note;
    public GameObject obj_btn_list_photo;
    private DateTime datetime_cur = DateTime.Now;
    private DateTime datetime_view = DateTime.Now;
    public Text txt_title_month_year;
    private int type_show = 0;
    private IList<RectTransform> list_tr_item;
    private float width_item_calendar_year_month = 150f;
    private float width_clandar_week = 68f;
    private float height_item_clandar_week = 80f;
    private Carrot_Box box_status;

    private DayOfWeek[] col_week_name = { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday,DayOfWeek.Wednesday,DayOfWeek.Thursday,DayOfWeek.Friday,DayOfWeek.Saturday};

    [Header("Color Border item")]
    public Color32 color_calendar_border_nomal;
    public Color32 color_calendar_border_old;
    public Color32 color_calendar_border_cur;

    [Header("Color Background item")]
    public Color32 color_calendar_bk_nomal;
    public Color32 color_calendar_bk_old;
    public Color32 color_calendar_bk_cur;

    [Header("Color Text item")]
    public Color32 color_calendar_text_nomal;
    public Color32 color_calendar_text_old;
    public Color32 color_calendar_text_cur;

    [Header("Color Week")]
    public Color32 color_calendar_sun;
    public Color32 color_calendar_sun_txt;
    public Color32 color_calendar_day_0;
    public Color32 color_calendar_day_1;

    [Header("Notice Calendar")]
    public Sprite sp_icon_list_notice;
    public GameObject item_notice_prefab;
    public GameObject item_notice_menu_prefab;
    private List<data_notice> list_notice;
    public Color32 color_notice_menu_sel;
    public Color32 color_notice_menu_nomal;

    [Header("Note Calendar")]
    public Sprite sp_icon_list_note;
    public GameObject item_note_prefab;
    private List<data_note> list_note;

    [Header("Photo Calendar")]
    public GameObject item_photo_prefab;
    private List<data_photo> list_photo;

    public void show_calendar_all_day_in_month()
    {
        this.show_calendar_by_month_year(this.datetime_cur.Year, this.datetime_cur.Month);
    }

    public void show_calendar_by_month_year(int year_show, int month_show, int day_point=-1)
    {
        this.GetComponent<App>().carrot.clear_contain(this.area_body_calendar);

        int num_day_in_month = DateTime.DaysInMonth(year_show, month_show);
        this.datetime_view = new DateTime(year_show, month_show,this.datetime_cur.Day);
        this.txt_title_month_year.text =  this.datetime_view.Month + " ("+ this.datetime_view.ToString("MMMM")+ ")"+ " - " + this.datetime_view.Year;

        this.list_notice = new List<data_notice>();
        this.list_note = new List<data_note>();
        this.list_photo = new List<data_photo>();
        this.list_tr_item = new List<RectTransform>();

        bool is_today = false;

        for (int i = 0; i < this.area_body_item_week.Length; i++)
        {
            this.GetComponent<App>().carrot.clear_contain(this.area_body_item_week[i]);
            GameObject item_week_name = Instantiate(this.item_week_name_prefab);
            item_week_name.transform.SetParent(this.area_body_item_week[i]);
            item_week_name.transform.localPosition = new Vector3(item_week_name.transform.localPosition.x, item_week_name.transform.localPosition.y, item_week_name.transform.localPosition.z);
            item_week_name.transform.localScale = new Vector3(1f, 1f, 1f);
            item_week_name.GetComponent<Item_week_label>().txt.text = this.col_week_name[i].ToString().Substring(0,3);
            if (i == 0) item_week_name.GetComponent<Image>().color = this.color_calendar_sun;
            else
            {
                if(i%2==0)
                    item_week_name.GetComponent<Image>().color = this.color_calendar_day_0;
                else
                    item_week_name.GetComponent<Image>().color = this.color_calendar_day_1;
            }
        }

        DateTime date_first = new DateTime(this.datetime_view.Year, this.datetime_view.Month, 1);
        for (int i = 0; i < this.get_day_week(date_first.DayOfWeek); i++)
        {
            GameObject item_blank = Instantiate(this.item_calendar_blank_prefab);
            item_blank.transform.SetParent(this.area_body_item_week[i]);
            item_blank.transform.localPosition = new Vector3(item_blank.transform.localPosition.x, item_blank.transform.localPosition.y, item_blank.transform.localPosition.z);
            item_blank.transform.localScale = new Vector3(1f, 1f, 1f);
            this.list_tr_item.Add(item_blank.GetComponent<RectTransform>());
        }


        for (int i = 1; i <= num_day_in_month; i++)
        {
            DateTime datetime_item = new DateTime(this.datetime_view.Year, this.datetime_view.Month, i);
            int day_of_week = this.get_day_week(datetime_item.DayOfWeek);

            GameObject item_calendar = Instantiate(this.item_calendar_prefab);
            item_calendar.transform.SetParent(this.area_body_item_week[day_of_week]);
            item_calendar.transform.localPosition = new Vector3(item_calendar.transform.localPosition.x, item_calendar.transform.localPosition.y, item_calendar.transform.localPosition.z);
            item_calendar.transform.localScale = new Vector3(1f, 1f, 1f);
            item_calendar.name = "item_calendar";
            item_calendar.GetComponent<Calendar_item>().txt.text = i.ToString();
            item_calendar.GetComponent<Calendar_item>().txt_small.text = i.ToString();

            this.list_tr_item.Add(item_calendar.GetComponent<RectTransform>());

            item_calendar.GetComponent<Calendar_item>().load_item(datetime_item);

            if (datetime_item.Month == this.datetime_cur.Month && datetime_item.Year == this.datetime_cur.Year && datetime_item.Day == this.datetime_cur.Day)
            {
                item_calendar.GetComponent<Image>().color = this.color_calendar_border_cur;
                item_calendar.GetComponent<Calendar_item>().img_bk.color = this.color_calendar_bk_cur;
                item_calendar.GetComponent<Calendar_item>().txt.color = this.color_calendar_text_cur;
                item_calendar.GetComponent<Calendar_item>().set_status(Day_Status.day_cur);
                is_today = true;
            }
            else if (datetime_item < this.datetime_cur)
            {
                item_calendar.GetComponent<Image>().color = this.color_calendar_border_old;
                item_calendar.GetComponent<Calendar_item>().img_bk.color = this.color_calendar_bk_old;
                item_calendar.GetComponent<Calendar_item>().txt.color = this.color_calendar_text_old;
                item_calendar.GetComponent<Calendar_item>().set_status(Day_Status.day_old);
            }
            else
            {
                item_calendar.GetComponent<Image>().color = this.color_calendar_border_nomal;
                item_calendar.GetComponent<Calendar_item>().img_bk.color = this.color_calendar_bk_nomal;
                item_calendar.GetComponent<Calendar_item>().txt.color = this.color_calendar_text_nomal;
                item_calendar.GetComponent<Calendar_item>().set_status(Day_Status.day_new);
            }
            int index_emoji_calendar = item_calendar.GetComponent<Calendar_item>().get_index_emobij();
            if (index_emoji_calendar != -1) item_calendar.GetComponent<Calendar_item>().img_emoji.sprite = this.GetComponent<View_day>().sp_emoji[index_emoji_calendar];
            if (item_calendar.GetComponent<Calendar_item>().get_status_notice())
            {
                string id_notice = item_calendar.GetComponent<Calendar_item>().get_id_notice();
                data_notice item_notice = new data_notice();
                item_notice.title = PlayerPrefs.GetString(id_notice);
                item_notice.index_notice = PlayerPrefs.GetInt(id_notice+ "_index",-1);
                item_notice.datetime = new DateTime(long.Parse(PlayerPrefs.GetString("notice_" + item_notice.index_notice + "_date")));
                item_notice.id = id_notice;
                item_notice.calendar_item = item_calendar.GetComponent<Calendar_item>();
                this.list_notice.Add(item_notice);
            }

            if (item_calendar.GetComponent<Calendar_item>().get_status_note())
            {
                data_note item_n = new data_note();
                item_n.title= PlayerPrefs.GetString(item_calendar.GetComponent<Calendar_item>().get_id_note());
                item_n.calendar_item= item_calendar.GetComponent<Calendar_item>();
                this.list_note.Add(item_n);
            }

            if (item_calendar.GetComponent<Calendar_item>().get_status_photo())
            {
                data_photo item_p = new data_photo();
                item_p.pic = this.GetComponent<App>().carrot.get_tool().get_sprite_to_playerPrefs(item_calendar.GetComponent<Calendar_item>().get_id_photo());
                item_p.calendar_item = item_calendar.GetComponent<Calendar_item>();
                this.list_photo.Add(item_p);
            }

            if (day_point != -1)
            {
                if (i == day_point) item_calendar.GetComponent<Calendar_item>().play_point();
            }
        }

        this.area_body_calendar.GetComponent<GridLayoutGroup>().cellSize = new Vector2(80f, 80f);
        this.area_body_calendar.gameObject.SetActive(false);
        this.area_body_calendar_week.gameObject.SetActive(true);
        this.area_body_calendar_week.GetComponent<GridLayoutGroup>().cellSize = new Vector2(this.width_clandar_week,600f);
        this.type_show = 0;
        this.img_icon_status.sprite = this.sp_icon_status[0];
        this.obj_btn_switch_calendar.SetActive(true);

        if (this.list_notice.Count>0)
            this.obj_btn_list_notice.SetActive(true);
        else
            this.obj_btn_list_notice.SetActive(false);

        if (this.list_note.Count > 0)
            this.obj_btn_list_note.SetActive(true);
        else
            this.obj_btn_list_note.SetActive(false);

        if (this.list_photo.Count > 0)
            this.obj_btn_list_photo.SetActive(true);
        else
            this.obj_btn_list_photo.SetActive(false);

        if (is_today)
            this.obj_btn_today_calendar.SetActive(false);
        else
            this.obj_btn_today_calendar.SetActive(true);

        for (int i = 0; i < this.list_tr_item.Count; i++) this.list_tr_item[i].sizeDelta = new Vector2(this.list_tr_item[i].sizeDelta.x, this.height_item_clandar_week);


        foreach (Transform tr in this.area_body_item_week[0])
        {
            if (tr.gameObject.GetComponent<Calendar_item>()!=null)
            {
                tr.gameObject.GetComponent<Calendar_item>().txt.color = this.color_calendar_sun_txt;
                tr.gameObject.GetComponent<Calendar_item>().txt_small.color = this.color_calendar_sun_txt;
            }
        }
    }

    public void freshen_calander_day_in_month()
    {
        this.show_calendar_by_month_year(this.datetime_view.Year, this.datetime_view.Month);
    }

    public void show_calendar_all_month_in_year()
    {
        this.GetComponent<App>().carrot.clear_contain(this.area_body_calendar);

        this.txt_title_month_year.text = this.datetime_view.Year.ToString();

        for (int i = 1; i <= 12; i++)
        {
            GameObject item_m = Instantiate(this.item_month_prefab);
            item_m.transform.SetParent(this.area_body_calendar);
            item_m.transform.localPosition = new Vector3(item_m.transform.localPosition.x, item_m.transform.localPosition.y, item_m.transform.localPosition.z);
            item_m.transform.localScale = new Vector3(1f, 1f, 1f);
            item_m.name = "item_month";
            

            DateTime datetime_item = new DateTime(this.datetime_view.Year, i,1);
            item_m.GetComponent<Month_item>().load_item(datetime_item);
            item_m.GetComponent<Month_item>().txt.text = datetime_item.ToString("MMM");
            item_m.GetComponent<Month_item>().txt_small.text = datetime_item.ToString("MMM");

            if (datetime_item.Month == this.datetime_cur.Month && datetime_item.Year == this.datetime_cur.Year)
            {
                item_m.GetComponent<Image>().color = this.color_calendar_border_cur;
                item_m.GetComponent<Month_item>().img_bk.color = this.color_calendar_bk_cur;
                item_m.GetComponent<Month_item>().txt.color = this.color_calendar_text_cur;
                item_m.GetComponent<Month_item>().txt_small.color = this.color_calendar_text_cur;
                item_m.GetComponent<Month_item>().img_menu_more.color = this.color_calendar_bk_cur;
            }
            else if (datetime_item < this.datetime_cur)
            {
                item_m.GetComponent<Image>().color = this.color_calendar_border_old;
                item_m.GetComponent<Month_item>().img_bk.color = this.color_calendar_bk_old;
                item_m.GetComponent<Month_item>().txt.color = this.color_calendar_text_old;
                item_m.GetComponent<Month_item>().txt_small.color = this.color_calendar_text_old;
                item_m.GetComponent<Month_item>().img_menu_more.color = this.color_calendar_bk_old;
            }
            else
            {
                item_m.GetComponent<Image>().color = this.color_calendar_border_nomal;
                item_m.GetComponent<Month_item>().img_bk.color = this.color_calendar_bk_nomal;
                item_m.GetComponent<Month_item>().txt.color = this.color_calendar_text_nomal;
            }
        }

        this.area_body_calendar.GetComponent<GridLayoutGroup>().cellSize = new Vector2(this.width_item_calendar_year_month, 150f);
        this.area_body_calendar.gameObject.SetActive(true);
        this.area_body_calendar_week.gameObject.SetActive(false);
        this.type_show = 1;
        this.img_icon_status.sprite = this.sp_icon_status[1];
        this.obj_btn_switch_calendar.SetActive(true);
        this.obj_btn_list_notice.SetActive(false);
        this.obj_btn_today_calendar.SetActive(true);
    }

    public void show_calendar_all_year()
    {
        this.GetComponent<App>().carrot.clear_contain(this.area_body_calendar);
        
        int start_year = (this.datetime_view.Year - 6);
        int end_year = (this.datetime_view.Year + 5);

        this.txt_title_month_year.text = start_year + " - " + end_year;

        for (int i = start_year; i <= end_year; i++)
        {
            GameObject item_y = Instantiate(this.item_year_prefab);
            item_y.transform.SetParent(this.area_body_calendar);
            item_y.transform.localPosition = new Vector3(item_y.transform.localPosition.x, item_y.transform.localPosition.y, item_y.transform.localPosition.z);
            item_y.transform.localScale = new Vector3(1f, 1f, 1f);
            item_y.name = "item_year";


            DateTime datetime_item = new DateTime(i, 1, 1);
            item_y.GetComponent<Year_item>().load_item(datetime_item);
            item_y.GetComponent<Year_item>().txt.text = i.ToString();
            item_y.GetComponent<Year_item>().txt_small.text = i.ToString();

            if (datetime_item.Year == this.datetime_cur.Year)
            {
                item_y.GetComponent<Image>().color = this.color_calendar_border_cur;
                item_y.GetComponent<Year_item>().img_bk.color = this.color_calendar_bk_cur;
                item_y.GetComponent<Year_item>().txt.color = this.color_calendar_text_cur;
            }
            else if (datetime_item < this.datetime_cur)
            {
                item_y.GetComponent<Image>().color = this.color_calendar_border_old;
                item_y.GetComponent<Year_item>().img_bk.color = this.color_calendar_bk_old;
                item_y.GetComponent<Year_item>().txt.color = this.color_calendar_text_old;
            }
            else
            {
                item_y.GetComponent<Image>().color = this.color_calendar_border_nomal;
                item_y.GetComponent<Year_item>().img_bk.color = this.color_calendar_bk_nomal;
                item_y.GetComponent<Year_item>().txt.color = this.color_calendar_text_nomal;
            }

        }

        this.area_body_calendar.GetComponent<GridLayoutGroup>().cellSize = new Vector2(this.width_item_calendar_year_month, 150f);
        this.area_body_calendar.gameObject.SetActive(true);
        this.area_body_calendar_week.gameObject.SetActive(false);
        this.type_show=2;
        this.obj_btn_switch_calendar.SetActive(false);
        this.obj_btn_list_notice.SetActive(false);
        this.obj_btn_today_calendar.SetActive(true);
    }

    public void next_month()
    {
        if (this.type_show == 0)
        {
            this.datetime_view = datetime_view.AddMonths(1);
            this.show_calendar_by_month_year(this.datetime_view.Year, this.datetime_view.Month);
        }

        if (this.type_show == 1)
        {
            this.datetime_view = datetime_view.AddMonths(12);
            this.show_calendar_all_month_in_year();
        }

        if (this.type_show == 2)
        {
            this.datetime_view = new DateTime(this.datetime_view.Year + 5, this.datetime_view.Month, this.datetime_view.Day);
            this.show_calendar_all_year();
        }

        this.GetComponent<App>().play_sound(1);
        this.GetComponent<App>().ads.show_ads_Interstitial();
    }

    public void prev_month()
    {
        if (this.type_show == 0)
        {
            this.datetime_view = datetime_view.AddMonths(-1);
            this.show_calendar_by_month_year(this.datetime_view.Year, this.datetime_view.Month);
        }

        if (this.type_show == 1)
        {
            this.datetime_view = datetime_view.AddMonths(-12);
            this.show_calendar_all_month_in_year();
        }

        if (this.type_show == 2)
        {
            this.datetime_view = new DateTime(this.datetime_view.Year - 5, this.datetime_view.Month, this.datetime_view.Day);
            this.show_calendar_all_year();
        }

        this.GetComponent<App>().play_sound(1);
        this.GetComponent<App>().ads.show_ads_Interstitial();
    }

    public void cur_month()
    {
        this.datetime_view = this.datetime_cur;
        this.show_calendar_by_month_year(this.datetime_view.Year, this.datetime_view.Month);
        this.GetComponent<App>().play_sound();
    }

    public void btn_switch_calendar()
    {
        if (this.type_show == 0)this.show_calendar_all_month_in_year();
        else if (this.type_show == 1) this.show_calendar_all_year();
        this.GetComponent<App>().play_sound(1);
        this.GetComponent<App>().ads.show_ads_Interstitial();
    }

    public void set_datetime_view(DateTime datetime_new_view)
    {
        this.datetime_view = datetime_new_view;
    }

    public void show_count_statu_by_month_year(Month_item m)
    {
        this.box_status=this.GetComponent<App>().carrot.Create_Box(m.datetime.ToString("MMM")+" - "+m.datetime.ToString("yyyy"), this.sp_icon_status[2]);

        int num_day = DateTime.DaysInMonth(m.datetime.Year, m.datetime.Month);
        List<int> list_emoji = new List<int>();

        for(int i = 0; i <= num_day; i++)
        {
            int id_emoji = PlayerPrefs.GetInt("day_" + i + "_" + m.datetime.ToString("MM") + "_" + m.datetime.ToString("yyyy"), -1);
            if (id_emoji != -1)
            {

                list_emoji.Add(id_emoji);
            }
        }

        int[] list_emoji_only = new int[list_emoji.Count];
        int count_list_only = 1;
        list_emoji_only[0] = list_emoji[0];
        for (int i = 0; i < list_emoji.Count; i++)
        {
            int count_item = 0;
            for (int j = 0; j < count_list_only; j++) if (list_emoji[i] == list_emoji_only[j]) count_item++;

            if (count_item == 0)
            {
                list_emoji_only[count_list_only] = list_emoji[i];
                count_list_only++;
            }

        }

        int[] list_emoji_count= new int[count_list_only];
        for (int i = 0; i < count_list_only; i++) list_emoji_count[i] = 0;

        for (int i = 0; i < count_list_only; i++)
        {
            for (int j = 0; j < list_emoji.Count; j++)
            {
                if (list_emoji[j] == list_emoji_only[i]) list_emoji_count[i]++;
            }
        }

        List<data_status_emoji> list_data_emoji = new List<data_status_emoji>();

        for (int i = 0; i < count_list_only; i++)
        {
            data_status_emoji emoji_status_item = new data_status_emoji();
            emoji_status_item.index = list_emoji_only[i];
            emoji_status_item.count = list_emoji_count[i];
            list_data_emoji.Add(emoji_status_item);
        }

        list_data_emoji.Sort(delegate (data_status_emoji x, data_status_emoji y)
        {
            if (x.count < y.count) return 0;
            else
                return -1;
        });

        for (int i = 0; i < list_data_emoji.Count; i++)
        {
            GameObject item_status_day = box_status.add_item(this.item_status_day_in_month_prefab);
            item_status_day.GetComponent<Item_status_day>().icon.sprite = this.GetComponent<App>().view_day.sp_emoji[list_data_emoji[i].index];
            item_status_day.GetComponent<Item_status_day>().txt_count_day.text = list_data_emoji[i].count.ToString()+" "+PlayerPrefs.GetString("day","Day");
            item_status_day.GetComponent<Item_status_day>().txt_status_day.text = this.GetComponent<App>().view_day.get_label_emoji(list_data_emoji[i].index);
        }
        box_status.update_gamepad_cosonle_control();
    }

    private void add_menu_notice_prefab(int index_menu_sel)
    {
        GameObject item_notice_menu = box_status.add_item(this.item_notice_menu_prefab);
        item_notice_menu.GetComponent<Item_notice>().set_sel_menu(index_menu_sel);
        item_notice_menu.GetComponent<Item_notice>().txt_title.text = PlayerPrefs.GetString("list_notice_month", "List of events scheduled for the month");
        item_notice_menu.GetComponent<Item_notice>().txt_tip.text = PlayerPrefs.GetString("list_notice_all", "List of all events");
    }

    public void btn_show_list_notice()
    {
        if (this.box_status != null) this.box_status.close();
        this.box_status=this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("list_notice_month", "List of events scheduled for the month"), this.sp_icon_list_notice);
        this.add_menu_notice_prefab(0);

        for (int i = 0; i < this.list_notice.Count; i++)
        {
            GameObject item_notice = box_status.add_item(this.item_notice_prefab);
            item_notice.transform.localPosition = new Vector3(item_notice.transform.localPosition.x, item_notice.transform.localPosition.y, item_notice.transform.localPosition.z);
            item_notice.transform.localScale = new Vector3(1f, 1f, 1f);
            item_notice.GetComponent<Item_notice>().txt_title.text = this.list_notice[i].title;
            item_notice.GetComponent<Item_notice>().txt_tip.text =  this.list_notice[i].datetime.ToString();
            item_notice.GetComponent<Item_notice>().datetime = this.list_notice[i].datetime;
            item_notice.GetComponent<Item_notice>().id = this.list_notice[i].id;
            item_notice.GetComponent<Item_notice>().index_notice = this.list_notice[i].index_notice;
            item_notice.GetComponent<Item_notice>().calendar_day_item= this.list_notice[i].calendar_item;
            if (this.list_notice[i].datetime < this.datetime_cur) item_notice.GetComponent<Item_notice>().set_old_notice();
        }
        this.GetComponent<App>().play_sound();
    }

    public void btn_show_list_all_notice()
    {
        if(this.box_status!=null) this.box_status.close();
        this.box_status = this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("list_notice_all", "List of all events"), this.sp_icon_list_notice);

        if (this.type_show == 0) this.add_menu_notice_prefab(1);

        List<data_notice> list_n = this.GetComponent<App>().notice.get_list_notice();

        for (int i = 0; i < list_n.Count; i++)
        {
            GameObject item_notice = box_status.add_item(this.item_notice_prefab);
            item_notice.transform.localPosition = new Vector3(item_notice.transform.localPosition.x, item_notice.transform.localPosition.y, item_notice.transform.localPosition.z);
            item_notice.transform.localScale = new Vector3(1f, 1f, 1f);
            item_notice.GetComponent<Item_notice>().txt_title.text = list_n[i].title;
            item_notice.GetComponent<Item_notice>().txt_tip.text = list_n[i].datetime.ToString();
            item_notice.GetComponent<Item_notice>().id = list_n[i].id;
            item_notice.GetComponent<Item_notice>().datetime = list_n[i].datetime;
            item_notice.GetComponent<Item_notice>().index_notice = list_n[i].index_notice;
            item_notice.GetComponent<Item_notice>().obj_btn_notice_edit.SetActive(false);
            if (list_n[i].datetime < this.datetime_cur) item_notice.GetComponent<Item_notice>().set_old_notice();
        }

        this.GetComponent<App>().play_sound();
    }

    public void btn_show_list_note()
    {
        Carrot_Box box_notice=this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("list_note_month","List of notes for the month"), this.sp_icon_list_notice);
        for (int i = 0; i < this.list_note.Count; i++)
        {
            GameObject item_notice = box_notice.add_item(this.item_note_prefab);
            item_notice.transform.localPosition = new Vector3(item_notice.transform.localPosition.x, item_notice.transform.localPosition.y, item_notice.transform.localPosition.z);
            item_notice.transform.localScale = new Vector3(1f, 1f, 1f);
            item_notice.GetComponent<Item_note>().txt_title.text = this.list_note[i].title;
            item_notice.GetComponent<Item_note>().txt_tip.text = this.list_note[i].calendar_item.get_datetime().ToString("dd-MM-yyyy");
            item_notice.GetComponent<Item_note>().calendar_day_item = this.list_note[i].calendar_item;
        }
        this.GetComponent<App>().play_sound();
    }

    public void btn_show_list_photo()
    {
        Carrot_Box box_photo=this.GetComponent<App>().carrot.show_grid();
        box_photo.set_title("List Photo");
        for (int i = 0; i < this.list_photo.Count; i++)
        {
            GameObject item_photo = box_photo.add_item(this.item_photo_prefab);
            item_photo.transform.localPosition = new Vector3(item_photo.transform.localPosition.x, item_photo.transform.localPosition.y, item_photo.transform.localPosition.z);
            item_photo.transform.localScale = new Vector3(1f, 1f, 1f);
            item_photo.GetComponent<Item_photo>().txt.text=this.list_photo[i].calendar_item.get_datetime().ToString("dd-MM");
            item_photo.GetComponent<Item_photo>().calendar_item = this.list_photo[i].calendar_item;
            item_photo.GetComponent<Item_photo>().img.sprite = this.list_photo[i].pic;
        }
        box_photo.update_gamepad_cosonle_control();
        this.GetComponent<App>().play_sound();
    }

    public void set_width_item_calendar_year_month(float w)
    {
        this.width_item_calendar_year_month = w;
        if (this.type_show != 0) this.area_body_calendar.GetComponent<GridLayoutGroup>().cellSize = new Vector2(this.width_item_calendar_year_month, 150f);
    }

    public void set_size_calendar_week(float w)
    {
        this.width_clandar_week = w;
        this.area_body_calendar_week.GetComponent<GridLayoutGroup>().cellSize = new Vector2(this.width_clandar_week,600f);
    }

    public void set_size_item_calendar_week(float h)
    {
        this.height_item_clandar_week = h;
        for (int i = 0; i < this.list_tr_item.Count; i++)
        {
            this.list_tr_item[i].sizeDelta = new Vector2(this.list_tr_item[i].sizeDelta.x, this.height_item_clandar_week);
        }
    }

    private int get_day_week(DayOfWeek dayOfWeek)
    {
        if (dayOfWeek == DayOfWeek.Sunday) return 0;
        if (dayOfWeek == DayOfWeek.Monday) return 1;
        if (dayOfWeek == DayOfWeek.Tuesday) return 2;
        if (dayOfWeek == DayOfWeek.Wednesday) return 3;
        if (dayOfWeek == DayOfWeek.Thursday) return 4;
        if (dayOfWeek == DayOfWeek.Friday) return 5;
        if (dayOfWeek == DayOfWeek.Saturday) return 6;
        return -1;
    }

}
