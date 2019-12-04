using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using NSubstitute;
using Okta.Sdk;
using reporting_tool;
using Xunit;

namespace TestBoolExpr
{
    public class UserFilterTest
    {
        [Fact]
        public void TestFailingExpression()
        {
            var expression = "profile.LOA eq 3";

            Assert.Throws<ParseCanceledException>(() => new UserFilter(expression));
        }
        
        [Fact]
        public void TestContainsProfileAttributeTrue()
        {
            var expression = "profile.LOA co \"3\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User(){Profile = new UserProfile{["LOA"] = "asdf3asdf"}};
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestContainsProfileAttributeFalse()
        {
            var expression = "profile.LOA co \"5\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User(){Profile = new UserProfile{["LOA"] = "asdf3asdf"}};
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestContainsArrayProfileAttributeTrue()
        {
            var expression = "profile.LOA co \"3\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User(){Profile = new UserProfile{["LOA"] = new List<string>{"asdf3asdf"}}};
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestContainsArrayProfileAttributeFalse()
        {
            var expression = "profile.LOA co \"5\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User(){Profile = new UserProfile{["LOA"] = new List<string>{"asdf3asdf", "asdfkl2oi"}}};
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestStartsWithProfileAttributeTrue()
        {
            var expression = "profile.SourceType sw \"ei\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            var userProfile = new UserProfile {["SourceType"] = "eidm"};
            user.Profile = userProfile;
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestStartsWithProfileAttributeFalse()
        {
            var expression = "profile.SourceType sw \"ei\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            var userProfile = new UserProfile {["SourceType"] = "HARP"};
            user.Profile = userProfile;
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestEndsWithProfileAttributeTrue()
        {
            const string expression = "profile.SourceType ew \"ei\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            var userProfile = new UserProfile {["SourceType"] = "Miei"};
            user.Profile = userProfile;
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestEndsWithProfileAttributeFalse()
        {
            const string expression = "profile.SourceType ew \"ei\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            var userProfile = new UserProfile {["SourceType"] = "HARP"};
            user.Profile = userProfile;
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestEqualProfileAttribute()
        {
            var expression = "profile.LOA eq \"3\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            var userProfile = new UserProfile {["LOA"] = "3"};
            user.Profile = userProfile;
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestNotEqualProfileAttribute()
        {
            var expression = "not profile.LOA eq \"3\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            var userProfile = new UserProfile {["LOA"] = "3"};
            user.Profile = userProfile;
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestNotEqualNonProfileAttribute()
        {
            var expression = "not created eq \"3\"";

            var f = new UserFilter(expression).F;
            
            IUser user = new User();
            
            Assert.True(f(user));
        }
        
        [Fact] public void TestEqualNonProfileAttribute()
        {
            var expression = "status eq \"ACTIVE\"";

            var f = new UserFilter(expression).F;

            var user = Substitute.For<IUser>();
            user.Status.Returns(UserStatus.Active);
            
            Assert.True(f(user));
        }
        
        [Fact] 
        public void TestPresentProfileAttributeFalse()
        {
            var expression = "profile.SourceType pr";

            var f = new UserFilter(expression).F;

            var user = new User {Profile = new UserProfile {["LOA"] = "eidm"}};

            Assert.False(f(user));
        }
        
        [Fact] 
        public void TestPresentProfileAttributeTrue()
        {
            var expression = "profile.SourceType pr";

            var f = new UserFilter(expression).F;

            var user = new User {Profile = new UserProfile {["SourceType"] = "eidm"}};

            Assert.True(f(user));
        }

        [Fact]
        public void TestAndTrue()
        {
            var expression = "profile.LOA eq \"3\" and profile.SourceType eq \"eidm\"";
            var f = new UserFilter(expression).F;

            var user = new User {Profile = new UserProfile {["SourceType"] = "eidm", ["LOA"] = "3"}};
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestAndFalse()
        {
            var expression = "profile.LOA eq \"3\" and profile.SourceType eq \"eidm\"";
            var f = new UserFilter(expression).F;

            var user = new User {Profile = new UserProfile {["SourceType"] = "edm", ["LOA"] = "3"}};
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestOrTrue()
        {
            var expression = "profile.LOA eq \"3\" or profile.SourceType eq \"eidm\"";
            var f = new UserFilter(expression).F;

            var user = new User {Profile = new UserProfile {["SourceType"] = "eidm", ["LOA"] = "3"}};
            
            Assert.True(f(user));
        }
        
        [Fact]
        public void TestOrFalse()
        {
            var expression = "profile.LOA eq \"3\" or profile.SourceType eq \"eidm\"";
            var f = new UserFilter(expression).F;

            var user = new User {Profile = new UserProfile {["SourceType"] = "edm", ["LOA"] = "2"}};
            
            Assert.False(f(user));
        }

        [Fact]
        public void TestParenthesisFalse()
        {
            const string expression =
                "(profile.LOA eq \"3\" or profile.SourceType eq \"eidm\") and profile.dateOfBirth pr";
            var f = new UserFilter(expression).F;
            
            var user = new User {Profile = new UserProfile {["SourceType"] = "edm", ["LOA"] = "2", ["dateOfBirth"] = "a"}};
            
            Assert.False(f(user));
        }
        
        [Fact]
        public void TestParenthesisTrue()
        {
            var expression =
                "(profile.LOA eq \"3\" or profile.SourceType eq \"eidm\") and profile.dateOfBirth pr";
            var f = new UserFilter(expression).F;
            
            var user = new User {Profile = new UserProfile {["SourceType"] = "edm", ["LOA"] = "3", ["dateOfBirth"] = "a"}};
            
            Assert.True(f(user));
        }

        [Fact]
        public void TestEmptyExpression()
        {
            var f = new UserFilter("").F;
            
            var user = new User();
            
            Assert.True(f(user));
        }
    }
}