namespace Nop.Plugin.POS.Kaching.Models
{
    public struct NameUnion
    {
        public Description NameClass;
        public string String;

        public bool IsNull => NameClass == null && String == null;
    }
}