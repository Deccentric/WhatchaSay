using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace WhatchaSay.Model
{
    public class ChatTranslateItem: QueuedXivEvent
    {
        public SeString Message { get; set; }
        public SeString Sender { get; set; }
        public XivChatType ChatType { get; set; }
    }
}
