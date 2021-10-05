using System;

namespace TestRobot.CodeGenerator
{
    internal sealed class MockedClassInfo
    {
        public MockedClassInfo(string mockName, string mockNamespace, string mockedInterfaceName, string mockedInterfaceNamespace)
        {
            MockName = mockName;
            MockNamespace = mockNamespace;
            MockedInterfaceName = mockedInterfaceName;
            MockedInterfaceNamespace = mockedInterfaceNamespace;
        }

        public override string ToString()
        {
            return $"[{nameof(MockedClassInfo)}: " +
                $"{nameof(MockName)}={MockName}, " +
                $"{nameof(MockNamespace)}={MockNamespace}, " +
                $"{nameof(MockedInterfaceName)}={MockedInterfaceName}, " +
                $"{nameof(MockedInterfaceNamespace)}={MockedInterfaceNamespace}]";
        }

        public string MockName { get; }
        public string MockNamespace { get; }
        public string MockedInterfaceName { get; }
        public string MockedInterfaceNamespace { get; }

        public string NameOfMockedInterfaceAsProperty => MockedInterfaceName.StartsWith("I")
                    ? $"{char.ToUpper(MockedInterfaceName[1])}{MockedInterfaceName.Substring(2)}Mock"
                    : $"{MockedInterfaceName.FirstCharToUpperCase()}Mock";
        
        public string NameOfMockedInterfaceAsField => MockedInterfaceName.StartsWith("I")
                    ? $"_{char.ToLower(MockedInterfaceName[1])}{MockedInterfaceName.Substring(2)}Mock"
                    : $"_{MockedInterfaceName.FirstCharToLowerCase()}Mock";
    }
}
