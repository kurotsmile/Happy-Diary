using Carrot;
using System;
using System.Collections;
using UnityEngine;

public class Sync_data_manager : MonoBehaviour
{
    public App app;
    public Sprite icon_sync;
    public Sprite icon_sync_download;
    public Sprite icon_sync_upload;

    private Carrot_Box box_sync_download;
    private Carrot_Window_Msg msg_quesion_sync;
    private int id_sync_delete_temp = -1;
    private int id_sync_load_temp = -1;
    private bool is_sync_replace = true;

    public void show_list_syn()
    {
        this.app.carrot.play_sound_click();
        //WWWForm frm_list_syn = this.app.carrot.frm_act("syn_list");
        //frm_list_syn.AddField("user_id", this.app.carrot.user.get_id_user_login());
        //frm_list_syn.AddField("user_lang", this.app.carrot.user.get_lang_user_login());
        //this.app.carrot.send(frm_list_syn, act_done_show_list_sync); 
    }

    private void act_done_show_list_sync(string s_data)
    {
        IList list_syn = (IList) Json.Deserialize(s_data);

        if (list_syn.Count <=0)
        {
            if (this.box_sync_download != null) this.box_sync_download.close();
            this.app.carrot.Show_msg(this.app.carrot.L("syn_title", "Backup and sync calendar data"), this.app.carrot.L("syn_data_no", "You have no backup created yet!"));
            return;
        }

        if (this.box_sync_download != null) this.box_sync_download.close();
        this.box_sync_download = this.app.carrot.Create_Box(this.app.carrot.L("syn_title", "Backup and sync calendar data"), this.icon_sync_download);
        for(int i = 0; i < list_syn.Count; i++)
        {
            IDictionary data_syn = (IDictionary)list_syn[i];
            var id_sync = int.Parse(data_syn["id"].ToString());

            Carrot_Box_Item syn_item=box_sync_download.create_item("item_syn_download");
            syn_item.set_icon(this.icon_sync_download);
            syn_item.set_title(data_syn["date"].ToString());
            syn_item.set_tip(data_syn["date"].ToString());
            syn_item.set_act(() => load_sync(id_sync));

            Carrot_Box_Btn_Item btn_syn_del=syn_item.create_item();
            btn_syn_del.set_icon(this.app.carrot.sp_icon_del_data);
            btn_syn_del.set_color(this.app.carrot.color_highlight);
            btn_syn_del.set_act(() => delete_sync(id_sync));
        }
        box_sync_download.update_color_table_row();
    }

    private void delete_sync(int s_id)
    {
        this.app.carrot.play_sound_click();
        this.id_sync_delete_temp = s_id;
        this.msg_quesion_sync=this.app.carrot.show_msg(PlayerPrefs.GetString("syn_title", "Backup and sync calendar data"), PlayerPrefs.GetString("delete_tip", "Are you sure you want to delete the selected item?"), delete_sync_yes, delete_sync_no);
    }

    private void delete_sync_yes()
    {
        this.app.carrot.play_sound_click();
        WWWForm frm_syn_delete = this.app.carrot.frm_act("delete_syn_data");
        frm_syn_delete.AddField("id_syn", this.id_sync_delete_temp);
        //this.app.carrot.send(frm_syn_delete, act_after_delete_sync);
    }

    private void delete_sync_no()
    {
        this.app.carrot.play_sound_click();
        this.msg_quesion_sync.close();
    }

    private void load_sync(int s_id)
    {
        this.id_sync_load_temp = s_id;
        this.app.carrot.play_sound_click();
        this.msg_quesion_sync = this.app.carrot.show_msg(PlayerPrefs.GetString("syn_recover", "Data recovery"), "Select data synchronization type");
        this.msg_quesion_sync.add_btn_msg(PlayerPrefs.GetString("syn_recover_replace", "Replace"), act_sync_replace);
        this.msg_quesion_sync.add_btn_msg(PlayerPrefs.GetString("syn_recover_additional", "Additional"), act_sync_additional);
    }

    private void act_sync_replace()
    {
        this.app.carrot.play_sound_click();
        this.is_sync_replace = true;
        this.msg_quesion_sync.close();
        this.act_sync();
    }

    private void act_sync_additional()
    {
        this.app.carrot.play_sound_click();
        this.is_sync_replace = false;
        this.msg_quesion_sync.close();
        this.act_sync();
    }

    private void act_sync()
    {
        this.app.carrot.play_sound_click();
        WWWForm frm_syn_load = this.app.carrot.frm_act("load_sync_data");
        frm_syn_load.AddField("id_syn", this.id_sync_load_temp);
        //this.app.carrot.send(frm_syn_load, act_after_load_sync_done);
    }

