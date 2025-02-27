namespace Agario.Game.AnimationSystem.Conditions
{
	public abstract class BaseAnimationParameter
	{
		public string Name { get; }

		protected object Value { get; private set; }

		public BaseAnimationParameter(string name, object defaultValue)
		{
			Name = name;
			Value = defaultValue;
		}

		public void SetValue(object value)
		{
			Value = value;
		}

		public T GetValue<T>()
		{
			return (T)Value;
		}

		public abstract bool IsSatisfied();
	}
}