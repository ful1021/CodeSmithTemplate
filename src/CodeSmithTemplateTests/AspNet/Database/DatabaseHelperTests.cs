using Xunit;
using CodeSmithTemplate.AspNet.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shouldly;

namespace CodeSmithTemplate.AspNet.Database.Tests
{
    public class DatabaseHelperTests
    {
        [Fact()]
        public void GetModelNameTest()
        {

            DatabaseHelper.GetNoSplitName("Item_1").Length.ShouldBe(4);
            DatabaseHelper.GetNoSplitName("Item_12").Length.ShouldBe(4);
            DatabaseHelper.GetNoSplitName("Item_").Length.ShouldBe(4);
            DatabaseHelper.GetNoSplitName("Item").Length.ShouldBe(4);

            DatabaseHelper.GetNoSplitName("").Length.ShouldBe(0);

            DatabaseHelper.GetNoSplitName(null)?.Length.ShouldBe(0);

        }


    }
}