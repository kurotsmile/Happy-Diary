using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_info_day : MonoBehaviour
{
    public Image img_icon;
    public Image img_btn;
    public Text txt_name;
    public Text txt_tip;
    public int index_func;
    public GameObject obj_btn_edit;
    public GameObject obj_btn_camera;
    public GameObject obj_btn_del;
    public GameObject obj_btn_ics;

    public void click()
    {
        if (index_func == 0)
            GameObject.Find("App").GetComponent<App>().carrot.Show_msg(GameObject.Find("App").GetComponent<App>().carrot.L("func_none", "The function is not open until the date has arrived"));
        else
            GameObject.Find("App").GetComponent<App>().view_day.btn_show_function_day(this.index_func);
    }

    public void btn_camera()
    {
        GameObject.Find("App").GetComponent<App>().view_day.show_camera();
    }

    public void btn_delete()
    {
        GameObject.Find("App").GetComponent<App>().view_day.delete_item_info(this.index_func);
    }

    public void btn_ics()
    {
        string s = "thanh";
        string s_path = Application.persistentDataPath + "/thanh.ics";
        System.IO.File.WriteAllText(s_path, s);
        GameObject.Find("App").GetComponent<App>().carrot.Show_msg("Da ghi file vao duong dan:"+ s_path);
    }
}
