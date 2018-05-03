using System;
using System.Collections.Generic;
using System.Linq;

namespace Checker
{
    public class SimpleTextChecker
    {
        private readonly List<string> _keywords;

        public SimpleTextChecker()
        {
            _keywords = new List<string> {"delay", "disruption"};
        }

        public bool TextMatchesKeywords(string text)
        {
            return _keywords.Any(text.Contains);
        }
    }
}
