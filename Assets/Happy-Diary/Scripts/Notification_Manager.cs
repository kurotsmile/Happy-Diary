using System.Collections.Generic;
using UnityEngine;
using System;
using Carrot;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class Notification_Manager : MonoBehaviour
{
    public Sprite sp_remind_on;
    public Sprite sp_remind_off;
    private bool is_remind_app = true;
    private int id_notice_remind_app = -1;

    public GameObject obj_btn_list_all_notice;
    private int length_notice = 0;
    private Calendar_item calendar_item_notice = null;

    public void load()
    {

#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == OS.Android)
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = "alert_event",
                Name = "Happy Diary Calendar",
                Importance = Importance.Default,
                Description = "Announcement of events",
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
#endif
        if (PlayerPrefs.GetInt("is_remind_app", 0) == 0)
            this.is_remind_app = true;
        else
            this.is_remind_app = false;

        this.id_notice_remind_app = PlayerPrefs.GetInt("id_notice_remind_app",-1);

        if (this.is_remind_app&&this.id_notice_remind_app==-1) this.regiter_notice_remind_app();

        this.length_notice = PlayerPrefs.GetInt("length_notice",0);
        this.check_show_btn_notice();
        this.check_notification();
    }

    public void check_notification()
    {
#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == Carrot.OS.Android)
        {
            AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler = delegate (AndroidNotificationIntentData data)
            {
                if (data.Id != this.id_notice_remind_app)
                {
                    this.received_notice(data.Id, data.Notification.Text);
                }
            };

            AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;

            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntentData != null)
            {
                this.received_notice(notificationIntentData.Id, notificationIntentData.Notification.Text);
            }
        }
#endif
    }

    private void received_notice(int id_notice,string s_msg)
    {
        this.calendar_item_notice = this.get_calendar_item_by_id_notice_sys(id_notice);
        Carrot_Window_Msg msg=this.GetComponent<App>().carrot.Show_msg(this.GetComponent<App>().carrot.L("event","Event"),s_msg);
        msg.add_btn_msg(this.GetComponent<App>().carrot.L("appointment_schedule", "Appointment schedule"), act_notice_view);
        msg.add_btn_msg(this.GetComponent<App>().carrot.L("cancel","Cancel"), act_notice_close);
        msg.update_btns_gamepad_console();
    }

    private void act_notice_view()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        this.GetComponent<App>().carrot.close();
        this.GetComponent<App>().view_day.show(this.calendar_item_notice,0);
#endif
    }

    private void act_notice_close()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        this.GetComponent<App>().carrot.close();
        this.GetComponent<App>().play_sound();
