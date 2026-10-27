using System;
using System.Collections.Generic;

namespace ModernizeMe
{
    class Library
    {
        private readonly List<Item> _availableItems = new List<Item>();
        private readonly List<Loan> _loans = new List<Loan>();
        private readonly List<Member> _members = new List<Member>();

        private DateTime _now = DateTime.Now;
        public DateTime Now
        {
            get { return _now; }
            set
            {
                _now = value;
                ConsoleEx.Log(string.Format("Current date set to: {0}", _now.ToShortDateString()), "Library.Now");
            }
        }

        public void AddItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item cannot be null.");
            }

            _availableItems.Add(item);
            ConsoleEx.Log(string.Format("Item added: {0}", item), "Library.AddItem");
        }

        public void RemoveItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item cannot be null.");
            }

            _availableItems.Remove(item);
            ConsoleEx.Log(string.Format("Item removed: {0}", item), "Library.RemoveItem");
        }

        public void RegisterMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "Member cannot be null.");
            }

            if (_members.Contains(member))
            {
                throw new InvalidOperationException("Member already registered.");
            }
            _members.Add(member);
            ConsoleEx.Log(string.Format("Member registered: {0}", member.Name), "Library.RegisterMember");
        }

        public Loan TryLoanItem(Member member, Item item)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "Member cannot be null.");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item cannot be null.");
            }

            if (!_members.Contains(member))
            {
                throw new InvalidOperationException("Member not registered.");
            }
            if (!_availableItems.Contains(item))
            {
                return null;
            }
            var loan = new Loan(member, item, GetReturnDate(item));
            _loans.Add(loan);
            _availableItems.Remove(item);
            ConsoleEx.Log(string.Format("Item loaned: {0} to {1}, return by {2}", item, member.Name, loan.ReturnDate.ToShortDateString()), "Library.TryLoanItem");
            return loan;
        }

        public bool TryReturnItem(Member member, Item item, bool paidFees, out decimal? requiredFees)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "Member cannot be null.");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item cannot be null.");
            }

            var loan = _loans.Find(l => l.Member.Equals(member) && l.Item.Equals(item));

            int overdueDays = (Now.Date - loan.ReturnDate).Days;
            requiredFees = overdueDays > 0 ? overdueDays * GetOverdueFeesPerDay(item) : (decimal?)null;

            if (requiredFees > 0 && !paidFees)
            {
                ConsoleEx.Log(string.Format("Return failed: {0} has overdue fees of {1:C} for item {2}", member.Name, requiredFees, item), "Library.TryReturnItem");
                return false;
            }

            _loans.Remove(loan);
            _availableItems.Add(item);
            ConsoleEx.Log(string.Format("Successfully returned: {0} by {1}. (Paid fees: {2:C})", item, member.Name, requiredFees), "Library.TryReturnItem");

            return true;
        }

        public string GetLoanSummary(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "Member cannot be null.");
            }
            var memberLoans = _loans.FindAll(l => l.Member.Equals(member));
            if (memberLoans.Count == 0)
            {
                return string.Format("{0} has no current loans.", member.Name);
            }
            var summary = string.Format("{0} has the following loans:\n", member.Name);
            foreach (var loan in memberLoans)
            {
                summary += string.Format("- {0}, due by {1:d}\n", loan.Item, loan.ReturnDate);
            }
            return summary;
        }

        private static decimal GetOverdueFeesPerDay(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item cannot be null.");
            }

            if (item.GetType() == typeof(Book))
            {
                return 0.2m;
            }
            else if (item.GetType() == typeof(Magazine))
            {
                return 1;
            }
            else
            {
                throw new InvalidOperationException("Unknown item type.");
            }
        }

        private DateTime GetReturnDate(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item cannot be null.");
            }

            if (item.GetType() == typeof(Book))
            {
                return Now.AddDays(28).Date;
            }
            else if (item.GetType() == typeof(Magazine))
            {
                return Now.AddDays(7).Date;
            }
            else
            {
                throw new InvalidOperationException("Unknown item type.");
            }
        }
    }
}
