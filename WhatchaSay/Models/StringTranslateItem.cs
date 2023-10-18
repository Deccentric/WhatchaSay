using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace WhatchaSay.Model
{
    public class StringTranslateItem : QueuedXivEvent
    {
        public string Message { get; set; }
        public string Target { get; set; }
    }
}