    private void act_after_load_sync_done(string s_data)
    {
        IDictionary data_syn = (IDictionary)Json.Deserialize(s_data);

        if (this.is_sync_replace)
        {
            this.app.emoji_manager.delete_all_data();
            this.app.notice.delete_all_data();
            this.app.note_manager.delete_all_data();
        }

        if (data_syn["emoji"] != null)
        {
            IList list_emoji = (IList)data_syn["emoji"];
            for (int i = 0; i < list_emoji.Count; i++)
            {
                IDictionary data_emoji = (IDictionary)list_emoji[i];
                this.app.view_day.add_emoji_day_data(data_emoji["id"].ToString(), int.Parse(data_emoji["index"].ToString()));
            }
        }

        if (data_syn["notice"] != null)
        {
            IList list_notice = (IList)data_syn["notice"];
            for (int i = 0; i < list_notice.Count; i++)
            {
                IDictionary data_notice = (IDictionary)list_notice[i];
                this.app.view_day.add_notice_day_data(data_notice["msg"].ToString(), data_notice["id"].ToString(), new DateTime(long.Parse(data_notice["date"].ToString())));
                PlayerPrefs.SetString(data_notice["id"].ToString(), data_notice["msg"].ToString());
                PlayerPrefs.SetInt(data_notice["id"].ToString() + "_type_sel_time", int.Parse(data_notice["type_sel_time"].ToString()));
            }
        }

        if (data_syn["note"] != null)
        {
            IList list_note = (IList)data_syn["note"];
            for (int i = 0; i < list_note.Count; i++)
            {
                IDictionary data_note = (IDictionary)list_note[i];
                this.app.view_day.add_note_day_data(data_note["msg"].ToString(), data_note["id"].ToString());
            }
        }

        this.app.calendar.freshen_calander_day_in_month();
        this.app.carrot.show_msg("Successful data recovery!");
    }

    private void act_after_delete_sync(string s_data)
    { 
        this.msg_quesion_sync.close();
        this.show_list_syn();
    }

    [ContextMenu("syn data online")]
    public void syn_data_online()
    {
        string s_json = "{";
        s_json+= "\"emoji\":" + this.app.emoji_manager.get_s_data_syn()+",";
        s_json += "\"notice\":" + this.app.notice.get_s_data_syn();
        s_json += "\"note\":" + this.app.note_manager.get_s_data_syn();
        s_json +="}";
        WWWForm frm_syn = this.app.carrot.frm_act("syn_data");
        frm_syn.AddField("user_id", this.app.carrot.user.get_id_user_login());
        frm_syn.AddField("user_lang", this.app.carrot.user.get_lang_user_login());
        frm_syn.AddField("syn_data", s_json);
        if(this.app.carrot.model_app==ModelApp.Develope) Debug.Log("syn data online...:" + s_json);
        //this.app.carrot.send(frm_syn, act_sync_data_success);
    }

    private void act_sync_data_success(string s_data)
    {
        this.app.carrot.Show_msg(this.app.carrot.L("syn_title", "Backup and sync calendar data"), this.app.carrot.L("syn_backup_success","Successful data backup created!"),Msg_Icon.Success);
    }

    public void show_menu_sync()
    {
        this.app.carrot.play_sound_click();
        Carrot_Box box_syn=this.app.carrot.Create_Box(this.app.carrot.L("syn_title", "Backup and sync calendar data"), this.icon_sync);
        Carrot_Box_Item item_syn_download = box_syn.create_item_of_top();
        item_syn_download.set_icon(this.icon_sync_download);
        item_syn_download.set_title(this.app.carrot.L("syn_recover", "Data recovery"));
        item_syn_download.set_tip(this.app.carrot.L("syn_recover_tip", "Recover data from server"));
        item_syn_download.set_lang_data("syn_recover_title", "syn_recover_tip");
        item_syn_download.set_act(this.show_list_syn);

        Carrot_Box_Item item_syn_upload = box_syn.create_item_of_top();
        item_syn_upload.set_icon(this.icon_sync_upload);
        item_syn_upload.set_title(this.app.carrot.L("syn_backup", "Data backup"));
        item_syn_upload.set_tip(this.app.carrot.L("syn_backup_tip", "Store your data on the server"));
        item_syn_upload.set_lang_data("syn_backup", "syn_backup_tip");
        item_syn_upload.set_act(this.syn_data_online);
    }
}
