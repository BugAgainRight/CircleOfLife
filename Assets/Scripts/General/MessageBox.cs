using System.Collections;
using System.Collections.Generic;
using Milease.Enums;
using Milease.Utils;
using Milutools.Milutools.UI;
using TMPro;
using UnityEngine;

namespace CircleOfLife.General
{
    public class MessageBox : ManagedUI<MessageBox, (string Title, string Content), MessageBox.Operation>
    {
        public enum Operation
        {
            Accept, Deny
        }

        public TMP_Text Title, Content;
        public RectTransform Panel;

        protected override void Begin()
        {
            
        }

        public void Accept() => Close(Operation.Accept);
        
        public void Deny() => Close(Operation.Deny);
        
        protected override void AboutToClose()
        {
            Panel.Milease(UMN.AnchoredPosition, Vector2.zero,new Vector2(0, -1000f), 
                    0.25f, 0f, EaseFunction.Back, EaseType.In)
                .Play();
        }

        public override void AboutToOpen((string Title, string Content) parameter)
        {
            Title.text = parameter.Title;
            Content.text = parameter.Content;
            
            Panel.Milease(UMN.AnchoredPosition, new Vector2(0, -1000f), Vector2.zero,
                    0.5f, 0f, EaseFunction.Back, EaseType.Out)
                .Play();
        }
    }
}
