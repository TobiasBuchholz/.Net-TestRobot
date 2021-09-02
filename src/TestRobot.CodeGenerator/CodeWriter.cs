using System;
using System.Collections.Generic;
using System.Text;

namespace TestRobot.CodeGenerator
{
    internal sealed class CodeWriter
    {
        private readonly StringBuilder _stringBuilder = new();

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        public void AppendLine(string line = null)
        {
            if(line == null) {
                _stringBuilder.AppendLine();
            } else {
                _stringBuilder.AppendLine(line);
            }
        }

        public void AppendLineWithIndent(int indent, string line)
        {
            _stringBuilder.Append(new string('\t', indent)).AppendLine(line);
        }

        public void AppendLines(IEnumerable<string> lines)
        {
            foreach(var line in lines) {
                AppendLine(line);
            }
        }

        public void AppendLinesWithIndent(int indent, IEnumerable<string> lines)
        {
            foreach(var line in lines) {
                AppendLineWithIndent(indent, line);
            }
        }
    }
}
