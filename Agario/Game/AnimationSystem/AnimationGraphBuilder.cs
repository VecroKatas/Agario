using Agario.Game.AnimationSystem.Conditions;
using SFML.Graphics;

namespace Agario.Game.AnimationSystem;

public class AnimationGraph
	{
		public List<AnimationState> States { get; }
		public List<AnimationTransition> Transitions { get; }
		public AnimationState InitialState { get; }
		public Dictionary<string, BaseAnimationParameter> Parameters = new();

		public AnimationGraph(List<AnimationState> states, List<AnimationTransition> transitions, AnimationState initialState, Dictionary<string, BaseAnimationParameter> parameters)
		{
			States = states;
			Transitions = transitions;
			InitialState = initialState;
			Parameters = parameters;
		}
	}

	public class AnimationGraphBuilder
	{
		private readonly Dictionary<string, AnimationState> _states = new();
		private readonly List<AnimationTransition> _transitions = new();
		private AnimationState? _initialState;

		private readonly Dictionary<string, BaseAnimationParameter> _parameters = new();

		public AnimationGraphBuilder AddState(string stateName, Animation animation, bool loop = true)
		{
			var state = new AnimationState(stateName, animation, loop);

			_states.Add(state.Name, state);
			return this;
		}

		public AnimationGraphBuilder SetInitialState(string stateName)
		{
			_initialState = _states[stateName];
			return this;
		}

		public AnimationGraphBuilder AddTransition(string fromStateName, string toStateName)
		{
			if (!_states.TryGetValue(fromStateName, out var fromState) || 
				!_states.TryGetValue(toStateName, out var toState))
			{
				return this;
			}

			_transitions.Add(new AnimationTransition(fromState, toState));

			return this;
		}

		public AnimationGraphBuilder AddBoolConditionTo(string stateName, string paramName, bool expectedValue)
		{
			var transition = _transitions.FirstOrDefault(t => t.FromState.Name == stateName && t.ToState.Name == paramName);

			if (transition == null)
			{
				return this;		
			}

			if (_parameters.TryGetValue(paramName, out var parameter))
			{
				transition.AddCondition(() => parameter.GetValue<bool>() == expectedValue);
			}
			else
			{
				parameter = new BoolParameter(paramName, !expectedValue);
				_parameters.Add(paramName, parameter);
				transition.AddCondition(() => parameter.GetValue<bool>() == expectedValue);
			}

			return this;
		}

		public AnimationGraph Build()
		{
			return new AnimationGraph(_states.Values.ToList(), _transitions, _initialState, _parameters);
		}
	}