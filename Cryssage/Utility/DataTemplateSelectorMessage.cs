using Cryssage.Models;

namespace Cryssage.Utility
{
    public class DataTemplateSelectorMessage : DataTemplateSelector
    {
        public DataTemplate MessageText { get; set; }
        public DataTemplate MessageFile { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((MessageModel)item)
                .Type switch
            {
                MessageType.TEXT => MessageText,
                MessageType.FILE => MessageFile,
                _ => default,
            };
        }
    }
}
