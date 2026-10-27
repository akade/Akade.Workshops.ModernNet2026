namespace ModernizeMe
{
    abstract class Item
    {
        private readonly string _name;

        public Item(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    class Book : Item
    {
        private readonly string _author;

        public Book(string name, string author) : base(name)
        {
            _author = author;
        }

        public string Author
        {
            get { return _author; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + Name.GetHashCode();
                hash = (hash * 23) + Author.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Book))
            {
                return Name.Equals(((Book)obj).Name) && Author.Equals(((Book)obj).Author);
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} by {1}", Name, Author);
        }
    }

    class Magazine : Item
    {
        private readonly int _issueNumber;
        public Magazine(string name, int issueNumber) : base(name)
        {
            _issueNumber = issueNumber;
        }
        public int IssueNumber
        {
            get { return _issueNumber; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + Name.GetHashCode();
                hash = (hash * 23) + IssueNumber.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Magazine))
            {
                return Name.Equals(((Magazine)obj).Name) && IssueNumber.Equals(((Magazine)obj).IssueNumber);
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} (Issue {1})", Name, IssueNumber);
        }
    }
}
