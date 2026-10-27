using System;

namespace ModernizeMe
{
    class Loan
    {
        private readonly Member _member;
        private readonly Item _item;
        private readonly DateTime _returnDate;

        public Loan(Member member, Item item, DateTime returnDate)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (returnDate.TimeOfDay != TimeSpan.Zero)
            {
                throw new ArgumentException("Return date must not include a time component.", "returnDate");
            }

            _member = member;
            _item = item;
            _returnDate = returnDate;
        }
        public Member Member
        {
            get { return _member; }
        }
        public Item Item
        {
            get { return _item; }
        }
        public DateTime ReturnDate
        {
            get { return _returnDate; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + Member.GetHashCode();
                hash = (hash * 23) + Item.GetHashCode();
                hash = (hash * 23) + ReturnDate.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Loan))
            {
                return Member.Equals(((Loan)obj).Member)
                    && Item.Equals(((Loan)obj).Item)
                    && ReturnDate.Equals(((Loan)obj).ReturnDate);
            }
            return false;
        }
    }
}
