namespace SfDataGridSample
{
	using System;
	using System.Reflection;

	internal class DynamicTypeInfo : TypeDelegator
	{
		private Func<string, PropertyInfo> getProperty;

		public DynamicTypeInfo(Func<string, PropertyInfo> getProperty)
			: base(typeof(object))
		{
			this.getProperty = getProperty;
		}

		public override PropertyInfo GetDeclaredProperty(string name)
		{
			return this.getProperty(name);
		}
	}
}
