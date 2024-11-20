namespace SfDataGridSample
{
	using System;
	using System.Reflection;

	internal class DynamicParameterInfo : ParameterInfo
	{
		private MemberInfo member;
		private Type type;

		public DynamicParameterInfo(MemberInfo member, Type type, string name)
		{
			this.member = member;
			this.type = type;
		}

		public override MemberInfo Member
		{
			get { return this.member; }
		}

		public override Type ParameterType
		{
			get { return this.type; }
		}
	}
}
