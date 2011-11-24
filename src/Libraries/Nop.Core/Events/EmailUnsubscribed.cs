namespace Nop.Core.Events {
    public class EmailUnsubscribed {
        private readonly string _email;

        public EmailUnsubscribed(string email) {
            _email = email;
        }

        public string Email {
            get { return _email; }
        }

        public bool Equals(EmailUnsubscribed other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._email, _email);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EmailUnsubscribed)) return false;
            return Equals((EmailUnsubscribed) obj);
        }

        public override int GetHashCode() {
            return (_email != null ? _email.GetHashCode() : 0);
        }
    }
}