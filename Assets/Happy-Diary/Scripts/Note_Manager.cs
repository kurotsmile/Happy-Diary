using UnityEngine;

public class Note_Manager : MonoBehaviour
{
    private int leng_note;

    public void load()
    {
        this.leng_note = PlayerPrefs.GetInt("leng_note",0);
    }

    public void add_note(string s_msg,string s_daay_id)
    {
        PlayerPrefs.SetString("msg_note_" + this.leng_note, s_msg);
        PlayerPrefs.SetString("id_note_" + this.leng_note, s_daay_id);
        this.leng_note++;
        PlayerPrefs.SetInt("leng_note_" + this.leng_note,this.leng_note);
    }

    public string get_s_data_syn()
    {
        bool is_data = false;
        string s_sync = "[";
        for(int i = 0; i < this.leng_note; i++)
        {
            s_sync+= "{";
            if(PlayerPrefs.GetString("id_note_" + i) != "")
            {
                s_sync += "\"msg\":\"" + PlayerPrefs.GetString("msg_note_" + i)+ "\",";
                s_sync += "\"id\":\"" + PlayerPrefs.GetString("id_note_" + i)+ "\"";
                is_data = true;
            }
            s_sync += "},";
        }
        if (is_data) s_sync = s_sync.Substring(0, s_sync.Length - 1);
        s_sync += "]";
        return s_sync;
    }

    public void delete_all_data()
    {
        for(int i = 0; i < this.leng_note; i++)
        {
            PlayerPrefs.DeleteKey("msg_note_" + i);
            PlayerPrefs.DeleteKey("date_note_" + i);
        }
    }

    public void delete_note(string s_id_date)
    {
        for(int i = 0; i < this.leng_note; i++)
        {
            if(PlayerPrefs.GetString("id_note_" + i) == s_id_date)
            {
                PlayerPrefs.DeleteKey("msg_note_" + i);
                PlayerPrefs.DeleteKey("id_note_" + i);
            }
        }
    }

    public int get_index_update_by_note_id(string s_id)
    {
        for (int i = 0; i < this.leng_note; i++)
        {
            if (PlayerPrefs.GetString("id_note_" + i) == s_id) return i;
        }
        return -1;
    }

    public void update_note(int index_note,string s_msg)
    {
        PlayerPrefs.SetString("msg_note_" + index_note, s_msg);
    }
}
