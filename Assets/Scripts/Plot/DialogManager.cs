using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CircleOfLife
{
    public class DialogManager : MonoBehaviour
    {
        /// <summary>
        /// 需要读取的剧情文本，格式为csv，编码方式为UFT-8
        /// </summary>
        public TextAsset DialogDataFile;
        
        /// <summary>
        /// 对话框中说话者的名字、说的内容
        /// </summary>
        public TMP_Text NameText,ContentText;

        /// <summary>
        /// 对话框中说话者的头像
        /// </summary>
        public Image Image;

        /// <summary>
        /// 继续按钮，点击显示下一句剧情文本
        /// </summary>
        public Button NextButton;

        /// <summary>
        /// 分支选项按钮预制体
        /// </summary>
        public GameObject OptionButton;

        /// <summary>
        /// 分支选项按钮的父节点，用于实现自动排列
        /// </summary>
        public Transform ButtonGroup;

        /// <summary>
        /// 头像列表
        /// </summary>
        public List<Sprite> Sprites=new List<Sprite>();

        /// <summary>
        /// 名字、头像字典，string对应角色姓名，Sprite对应ta的头像贴图
        /// </summary>
        Dictionary<string, Sprite> ImageDictionary = new Dictionary<string, Sprite>();

        /// <summary>
        /// 对话编号，表示目前读取到剧情文本中的哪一条对话
        /// </summary>
        public int DialogIndex=0;

        /// <summary>
        /// 剧情文本的所有行数组
        /// </summary>
        public string[] DialogRows;

        public PlotBox BindUIController;

        /// <summary>
        /// 在这里把角色的名字和贴图对应起来
        /// </summary>
        private void Awake()
        {
            ImageDictionary["辛拉面"] = Sprites[0];
            ImageDictionary["五月"] = Sprites[1];
            ImageDictionary["我"]= Sprites[2];
            ImageDictionary["卓玛"]=Sprites[3];
            ImageDictionary["多吉"]=Sprites[4];
            ImageDictionary["扎西"]=Sprites[5];
            ImageDictionary["村长"]=Sprites[6];
            ImageDictionary["盗猎者"]=Sprites[7];
            ImageDictionary["？"] = Sprites[8];
            ImageDictionary["汪汪"]=Sprites[9];
        }

        void Start()
        {
            ReadText(DialogDataFile);
            PrintDialogRows();//把第一句对话加载出来
        }

        /// <summary>
        /// 更新对话框中的文本
        /// </summary>
        /// <param name="name">说话者的名字</param>
        /// <param name="content">说的内容</param>
        public void UpdateText(string name,string content)
        {
            NameText.text = name;
            ContentText.text = content;
        }

        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="name"></param>
        public void UpdateImage(string name)
        {
            Image.sprite = ImageDictionary[name];
        }

        /// <summary>
        /// 读取剧情文本
        /// </summary>
        /// <param name="textAsset">剧情文本文件</param>
        public void ReadText(TextAsset textAsset)
        {
            DialogRows = textAsset.text.Split('\n');
            
            Debug.Log("剧情文本读取成功");
        }

        /// <summary>
        /// 在对话框中显示每一条剧情文本
        /// </summary>
        public void PrintDialogRows()
        {
            for (int i = 0 ;i < DialogRows.Length ; i++)
            {
                string[] cells = DialogRows[i].Split(',');
                if (cells[0] == "#" && int.Parse(cells[1]) == DialogIndex )//普通剧情文本
                {
                    UpdateText(cells[2],cells[3]);
                    UpdateImage(cells[2]);
                    DialogIndex = int.Parse(cells[4]);
                    NextButton.gameObject.SetActive(true);
                    break;
                }
                else if(cells[0] == "@" && int.Parse(cells[1]) == DialogIndex )//分支选项文本
                {
                    NextButton.gameObject.SetActive(false);
                    GenerateOption(i);
                }
                else if (cells[0] == "END" && int.Parse(cells[1]) == DialogIndex)//结束
                {
                    Debug.Log("剧情结束");
                    BindUIController.Close();
                }
            }
        }

        /// <summary>
        /// 显示下一句（很水的注释一则）
        /// </summary>
        public void OnClickNextButton()
        {
            PrintDialogRows();
        }

        /// <summary>
        /// 生成选项按钮，并绑定选项事件
        /// </summary>
        /// <param name="index">当前的对话编号</param>
        public void GenerateOption(int index)
        {
            string[] cells = DialogRows[index].Split(",");
            if(cells[0] == "@")
            {
                Debug.Log("生成一个选项按钮");
                GameObject button = Instantiate(OptionButton, ButtonGroup);

                button.GetComponentInChildren<TMP_Text>().text = cells[3];
                button.GetComponent<Button>().onClick.AddListener(
                    delegate 
                    { 
                        OnOptionButtonClick(int.Parse(cells[4])); 
                    });

                GenerateOption(index + 1);
            }
        }

        /// <summary>
        /// 销毁选项按钮
        /// </summary>
        /// <param name="index">该选项对应的下一条对话编号</param>
        public void OnOptionButtonClick(int index)
        {
            DialogIndex= index;
            PrintDialogRows();
            for (int i = 0; i < ButtonGroup.childCount; i++)
            {
                Destroy(ButtonGroup.GetChild(i).gameObject);
            }
        }

    
    }
}
