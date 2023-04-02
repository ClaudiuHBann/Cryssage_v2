using Cryssage.Models;

namespace Cryssage
{
    public class DataTemplateSelectorMessage : DataTemplateSelector
    {
        public DataTemplate MessageText { get; set; }
        public DataTemplate MessageFile { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            switch (((MessageModel)item).Type)
            {
                case MessageType.TEXT:
                    return MessageText;
                case MessageType.FILE:
                    return MessageFile;
            }

            return default;
        }
    }
}
