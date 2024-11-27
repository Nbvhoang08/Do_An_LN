using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

public class SetingCanvas : UICanvas
{


    [SerializeField] private List<Button> Qbuttons; // Danh sách các nút
    [SerializeField] private List<Button> Sbuttons;
    [SerializeField] private List<Button> Mbuttons;
    [SerializeField] private List<TextMeshProUGUI> SettingText;
    [SerializeField] private FigmaImage Panel;
    private Color defaultColor = new Color32(0xD9,0xD9,0xD9,0xFF); // Màu mặc định
    private Color selectedColor = new Color32(0x5D, 0xD4, 0xE6, 0xFF); // Màu khi được chọn
    public bool isDark = true;
    bool Momthoi;
    private void Start()
    {
        // Gắn sự kiện OnClick cho từng nút trong danh sách
        foreach (Button button in Qbuttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button,Qbuttons));
        }
        foreach (Button button in Sbuttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button, Sbuttons));
        }
        foreach (Button button in Mbuttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button, Mbuttons));
        }


        
    }
    private void Update()
    {
        if (isDark)
        {
            Panel.FillColor = Color.black; // chuyen panel thank den
            foreach(TextMeshProUGUI text in SettingText)
            {
                if(text.color != Color.white)
                {
                    text.color = Color.white;
                }
            }
        }
        else
        {
            Panel.FillColor = new Color32(0xEE, 0xEE, 0xEE, 0xFF);
            foreach (TextMeshProUGUI text in SettingText)
            {
                if (text.color != Color.black)
                {
                    text.color = Color.black;
                }
            }
           
        }

        
    }

    private void SetMode(bool value)
    {
        if (isDark != value)
        {
            isDark = value;
        }
    }
    public void LightMode()
    {
        SetMode(false);
    }
    public void DarkMode()
    {
        SetMode(true);
    }
   
    

    public void OnVolume() 
    {
        SoundManger.Instance.TurnOn = true;
    }
    public void OffVolume() 
    {
        SoundManger.Instance.TurnOn = false;
    }




    private void OnButtonClicked(Button selectedButton , List<Button> buttons)
    {
        // Đặt lại màu cho tất cả các nút về màu mặc định
        foreach (Button button in buttons)
        {
            button.GetComponent<FigmaImage>().FillColor = defaultColor;
        }

        // Đổi màu cho nút được chọn
        selectedButton.GetComponent<FigmaImage>().FillColor = selectedColor;
    }
    public void BackBtn()
    {
        UIManager.Instance.CloseUIDirectly<SetingCanvas>();// Tat truc tiep
    }

}
