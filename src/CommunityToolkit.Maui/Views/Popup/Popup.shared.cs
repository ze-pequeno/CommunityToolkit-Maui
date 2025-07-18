using CommunityToolkit.Maui.Extensions;

namespace CommunityToolkit.Maui.Views;

/// <summary>
/// Represents a small View that pops up at front the Page.
/// </summary>
public partial class Popup : ContentView
{
	/// <summary>
	/// Bindable property to set the margin between the <see cref="Popup"/> and the edge of the window
	/// </summary>
	public static new readonly BindableProperty MarginProperty = View.MarginProperty;

	/// <summary>
	/// Bindable property to set the padding between the <see cref="Popup"/> border and the <see cref="Popup"/> content
	/// </summary>
	public static new readonly BindableProperty PaddingProperty = ContentView.PaddingProperty;

	/// <summary>
	/// Bindable property to set the horizontal position of the <see cref="Popup"/> when displayed on screen
	/// </summary>
	public static new readonly BindableProperty HorizontalOptionsProperty = View.HorizontalOptionsProperty;

	/// <summary>
	/// Bindable property to set the vertical position of the <see cref="Popup"/> when displayed on screen
	/// </summary>
	public static new readonly BindableProperty VerticalOptionsProperty = View.VerticalOptionsProperty;

	/// <summary>
	///  Backing BindableProperty for the <see cref="CanBeDismissedByTappingOutsideOfPopup"/> property.
	/// </summary>
	public static readonly BindableProperty CanBeDismissedByTappingOutsideOfPopupProperty = BindableProperty.Create(nameof(CanBeDismissedByTappingOutsideOfPopup), typeof(bool), typeof(Popup), Options.DefaultPopupSettings.CanBeDismissedByTappingOutsideOfPopup);

	/// <summary>
	/// Initializes Popup
	/// </summary>
	public Popup()
	{
		Margin = Options.DefaultPopupSettings.Margin;
		Padding = Options.DefaultPopupSettings.Padding;
		HorizontalOptions = Options.DefaultPopupSettings.HorizontalOptions;
		VerticalOptions = Options.DefaultPopupSettings.VerticalOptions;
		BackgroundColor = Options.DefaultPopupSettings.BackgroundColor;
	}

	/// <summary>
	/// Event occurs when <see cref="Popup"/> is opened.
	/// </summary>
	public event EventHandler? Opened;

	/// <summary>
	/// Event occurs when <see cref="Popup"/> is closed.
	/// </summary>
	public event EventHandler? Closed;

	/// <summary>
	/// Sets the margin between the <see cref="Popup"/> and the edge of the window
	/// </summary>
	public new Thickness Margin
	{
		get => base.Margin;
		set => base.Margin = value;
	}

	/// <summary>
	/// Sets the padding between the <see cref="Popup"/> border and the <see cref="Popup"/> content
	/// </summary>
	public new Thickness Padding
	{
		get => base.Padding;
		set => base.Padding = value;
	}

	/// <summary>
	/// Sets the horizontal position of the <see cref="Popup"/> when displayed on screen
	/// </summary>
	public new LayoutOptions HorizontalOptions
	{
		get => base.HorizontalOptions;
		set => base.HorizontalOptions = value;
	}

	/// <summary>
	/// Sets the vertical position of the <see cref="Popup"/> when displayed on screen
	/// </summary>
	public new LayoutOptions VerticalOptions
	{
		get => base.VerticalOptions;
		set => base.VerticalOptions = value;
	}

	/// <inheritdoc cref="IPopupOptions.CanBeDismissedByTappingOutsideOfPopup"/> />
	/// <remarks>
	/// When true and the user taps outside the popup, it will dismiss.
	/// On Android - when false the hardware back button is disabled.
	/// </remarks>
	public bool CanBeDismissedByTappingOutsideOfPopup
	{
		get => (bool)GetValue(CanBeDismissedByTappingOutsideOfPopupProperty);
		set => SetValue(CanBeDismissedByTappingOutsideOfPopupProperty, value);
	}

	/// <summary>
	/// Close the Popup.
	/// </summary>
	public virtual Task CloseAsync(CancellationToken token = default) => GetPopupPage().CloseAsync(new PopupResult(false), token);

	internal void NotifyPopupIsOpened()
	{
		Opened?.Invoke(this, EventArgs.Empty);
	}

	internal void NotifyPopupIsClosed()
	{
		Closed?.Invoke(this, EventArgs.Empty);
	}

	private protected PopupPage GetPopupPage()
	{
		var parent = Parent;

		while (parent is not null)
		{
			if (parent.Parent is PopupPage popuppage)
			{
				return popuppage;
			}

			parent = parent.Parent;
		}

		throw new PopupNotFoundException();
	}
}

/// <summary>
/// Represents a small View that pops up at front the Page.
/// </summary>
public partial class Popup<T> : Popup
{
	/// <summary>
	/// Close the Popup with a result.
	/// </summary>
	/// <param name="result">Popup result</param>
	/// <param name="token"><see cref="CancellationToken"/></param>
	public virtual Task CloseAsync(T result, CancellationToken token = default) => GetPopupPage().CloseAsync(new PopupResult<T>(result, false), token);
}

sealed class PopupNotFoundException() : InvalidPopupOperationException($"Unable to close popup: could not locate {nameof(PopupPage)}. {nameof(PopupExtensions.ShowPopup)} or {nameof(PopupExtensions.ShowPopupAsync)} must be called before {nameof(Popup.CloseAsync)}. If using a custom implementation of {nameof(Popup)}, override the {nameof(Popup.CloseAsync)} method");
sealed class PopupBlockedException(in Page currentVisibleModalPage) : InvalidPopupOperationException($"Unable to close Popup because it is blocked by the Modal Page {currentVisibleModalPage.GetType().FullName}. Please call `{nameof(Page.Navigation)}.{nameof(Page.Navigation.PopModalAsync)}()` to first remove {currentVisibleModalPage.GetType().FullName} from the {nameof(Page.Navigation.ModalStack)}");
class InvalidPopupOperationException(in string message) : InvalidOperationException(message);