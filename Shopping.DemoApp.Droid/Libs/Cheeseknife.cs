/*
 * Copyright 2014 Marcel Braghetto
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Linq;
using Android.Views;
using System.Reflection;
using Android.App;
using Android.Widget;
using System.Text;
using Android.Runtime;
using System.Collections.Generic;

namespace Com.Lilarcor.Cheeseknife {
	/// <summary>
	/// Base injection attribute, to include a ResourceId
	/// which should refer to an Android view resource id.
	/// </summary>
	public class BaseInjectionAttribute : Attribute {
		public int ResourceId { get; private set; }

		public BaseInjectionAttribute(int resourceId) {
			ResourceId = resourceId;
		}
	}

	/// <summary>
	/// Inject view attribute. Android widgets based on the
	/// View super class can be resolved at runtime when
	/// annotated with this attribute. This attribute is only
	/// permitted on instance fields.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class InjectView : BaseInjectionAttribute {	
		public InjectView(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject click event handler onto an Android View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, EventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnClick : BaseInjectionAttribute {
		public InjectOnClick(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject checked change event handler onto an Android CompoundButton View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, CompoundButton.CheckedChangeEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnCheckedChange : BaseInjectionAttribute {
		public InjectOnCheckedChange(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject editor action event handler onto an Android TextView.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, TextView.EditorActionEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnEditorAction : BaseInjectionAttribute {
		public InjectOnEditorAction(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject focus changed event handler onto an Android View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, View.FocusChangeEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnFocusChange : BaseInjectionAttribute {
		public InjectOnFocusChange(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject item click event handler onto an Android AdapterView.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, AdapterView.ItemClickEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnItemClick : BaseInjectionAttribute {
		public InjectOnItemClick(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject item long click event handler onto an Android AdapterView.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, AdapterView.ItemLongClickEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnItemLongClick : BaseInjectionAttribute {
		public InjectOnItemLongClick(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject long click event handler onto an Android View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, View.LongClickEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnLongClick : BaseInjectionAttribute {
		public InjectOnLongClick(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject touch event handler onto an Android View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, View.TouchEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnTouch : BaseInjectionAttribute {
		public InjectOnTouch(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject text changed event handler onto an Android View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, Android.Text.TextChangedEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnTextChanged : BaseInjectionAttribute {
		public InjectOnTextChanged(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject after text changed event handler onto an Android View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, Android.Text.AfterTextChangedEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnAfterTextChanged : BaseInjectionAttribute {
		public InjectOnAfterTextChanged(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Inject item selected event handler onto an Android Spinner View.
	/// Your method must have the following signature:
	/// <para></para><para></para>
	/// void SomeMethodName(object sender, Spinner.ItemSelectedEventArgs e) { ... }
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InjectOnItemSelected : BaseInjectionAttribute {
		public InjectOnItemSelected(int resourceId) : base(resourceId) { }
	}

	/// <summary>
	/// Cheeseknife exception which gets thrown when injection mappings
	/// are wrong or fail ...
	/// </summary>
	public class CheeseknifeException : Exception {
		const string PREFIX = "Cheeseknife Exception: ";

		/// <summary>
		/// Call this constructor with an Android view class type and a UI
		/// event name to indicate that the view class is not compatible
		/// with the particular event type specified.
		/// </summary>
		/// <param name="viewType">View type.</param>
		/// <param name="eventName">Event name.</param>
		public CheeseknifeException(Type viewType, string eventName) : base(GetViewTypeExceptionMessage(viewType, eventName)) { }

		/// <summary>
		/// Call this constructor with a list of required event type 
		/// parameters to indicate that the parameters couldn't be found
		/// or matched to the signature of the user attributed method.
		/// </summary>
		/// <param name="requiredEventParameters">Required event types.</param>
		public CheeseknifeException(Type[] requiredEventParameters) : base(GetArgumentTypeExceptionMessage(requiredEventParameters)) { }

		/// <summary>
		/// Gets the view type exception message for an Android view class
		/// that can't receive the specified event type.
		/// </summary>
		/// <returns>The view type exception message.</returns>
		/// <param name="viewType">View type.</param>
		/// <param name="eventName">Event name.</param>
		static string GetViewTypeExceptionMessage(Type viewType, string eventName) {
			var sb = new StringBuilder();
			sb.Append(PREFIX);
			sb.Append(" Incompatible Android view type specified for event '");
			sb.Append(eventName);
			sb.Append("', the Android view type '");
			sb.Append(viewType.ToString());
			sb.Append("' doesn't appear to support this event.");
			return sb.ToString();
		}

		/// <summary>
		/// Gets the argument type exception message when the user attributed
		/// method doesn't have the same number of parameters as the specified
		/// event signature, or the parameter types don't match between the
		/// event and user method.
		/// </summary>
		/// <returns>The argument type exception message.</returns>
		/// <param name="requiredEventParameters">Required event parameters.</param>
		static string GetArgumentTypeExceptionMessage(Type[] requiredEventParameters) {
			var sb = new StringBuilder();
			sb.Append(PREFIX);
			sb.Append(" Incorrect arguments in receiving method, should be => (");
			for(var i = 0; i < requiredEventParameters.Length; i++) {
				sb.Append(requiredEventParameters[i].ToString());
				if(i < requiredEventParameters.Length - 1) {
					sb.Append(", ");
				}
			}
			sb.Append(")");
			return sb.ToString();
		}
	}

	/// <summary>
	/// Cheeseknife! It's like a Butterknife with a weird shape!
	/// <para></para><para></para>
	/// Inspired by the extremely helpful Java based Butterknife
	/// Android library, this helper class allows for easy Android
	/// view and common event handler injections for Xamarin.Android.
	/// This injection happens at runtime rather than compile time.
	/// </summary>
	public static class Cheeseknife {
		#region EVENT / METHOD CONSTANTS
		const string METHOD_NAME_INVOKE = "Invoke";
		const string METHOD_NAME_RESOLVE_ANDROID_VIEW = "ResolveAndroidView";
		#endregion

		#region PRIVATE CONSTANTS
		const BindingFlags INJECTION_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		#endregion

		#region PUBLIC API
		/// <summary>
		/// Inject the specified parent activity, scanning all class
		/// member fields and methods for injection attributions. The
		/// assumption is that the activitie's 'Window.DecorView.RootView'
		/// represents the root view in the layout hierarchy for the
		/// given activity.<para></para>
		/// <para></para>
		/// Sample activity usage:<para></para>
		/// <para></para>
		/// [InjectView(Resource.Id.my_text_view)]<para></para>
		/// TextView myTextView;<para></para>
		/// <para></para>
		/// [InjectOnClick(Resource.Id.my_button)]<para></para>
		/// void OnMyButtonClick(object sender, EventArgs e) {<para></para>
		/// . . . myTextView.Text = "I clicked my button!";<para></para>
		/// }<para></para>
		/// <para></para>
		/// protected override void OnCreate(Bundle bundle) {<para></para>
		/// . . . base.OnCreate(bundle);<para></para>
		///<para></para>
		/// . . . SetContentView(Resource.Layout.Main);<para></para>
		/// . . . Cheeseknife.Inject(this);<para></para>
		/// <para></para>
		/// . . . myTextView.Text = "I was injected!";<para></para>
		/// }<para></para>
		/// </summary>
		/// <param name="parent">Parent.</param>
		public static void Inject(Activity parent) {
			InjectView(parent, parent.Window.DecorView.RootView);
		}

		/// <summary>
		/// Inject the specified parent and view, scanning all class
		/// member fields and methods for injection attributions.
		/// This method would normally be called to inject a fragment
		/// or other arbitrary view container. eg:<para></para>
		/// <para></para>
		/// Fragment Example Usage:<para></para>
		/// <para></para>
		/// In your OnCreateView method ...<para></para>
		/// var view = inflater.Inflate(Resource.Layout.fragment, null);<para></para>
		/// Cheeseknife.Inject(this, view);<para></para>
		/// return view;<para></para>
		/// <para></para>
		/// In your OnDestroyView method ...<para></para>
		/// Cheeseknife.Reset(this);<para></para>
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="view">View.</param>
		public static void Inject(object parent, View view) {
			InjectView(parent, view);
		}

		/// <summary>
		/// Reset the specified parent fields to null, which is useful
		/// within the OnDestroyView fragment lifecycle method, particularly
		/// if you are using RetainInstance = true.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public static void Reset(object parent) {
			// Iterate and clear all fields in the parent with the InjectView attribute ...
			foreach(var field in GetAttributedFields(typeof(InjectView), parent)) {
				field.SetValue(parent, null);
			}

			// Iterate and clear all properties in the parent with the InjectView attribute ...
			foreach(var property in GetAttributedProperties(typeof(InjectView), parent)) {
				property.SetValue(parent, null);
			}
		}
		#endregion

		#region PRIVATE API
		/// <summary>
		/// In order to prevent the linker from stripping out
		/// the Android UI events, we need to preserve a method
		/// that references each of the event types we would like
		/// to keep. Note that this method never actually gets
		/// called anywhere, but simply marks the event types
		/// to be preserved. If you want to add other events to
		/// Cheeseknife, be sure to register dummy events in this
		/// method to be sure it doesn't get stripped from your
		/// release builds. Does this feel hacky? Sure does - 
		/// thankyou Linker!
		/// </summary>
		[Preserve]
		static void InjectionEventPreserver() {
			new View(null).Click += (s, e) => {};
			new View(null).LongClick += (s, e) => {};
			new View(null).FocusChange += (s, e) => {};
			new View(null).Touch += (s, e) => {};
			new TextView(null).EditorAction += (s, e) => {};
			new TextView(null).TextChanged += (s, e) => {};
			new ListView(null).ItemClick += (s, e) => {};
			new ListView(null).ItemLongClick += (s, e) => {};
			new CheckBox(null).CheckedChange += (s, e) => {};
			new EditText(null).AfterTextChanged += (s, e) => {};
			new Spinner(null).ItemSelected += (s, e) => {};
		}

		/// <summary>
		/// Gets the injection attribute events to iterate through
		/// when checking for methods to inject in the Android view.
		/// If you want to add more injectable method types, make
		/// sure to add a new InjectOnXXXXX class, and register it
		/// in the dictionary in this method. Also don't forget to
		/// make sure the linker doesn't strip out your required
		/// Android UI event type (use the InjectionEventPreserver
		/// dummy method above to include an event reference so it
		/// doesn't get linked away in a release build).
		/// </summary>
		/// <returns>The injection attribute types.</returns>
		static Dictionary<Type, string> GetInjectableEvents() {
			var types = new Dictionary<Type, string>();

			types.Add(typeof(InjectOnClick), "Click");
			types.Add(typeof(InjectOnItemClick), "ItemClick");
			types.Add(typeof(InjectOnLongClick), "LongClick");
			types.Add(typeof(InjectOnItemLongClick), "ItemLongClick");
			types.Add(typeof(InjectOnFocusChange), "FocusChange");
			types.Add(typeof(InjectOnCheckedChange), "CheckedChange");
			types.Add(typeof(InjectOnEditorAction), "EditorAction");
			types.Add(typeof(InjectOnTouch), "Touch");
			types.Add(typeof(InjectOnTextChanged), "TextChanged");
			types.Add(typeof(InjectOnAfterTextChanged), "AfterTextChanged");
			types.Add(typeof(InjectOnItemSelected), "ItemSelected");

			return types;
		}

		/// <summary>
		/// Gets the attributed fields inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed fields.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<FieldInfo> GetAttributedFields(Type attributeType, object parent) {
			return parent.GetType().GetFields(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Gets the attributed properties inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed properties.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<PropertyInfo> GetAttributedProperties(Type attributeType, object parent) {
			return parent.GetType().GetProperties(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Gets the attributed methods inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed methods.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<MethodInfo> GetAttributedMethods(Type attributeType, object parent) {
			return parent.GetType().GetMethods(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Resolves an android view to a specific view type. This is
		/// needed to allow custom Android view classes to resolve
		/// correctly (eg, Com.Android.Volley.NetworkImageView etc).
		/// </summary>
		/// <returns>The android view.</returns>
		/// <param name="view">Parent view to resolve view from.</param>
		/// <param name="resourceId">Resource identifier.</param>
		/// <typeparam name="T">The required specific Android view type.</typeparam>
		static T ResolveAndroidView<T>(View view, int resourceId) where T : View {
			return view.FindViewById<T>(resourceId);
		}

		/// <summary>
		/// Injects the parent class by iterating over all of its
		/// fields, properties and methods, checking if they have
		/// injection attributes. For any fields/props/methods that
		/// have injection attributes do the following:<para></para>
		/// <para></para>
		/// 1. If it is a field/prop -> attempt to resolve the actual
		/// Android widget in the given view and assign it as the
		/// field value, effectively 'injecting' it.<para></para>
		/// <para></para>
		/// 2. If it is a method -> attempt to apply an event
		/// handler of the related type to the widget identified
		/// by the resource id specified in the attribute. Some
		/// widget types are verified before applying the events.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="view">View.</param>
		static void InjectView(object parent, View view) {
			var resolveMethod = typeof(Cheeseknife).GetMethod(METHOD_NAME_RESOLVE_ANDROID_VIEW, BindingFlags.Static | BindingFlags.NonPublic);

			// Grab all the instance fields in the parent class that have custom attributes
			// For each field, check whether it has the InjectView attribute
			foreach(var field in GetAttributedFields(typeof(InjectView), parent)) {
				var attribute = field.GetCustomAttribute<InjectView>();
				var genericMethod = resolveMethod.MakeGenericMethod(field.FieldType);
				var widget = genericMethod.Invoke(parent, new object[] { view, attribute.ResourceId });
				field.SetValue(parent, widget);
			}

			// Grab all the properties in the parent class that have custom attributes
			// For each field, check whether it has the InjectView attribute
			foreach(var property in GetAttributedProperties(typeof(InjectView), parent)) {
				var attribute = property.GetCustomAttribute<InjectView>();
				var genericMethod = resolveMethod.MakeGenericMethod(property.PropertyType);
				var widget = genericMethod.Invoke(parent, new object[] { view, attribute.ResourceId });
				property.SetValue(parent, widget);
			}

			// Retrieve all our registered Attribute/Event types to scan
			foreach(var injectableEvent in GetInjectableEvents()) {
				// Get the current type of injection attribute to process
				var attributeType = injectableEvent.Key;
				// Get the name of the event to apply for this injection
				var eventName = injectableEvent.Value;
				// Find any methods in the parent class that have the current injection attribute
				//var methods = parentType.GetMethods(bindingFlags).Where(x => x.IsDefined(attributeType));
				var methods = GetAttributedMethods(attributeType, parent);
				// Loop through each method with the current injection attribute
				foreach(var method in methods) {
					// And inject an event handler on it!
					InjectMethod(attributeType, method, parent, view, eventName);
				}
			}
		}

		/// <summary>
		/// Injects a method by mapping the appropriate event handler to
		/// the user's attributed receiving method.
		/// </summary>
		/// <param name="attributeType">Attribute Type.</param>
		/// <param name="method">Method.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="view">View.</param>
		/// <param name="eventName">Event name.</param>
		static void InjectMethod(Type attributeType, MethodInfo method, object parent, View view, string eventName) {
			// Check whether the provided method has the attribute represented by attributeType
			var attribute = method.GetCustomAttribute(attributeType, false) as BaseInjectionAttribute;
			// If the attribute can't be found, exit ...
			if(attribute == null) {
				return;
			}

			// Get a reference to the Android UI object with the attributed resource id
			var widget = view.FindViewById<View>(attribute.ResourceId);

			// Attempt to find the given event name on the widget.
			// If the event cannot be found, then we can't do anything
			// further ...
			var eventInfo = widget.GetType().GetEvent(eventName);
			if(eventInfo == null) {
				throw new CheeseknifeException(widget.GetType(), eventName);
			}

			// Get a list of all the user defined attributed method parameters, and the event type parameters
			var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
			var eventParameterTypes = eventInfo.EventHandlerType.GetMethod(METHOD_NAME_INVOKE).GetParameters().Select(p => p.ParameterType).ToArray();

			// If the user method doesn't define the same number of parameters as the event type, bail ...
			if(methodParameterTypes.Length != eventParameterTypes.Length) {
				throw new CheeseknifeException(eventParameterTypes);
			}

			// Step through the method parameters and event type parameters and make sure the Type of each param matches.
			for(var i = 0; i < methodParameterTypes.Length; i++) {
				if(methodParameterTypes[i] != eventParameterTypes[i]) {
					throw new CheeseknifeException(eventParameterTypes);
				}
			}

			// If we reach this stage, the user method should be able to correctly consume the dispatched event
			// so simply create a new delegate method call and add it to the Android UI object's event handler.
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, parent, method);
			eventInfo.AddEventHandler(widget, handler);
		}
		#endregion
	}
}