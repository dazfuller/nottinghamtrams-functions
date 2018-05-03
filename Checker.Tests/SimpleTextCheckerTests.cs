using System;
using Xunit;
using FluentAssertions;
using Checker;

namespace Checker.Tests
{
    public class SimpleTextCheckerTests
    {
        [Fact]
        public void TestDisruptionMessage()
        {
            var message = "Warning: disruption affecting all services owing to power failure";
            
            var checker = new SimpleTextChecker();
            checker.TextMatchesKeywords(message).Should().BeTrue();
        }
        
        [Fact]
        public void TestDelayMessage()
        {
            var message = "There are significant delays affecting the service because of a broken down tram";
            
            var checker = new SimpleTextChecker();
            checker.TextMatchesKeywords(message).Should().BeTrue();
        }
        
        [Fact]
        public void TestNoIssue()
        {
            var message = "Everything is great";
            
            var checker = new SimpleTextChecker();
            checker.TextMatchesKeywords(message).Should().BeFalse();
        }
    }
}
