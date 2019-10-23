using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
using System.Web;
using FoxOne.Business;
namespace FoxOne.Controls
{

    /// <summary>
    /// KV文本框
    /// </summary>
    [DisplayName("KV文本框")]
    public class TextValueTextBox : FormControlBase
    {
        public TextValueTextBox()
        {
            DialogWidth = 700;
            DialogHeight = 400;
        }

        protected override string TagName
        {
            get { return "div"; }
        }
        public string TextID { get; set; }

        [DisplayName("选择器名称")]
        public string SelectType { get; set; }

        [DisplayName("是否多选")]
        public bool IsMulitle { get; set; }

        public ShowType ShowType { get; set; }

        public int DialogWidth { get; set; }

        public int DialogHeight { get; set; }

        public override string Render()
        {
            if (Visiable)
            {
                AddAttributes();
                var texts = new List<string>();
                if (TextID.IsNullOrEmpty())
                {
                    TextID = "{0}_Text".FormatTo(Id);
                }
                if(!Value.IsNullOrEmpty() && !SelectType.IsNullOrEmpty())
                {
                    var page = PageBuilder.BuildPage(SelectType);
                    if (page != null && page.Controls.Count > 0)
                    {
                        IFieldConverter listDs = null;
                        page.Controls.ForEach((o) =>
                        {
                            if (o is IListDataSourceControl)
                            {
                                listDs = (o as IListDataSourceControl).DataSource as IFieldConverter;
                                return;
                            }
                            else if (o is ICascadeDataSourceControl)
                            {
                                listDs = (o as ICascadeDataSourceControl).DataSource as IFieldConverter;
                                return;
                            }
                        });
                        if(listDs!=null)
                        {
                            if(IsMulitle)
                            {
                                foreach(var v in Value.Split(','))
                                {
                                    texts.Add(listDs.Converter(Id, v, null).ToString());
                                }
                            }
                            else
                            {
                                texts.Add(listDs.Converter(Id, Value, null).ToString());
                            }
                        }
                    }
                }
                string text = string.Empty;
                if(texts.Count>0)
                {
                    text = string.Join(",", texts.ToArray());
                }
                var textBox = new TextBox() { Id = TextID, Name = TextID, Value = text };
                if (!Attributes.IsNullOrEmpty())
                {
                    foreach (var attr in Attributes)
                    {
                        textBox.Attributes[attr.Key] = attr.Value;
                    }
                }
                textBox.Attributes["data-selector"] = SelectType;
                textBox.Attributes["data-showtype"] = ShowType.ToString();
                textBox.Attributes["data-multiple"] = IsMulitle.ToString().ToLower();
                textBox.Attributes["data-target"] = Id;
                textBox.Attributes["data-dialogheight"] = DialogHeight.ToString();
                textBox.Attributes["data-dialogwidth"] = DialogWidth.ToString();
                
                var hidden = new HiddenField() { Id = Id, Name = Id, Value = Value, Validator = Validator };
                string result = hidden.Render() + textBox.Render();
                return ContainerTemplate.FormatTo(Id, Label, result, Description);
            }
            return string.Empty;
        }
    }

    public enum ShowType
    {
        SlideDown,
        Modal
    }
}
