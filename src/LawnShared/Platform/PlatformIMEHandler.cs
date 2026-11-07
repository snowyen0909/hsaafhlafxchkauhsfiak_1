// Soft-keyboard text input. iOS: hidden UITextField; elsewhere: MonoGame Window.TextInput.
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.IMEHelper;
#if __IOS__
using Foundation;
using UIKit;
#endif

namespace LawnPlatform;

public class PlatformIMEHandler : IMEHandler
{
	private bool _enabled;
	private int _keyboardHeight;

	public override bool Enabled
	{
		get => _enabled;
		protected set => _enabled = value;
	}

	public override int VirtualKeyboardHeight => _keyboardHeight;

	public PlatformIMEHandler(Game game) : base(game)
	{
	}

#if __IOS__
	private sealed class BackspaceTextField : UITextField
	{
		public Action OnBackspace;

		public BackspaceTextField() : base(new CoreGraphics.CGRect(-1000, -1000, 10, 10)) { }

		public override void DeleteBackward()
		{
			OnBackspace?.Invoke();
			base.DeleteBackward();
		}
	}

	private BackspaceTextField _field;
	private NSObject _showObs, _hideObs;

	private const string Sentinel = " ";

	public override void PlatformInitialize()
	{
		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			var vc = GameInstance.Services.GetService(typeof(UIViewController)) as UIViewController ?? PlatformUI.TopViewController();
			_field = new BackspaceTextField
			{
				AutocorrectionType = UITextAutocorrectionType.No,
				AutocapitalizationType = UITextAutocapitalizationType.None,
				SpellCheckingType = UITextSpellCheckingType.No,
				ReturnKeyType = UIReturnKeyType.Done,
				Alpha = 0.02f,
				Text = Sentinel,
			};
			_field.OnBackspace = () => OnTextInput((char)Keys.Back, Keys.Back);
			_field.EditingChanged += (s, e) =>
			{
				string text = _field.Text ?? string.Empty;
				if (text.StartsWith(Sentinel)) text = text.Substring(Sentinel.Length);
				// Ignore while an IME (e.g. pinyin) is still composing.
				if (_field.MarkedTextRange != null) return;
				foreach (char c in text)
				{
					OnTextInput(c, Keys.None);
				}
				_field.Text = Sentinel;
			};
			_field.ShouldReturn = tf =>
			{
				OnTextInput((char)Keys.Enter, Keys.Enter);
				return false;
			};
			vc?.View?.AddSubview(_field);
			_showObs = UIKeyboard.Notifications.ObserveWillShow((s, e) =>
			{
				nfloat scale = UIScreen.MainScreen.Scale;
				_keyboardHeight = (int)(e.FrameEnd.Height * scale);
			});
			_hideObs = UIKeyboard.Notifications.ObserveWillHide((s, e) => _keyboardHeight = 0);
		});
	}

	public override void StartTextComposition()
	{
		if (_enabled) return;
		_enabled = true;
		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			if (_field == null) return;
			_field.Text = Sentinel;
			_field.BecomeFirstResponder();
		});
	}

	public override void StopTextComposition()
	{
		if (!_enabled) return;
		_enabled = false;
		_keyboardHeight = 0;
		UIApplication.SharedApplication.InvokeOnMainThread(() => _field?.ResignFirstResponder());
	}
#else
	public override void PlatformInitialize()
	{
	}

	public override void StartTextComposition()
	{
		_enabled = true;
	}

	public override void StopTextComposition()
	{
		_enabled = false;
	}
#endif
}
