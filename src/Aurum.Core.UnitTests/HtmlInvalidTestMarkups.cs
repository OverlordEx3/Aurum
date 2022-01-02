using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Core.UnitTests
{
    internal class HtmlInvalidTestMarkups : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "<html>" };
            yield return new object[] { "<html" };
            yield return new object[] { "<html><"};
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
