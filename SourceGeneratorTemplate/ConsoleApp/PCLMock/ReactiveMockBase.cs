using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PCLMock
{
	public class ReactiveMockBase<TMock> : MockBase<TMock>, INotifyPropertyChanged
	{
		public ReactiveMockBase(MockBehavior behavior = MockBehavior.Strict, bool usesSetters = false)
			: base(behavior)
		{
			UsesSetters = usesSetters;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
		
		protected bool UsesSetters { get; }
	}
}
