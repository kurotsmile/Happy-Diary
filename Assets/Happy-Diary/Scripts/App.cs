using Carrot;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class App : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public IronSourceAds ads;
    public Calendar_happy calendar;
    public View_day view_day;
    public AudioSource[] sound;
    public Notification_Manager notice;
    public Emoji_Manager emoji_manager;
    public Note_Manager note_manager;
    public Sync_data_manager syn_data;

    void Start()
    {
        Screen.fullScreen = true;
        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.shop.onCarrotPaySuccess +=this.carrot_by_success;
        this.ads.onRewardedSuccess+=this.on_ads_rewarded_success;
        this.carrot.act_after_delete_all_data = this.calendar.freshen_calander_day_in_month;
        this.carrot.game.load_bk_music(this.sound[2]);

        Thread.CurrentThread.CurrentCulture = new CultureInfo(PlayerPrefs.GetString("lang", "en"));
        this.emoji_manager.load_emoji();
        this.carrot.delay_function(1f, this.check_rotate);
        calendar.show_calendar_all_day_in_month();
        this.notice.load();
        this.note_manager.load();
        this.view_day.load_view_day();

        /*
        Carrot_Box_Item item_syn_user = this.carrot.user.create_item_field_user_login();
        item_syn_user.set_icon(this.syn_data.icon_sync);
        item_syn_user.set_title(PlayerPrefs.GetString("syn_title", "Backup and sync calendar data"));
        item_syn_user.set_tip(PlayerPrefs.GetString("syn_tip", "Sync and backup data with carrot account"));
        item_syn_user.set_lang_data("syn_data", "syn_data_tip");
        item_syn_user.set_act(()=>this.syn_data.show_menu_sync());  
        */
    }

    private void check_exit_app()
    {
        if (this.view_day.panel_view_day.activeInHierarchy)
        {
            this.view_day.close();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void play_sound(int index_sound=0)
    {
        if(this.carrot.get_status_sound()) this.sound[index_sound].Play();
    }

    public void btn_show_setting()
    {
        this.play_sound();
        Carrot_Box box_setting = this.carrot.Create_Setting();

        /*
        Carrot_Box_Item item_syn_data = box_setting.create_item_of_top();
        item_syn_data.set_icon(this.syn_data.icon_sync);
        item_syn_data.set_title(PlayerPrefs.GetString("syn_title", "Backup and sync calendar data"));
        item_syn_data.set_tip(PlayerPrefs.GetString("syn_tip", "Sync and backup data with carrot account"));
        item_syn_data.set_lang_data("syn_title", "syn_tip");
        item_syn_data.set_act(this.act_check_syn_data);
        */

        Carrot_Box_Item item_emoji_package = box_setting.create_item_of_top();
        item_emoji_package.set_icon_white(this.emoji_manager.get_icon_emoji_package_cur());
        item_emoji_package.set_title(PlayerPrefs.GetString("emoji", "Emoji"));
        item_emoji_package.set_tip(PlayerPrefs.GetString("emoji_tip", "Change emoji packs for calendar"));
        item_emoji_package.set_lang_data("emoji", "emoji_tip");
        item_emoji_package.set_act(() => this.emoji_manager.show_list_packer_emoji(item_emoji_package));


        Carrot_Box_Item item_remind_app = box_setting.create_item_of_top("remind_app");
        item_remind_app.set_title(PlayerPrefs.GetString("remind_app", "Remind me to evaluate the calendar emotions"));
        item_remind_app.set_tip(PlayerPrefs.GetString("remind_app_tip", "Enable or disable app usage reminder"));
        item_remind_app.set_lang_data("remind_app", "remind_app_tip");
        if (this.notice.get_status_remind_app())
            item_remind_app.set_icon(this.notice.sp_remind_on);
        else
            item_remind_app.set_icon(this.notice.sp_remind_off);
        item_remind_app.set_act(() => this.notice.btn_change_status_remind_app(item_remind_app));

        box_setting.update_gamepad_cosonle_control();
        box_setting.update_color_table_row();
        box_setting.set_act_before_closing(actr_close_setting);
    }

    public void actr_close_setting(List<string> item_change)
    {
        foreach(string s in item_change)
        {
            if (s == "lang")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(this.carrot.lang.Get_key_lang());
                this.view_day.load_data_notice_at_dropdown();
                this.calendar.freshen_calander_day_in_month();
                this.notice.re_subscribe_reminder_notice();

                this.carrot.lang.Load_lang_emp();
                this.ads.show_ads_Interstitial();
            }
        }
        this.play_sound();
    }

    public void act_check_syn_data()
    {
        if (this.carrot.user.get_id_user_login() != "")
            this.syn_data.show_menu_sync();
        else
            this.carrot.user.show_login(this.syn_data.show_menu_sync);
    }

    public void carrot_by_success(string s_id_product)
    {
        if (s_id_product == this.carrot.shop.get_id_by_index(1))
        {
            this.emoji_manager.on_buy_success_package();
            this.carrot.Show_msg(PlayerPrefs.GetString("shop","Shop"),"Purchased icon pack success",Msg_Icon.Success);
        }
    }

    public void btn_show_user_carrot()
    {
        this.carrot.user.show_login();
    }

    public void change_rotate_scene()
    {
        this.carrot.delay_function(0.3f, this.check_rotate);
    }

    private void check_rotate()
    {
        bool status_rotate_portrait= this.GetComponent<Carrot_DeviceOrientationChange>().get_status_portrait();
        if (status_rotate_portrait)
        {
            this.calendar.set_width_item_calendar_year_month(150f);
            this.calendar.set_size_calendar_week(65f);
            this.calendar.set_size_item_calendar_week(80f);
        }
        else
        {
            this.calendar.set_width_item_calendar_year_month(120f);
            this.calendar.set_size_calendar_week(110f);
            this.calendar.set_size_item_calendar_week(50f);
        }   
    }

    public void on_ads_rewarded_success()
    {
        this.emoji_manager.on_rewarded_success_package();
    }
}
