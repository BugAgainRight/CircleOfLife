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
        public SpriteRenderer Image;

        /// <summary>
        /// 继续按钮，点击显示下一句剧情文本
        /// </summary>
        public Button NextButton;

        /// <summary>
        /// 分支选项按钮预制体
        /// </summary>
        public Button OptionButton;

        /// <summary>
        /// 分支选项按钮的父节点，用于实现自动排列
        /// </summary>
        public Transform ButtonGroup;

        /// <summary>
        /// 头像列表
        /// </summary>
        public List<Sprite> Sprites=new List<Sprite>();

        /// <summary>
        /// 名字、头像字典，string对应角色姓名，Sprite对应ta的头像
        /// </summary>
        Dictionary<string, Sprite> ImageDictionary = new Dictionary<string, Sprite>();

        /// <summary>
        /// 表示目前读取到剧情文本中的哪一条对话
        /// </summary>
        public int DialogIndex=0;

        /// <summary>
        /// 剧情文本的所有行数组
        /// </summary>
        public string[] DialogRows;

        /// <summary>
        /// 在这里把角色的名字和贴图对应起来
        /// </summary>
        private void Awake()
        {
            ImageDictionary["辛拉面"] = Sprites[0];
            ImageDictionary["五月"] = Sprites[1];
        }

        void Start()
        {
            ReadText(DialogDataFile);
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
            foreach (string row in DialogRows)
            {
                string[] cells = row.Split(',');
                if (cells[0] == "#" && int.Parse(cells[1]) == DialogIndex )
                {
                    UpdateText(cells[2],cells[3]);
                    UpdateImage(cells[2]);

                    DialogIndex = int.Parse(cells[4]);
                    break;
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


    }
}