#endif
    }

    public int add_notification(string s_msg,string id_notice,DateTime datetime_set)
    {
#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == OS.Android)
        {
            PlayerPrefs.SetString("notice_" + this.length_notice + "_id",id_notice);
            PlayerPrefs.SetString("notice_" + this.length_notice + "_msg", s_msg);
            PlayerPrefs.SetString("notice_" + this.length_notice + "_date", datetime_set.ToBinary().ToString());

            var notification = new AndroidNotification();
            notification.Title = "Happy Diary";
            notification.Text = s_msg;
            notification.FireTime = datetime_set;
            int id_notice_sys = AndroidNotificationCenter.SendNotification(notification, "alert_event");
            PlayerPrefs.SetInt("notice_" + this.length_notice + "_id_sys", id_notice_sys);

            this.length_notice = this.length_notice + 1;
            PlayerPrefs.SetInt("length_notice",this.length_notice);
            this.check_show_btn_notice();
            return this.length_notice - 1;
        }
#endif
        return -1;
    }

    public void update_notification(int id_notice_sys,int index_notice,string s_msg, DateTime datetime_set)
    {
#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == OS.Android)
        {
            AndroidNotificationCenter.CancelNotification(id_notice_sys);
            PlayerPrefs.SetString("notice_" + index_notice + "_msg", s_msg);
            PlayerPrefs.SetString("notice_" + index_notice + "_date", datetime_set.ToBinary().ToString());

            var notification = new AndroidNotification();
            notification.Title = "Happy Diary";
            notification.Text = s_msg;
            notification.FireTime = datetime_set;
            int id_notice_sys_new=AndroidNotificationCenter.SendNotification(notification, "alert_event");
            PlayerPrefs.SetInt("notice_" + this.length_notice + "_id_sys", id_notice_sys_new);
        }
#endif
    }

    public List<data_notice> get_list_notice()
    {
        List<data_notice> lis_notice = new List<data_notice>();

        for(int i = 0; i < this.length_notice; i++)
        {
            string id_n = PlayerPrefs.GetString("notice_" + i + "_id","");
            if (id_n != "")
            {
                data_notice n = new data_notice();
                n.id = PlayerPrefs.GetString("notice_" + i + "_id");
                n.id_notice_sys= PlayerPrefs.GetInt("notice_" + i + "_id_sys");
                n.title = PlayerPrefs.GetString("notice_" + i + "_msg");
                n.datetime=new DateTime(long.Parse(PlayerPrefs.GetString("notice_" + i + "_date")));
                n.index_notice = i;
                lis_notice.Add(n);
            }
        }

        if (lis_notice.Count == 0)
        {
            this.length_notice = 0;
            PlayerPrefs.SetInt("length_notice", this.length_notice);
            this.check_show_btn_notice();
        }
        return lis_notice;
    }

    public void delete_buy_index(int index_notice)
    {
        
#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == Carrot.OS.Android)
        {
            int id_notice_sys = PlayerPrefs.GetInt("notice_" + index_notice + "_id_sys");
            AndroidNotificationCenter.CancelNotification(id_notice_sys);
        }
#endif
        PlayerPrefs.DeleteKey("notice_" + index_notice + "_id");
        PlayerPrefs.DeleteKey("notice_" + index_notice + "_msg");
        PlayerPrefs.DeleteKey("notice_" + index_notice + "_date");
        PlayerPrefs.DeleteKey("notice_" + index_notice + "_id_sys");
    }

    private void check_show_btn_notice()
    {
        if (this.length_notice > 0)
            this.obj_btn_list_all_notice.SetActive(true);
        else
            this.obj_btn_list_all_notice.SetActive(false);
    }

    public void btn_change_status_remind_app(Carrot_Box_Item item_setting)
    {
        if (this.is_remind_app)
        {
            this.is_remind_app = false;
            PlayerPrefs.SetInt("is_remind_app", 1);
            this.cancel_notice_remind_app();
            item_setting.set_icon(this.sp_remind_off);
        }
        else
        {
            this.is_remind_app = true;
            PlayerPrefs.SetInt("is_remind_app", 0);
            this.regiter_notice_remind_app();
            item_setting.set_icon(this.sp_remind_on);
        }

        this.GetComponent<App>().play_sound();
    }


    public void re_subscribe_reminder_notice()
    {
        if (this.is_remind_app)
        {
            this.cancel_notice_remind_app();
            this.regiter_notice_remind_app();
        }
    }

    private void regiter_notice_remind_app()
    {
#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == Carrot.OS.Android)
        {
            var notification = new AndroidNotification();
            notification.Title = "Happy Diary";
            notification.Text =PlayerPrefs.GetString("app_reminder_notice_tip","Don't forget to write down how you feel today!");
            notification.FireTime = DateTime.Now.AddHours(23);
            notification.RepeatInterval = new TimeSpan(1, 0, 0, 0);
            this.id_notice_remind_app= AndroidNotificationCenter.SendNotification(notification, "alert_event");
            PlayerPrefs.SetInt("id_notice_remind_app", this.id_notice_remind_app);
        }
#endif
    }

    private void cancel_notice_remind_app()
    {
#if UNITY_ANDROID
        if (this.GetComponent<App>().carrot.os_app == Carrot.OS.Android)
        {
            AndroidNotificationCenter.CancelNotification(this.id_notice_remind_app);
            PlayerPrefs.DeleteKey("id_notice_remind_app");
            this.id_notice_remind_app = -1;
        }
#endif
    }

    public Calendar_item get_calendar_item_by_id_notice_sys(int id_notice_sys)
    {
        List<data_notice> list_n=this.get_list_notice();
        for(int i = 0; i < list_n.Count; i++)
        {
           if(list_n[i].id_notice_sys== id_notice_sys)
            {
                GameObject obj_item_calenda = Instantiate(this.GetComponent<App>().calendar.item_calendar_prefab);
                obj_item_calenda.GetComponent<Calendar_item>().load_item(list_n[i].datetime);
                obj_item_calenda.GetComponent<Calendar_item>().id_notice_sys = id_notice_sys;
                return obj_item_calenda.GetComponent<Calendar_item>();
            }
        }
        return null;
    }

    public bool get_status_remind_app()
    {
        return this.is_remind_app;
    }

    public string get_s_data_syn()
    {
        bool is_data = false;
        string s_sync = "[";
        for (int i = 0; i < this.length_notice; i++)
        {
            string id_n = PlayerPrefs.GetString("notice_" + i + "_id", "");
            if (id_n != "")
            {
                string id_notice = PlayerPrefs.GetString("notice_" + i + "_id");
                s_sync += "{";
                s_sync += "\"msg\":\""+PlayerPrefs.GetString("notice_" + i + "_msg")+"\",";
                s_sync += "\"date\":\"" + PlayerPrefs.GetString("notice_" + i + "_date") + "\"";
                s_sync += "\"id\":\"" + PlayerPrefs.GetString("notice_" + i + "_id") + "\"";
                s_sync += "\"type_sel_time\":\"" + PlayerPrefs.GetInt(id_notice+"_type_sel_time") + "\"";
                s_sync += "},";
                is_data = true;
            }
        }
        if(is_data)s_sync = s_sync.Substring(0, s_sync.Length - 1);
        s_sync = s_sync + "]";
        return s_sync;
    }

    public void delete_all_data()
    {
        for(int i = 0; i < this.length_notice; i++)
        {
            string id_n = PlayerPrefs.GetString("notice_" + i + "_id", "");
            if (id_n != "")
            {
                int id_notice_syn=PlayerPrefs.GetInt("notice_" + i + "_id_sys");
#if UNITY_ANDROID
                AndroidNotificationCenter.CancelNotification(id_notice_syn);
#endif
                PlayerPrefs.DeleteKey("notice_" + i + "_id");
                PlayerPrefs.DeleteKey("notice_" + i + "_id_sys");
                PlayerPrefs.DeleteKey("notice_" + i + "_msg");
                PlayerPrefs.DeleteKey("notice_" + i + "_date");
            }
        }
    }
}
