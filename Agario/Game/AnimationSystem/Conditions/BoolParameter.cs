namespace Agario.Game.AnimationSystem.Conditions
{
	public class BoolParameter : BaseAnimationParameter
	{
		private readonly bool _defaultValue;

		public BoolParameter(string name, bool defaultValue) : base(name, defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public override bool IsSatisfied()
		{
			if (Value == null)
			{
				return false;
			}

			return _defaultValue == (bool)Value;
		}
	}
}