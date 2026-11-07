// Soft-keyboard text input. iOS: hidden UITextField; elsewhere: no-op.
//
// VirtualKeyboardHeight follows the Android implementation's meaning: it is the height of the
// part of the screen NOT covered by the keyboard (i.e. the keyboard's top edge, in back-buffer
// pixels), or 0 while unknown. WidgetManager uses it to move the edited widget above the keyboard.
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
	private volatile int _visibleHeight;

	/// <summary>Set from the UI thread; consumed by the game thread (Main.Update).</summary>
	public static volatile bool DismissRequested;

	/// <summary>Set when the keyboard frame changed and the edited widget should be re-placed.</summary>
	public static volatile bool RelayoutRequested;

	public override bool Enabled
	{
		get => _enabled;
		protected set => _enabled = value;
	}

	public override int VirtualKeyboardHeight => _visibleHeight;

	public PlatformIMEHandler(Game game) : base(game)
	{
	}

#if __IOS__
	private sealed class BackspaceTextField : UITextField
	{
		public Action OnBackspace;

		public BackspaceTextField() : base(new CoreGraphics.CGRect(0, 0, 2, 2)) { }

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
				// Wait until an IME (pinyin / zhuyin...) has committed the text.
				if (_field.MarkedTextRange != null) return;
				string text = _field.Text ?? string.Empty;
				if (text.StartsWith(Sentinel)) text = text.Substring(Sentinel.Length);
				foreach (char c in text)
				{
					OnTextInput(c, Keys.None);
				}
				_field.Text = Sentinel;
			};
			_field.ShouldReturn = tf =>
			{
				DismissRequested = true;
				return false;
			};
			vc?.View?.AddSubview(_field);

			_showObs = UIKeyboard.Notifications.ObserveWillChangeFrame((s, e) => UpdateKeyboardFrame(e.FrameEnd));
			_hideObs = UIKeyboard.Notifications.ObserveWillHide((s, e) =>
			{
				_visibleHeight = ScreenPixelHeight();
				if (_enabled)
				{
					// Keyboard closed by the system (e.g. hide-keyboard key on iPad).
					DismissRequested = true;
				}
			});
		});
	}

	private static int ScreenPixelHeight()
	{
		var b = UIScreen.MainScreen.Bounds;
		nfloat h = b.Height < b.Width ? b.Height : b.Width; // landscape-only game
		return (int)(h * UIScreen.MainScreen.Scale);
	}

	private void UpdateKeyboardFrame(CoreGraphics.CGRect frameEnd)
	{
		int screenH = ScreenPixelHeight();
		int top = (int)(frameEnd.Y * UIScreen.MainScreen.Scale);
		if (top <= 0 || top > screenH)
		{
			top = screenH;
		}
		_visibleHeight = top;
		RelayoutRequested = true;
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
