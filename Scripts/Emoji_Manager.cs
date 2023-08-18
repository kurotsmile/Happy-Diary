using Carrot;
using System.Collections;
using UnityEngine;

public class Emoji_Manager : MonoBehaviour
{
    public Sprite icon_package_emoji;
    public GameObject item_package_prefab;
    private int sel_index_emoji = -1;
    private int index_emoji_package_buy = -1;
    private int index_emoji_package_rewarded_ads = -1;

    public GameObject[] obj_data_emoji;

    private Carrot_Box_Item item_emoji_setting = null;
    private Carrot_Box box_emoji = null;
    private int emoji_in_day_length = 0;
    public void load_emoji()
    {
        this.sel_index_emoji = PlayerPrefs.GetInt("sel_index_emoji",-1);
        this.emoji_in_day_length = PlayerPrefs.GetInt("emoji_in_day_length", 0);
        if (this.sel_index_emoji != -1) this.check_and_show_emoji();
    }

    public void show_list_packer_emoji(Carrot_Box_Item item_emoji)
    {
        this.item_emoji_setting = item_emoji;
        this.box_emoji=this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("emoji","List package emojo"), this.icon_package_emoji);
        this.GetComponent<App>().play_sound();

        if (this.sel_index_emoji == -1) this.sel_index_emoji = 0;

        for(int i = 0; i < this.obj_data_emoji.Length; i++)
        {
            GameObject item_p = this.box_emoji.add_item(this.item_package_prefab);

            Item_emoji_package i_p = item_p.GetComponent<Item_emoji_package>();
            i_p.index = i;

            if (i_p.index == this.sel_index_emoji)
                i_p.img_bk.color = this.GetComponent<App>().carrot.color_highlight;
            else
                i_p.img_bk.color = Color.white;


            Emoji_data e_data = obj_data_emoji[i].GetComponent<Emoji_data>();
            i_p.is_buy = e_data.is_buy;

            if (PlayerPrefs.GetInt("is_buy_package_" + i, 0) != 0) i_p.is_buy = false;

            if (i_p.is_buy)
            {
                i_p.obj_btn_buy.SetActive(true);
                i_p.obj_btn_rewarded_ads.SetActive(true);

            }
            else
            {
                i_p.obj_btn_buy.SetActive(false);
                i_p.obj_btn_rewarded_ads.SetActive(false);
            }

            for (int y = 0; y < i_p.img_emoji.Length; y++) i_p.img_emoji[y].sprite = e_data.sp_emoji[y];
        }
        box_emoji.update_gamepad_cosonle_control();
    }

    public void sel_package_emoji(int index_sel)
    {
        PlayerPrefs.SetInt("sel_index_emoji", index_sel);
        this.sel_index_emoji = index_sel;
        this.check_and_show_emoji();
        this.GetComponent<App>().calendar.freshen_calander_day_in_month();
        this.box_emoji.close();
    }

    private void check_and_show_emoji()
    {
        this.GetComponent<App>().view_day.sp_emoji = this.obj_data_emoji[this.sel_index_emoji].GetComponent<Emoji_data>().sp_emoji;
        if(this.item_emoji_setting!=null) this.item_emoji_setting.set_icon_white(this.GetComponent<App>().view_day.sp_emoji[0]);
    }

    public Sprite get_icon_emoji_package_cur()
    {
        int index_emoji;
        if (this.sel_index_emoji == -1) index_emoji = 0;
        else index_emoji = this.sel_index_emoji;

        return this.obj_data_emoji[index_emoji].GetComponent<Emoji_data>().sp_emoji[0];
    }

    public void buy_package_emobj(int index_check_buy)
    {
        this.index_emoji_package_buy = index_check_buy;
        this.GetComponent<App>().carrot.shop.buy_product(1);
    }

    public void set_rewarded_ads_package_emobj(int index_rewarded)
    {
        this.index_emoji_package_rewarded_ads = index_rewarded;
    }

    public void on_buy_success_package()
    {
        PlayerPrefs.SetInt("is_buy_package_"+this.index_emoji_package_buy,1);
        this.sel_package_emoji(this.index_emoji_package_buy);
    }

    public void on_rewarded_success_package()
    {
        if (this.index_emoji_package_rewarded_ads != -1)
        {
            this.sel_package_emoji(this.index_emoji_package_rewarded_ads);
            this.index_emoji_package_rewarded_ads = -1;
        }
    }

    public void add_emoji_in_day(int index_emoji,string s_id_date)
    {
        PlayerPrefs.SetInt("ej_index_" + this.emoji_in_day_length, index_emoji);
        PlayerPrefs.SetString("ej_id_" + this.emoji_in_day_length, s_id_date);
        this.emoji_in_day_length++;
        PlayerPrefs.SetInt("emoji_in_day_length", this.emoji_in_day_length);
    }

    public string get_s_data_syn()
    {
        string s_syn = "[";
        for(int i = 0; i < this.emoji_in_day_length; i++)
        {
            s_syn += "{\"index\":" + PlayerPrefs.GetInt("ej_index_" + i)+",";
            s_syn += "\"id\":\"" + PlayerPrefs.GetString("ej_id_" + i)+ "\"},";
        }
        s_syn = s_syn.Substring(0, s_syn.Length - 1);
        s_syn = s_syn+"]";
        return s_syn;
    }

    public void delete_all_data()
    {
        for (int i = 0; i < this.emoji_in_day_length; i++)
        {
            string id_day = PlayerPrefs.GetString("ej_id_" + i);
            PlayerPrefs.DeleteKey("ej_index_" + i);
            PlayerPrefs.DeleteKey("ej_id_" + i);
            PlayerPrefs.DeleteKey(id_day);
        }
    }
}
