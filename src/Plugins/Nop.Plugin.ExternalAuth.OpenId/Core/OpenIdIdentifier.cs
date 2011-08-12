//Contributor:  Nicholas Mayne


using DotNetOpenAuth.OpenId;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public sealed class OpenIdIdentifier
    {
        public OpenIdIdentifier(string externalIdentifier)
        {
            Identifier id;
            if (Identifier.TryParse(externalIdentifier, out id))
            {
                Identifier = id;
            }
        }

        public Identifier Identifier
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get
            {
                return Identifier != null;
            }
        }

        public override string ToString()
        {
            return Identifier != null ? Identifier.ToString() : string.Empty;
        }
    }
}